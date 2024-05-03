using bookShareBEnd.Database;
using bookShareBEnd.Database.DTO;
using bookShareBEnd.Services;
using bookShareBEnd.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace bookShareBEnd
{
    public class Startup
    {
        public string ConnectionString { get; set; }
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetConnectionString("DefaultConnectionString");
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding the Auth with Jwt Token

            // Adding the Auth with Jwt Token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(option => {
                      option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidateAudience = true,
                          ValidateLifetime = true,
                          ValidateIssuerSigningKey = true,
                          ValidIssuer = Configuration["Jwt:Issuer"], // from this part to the closing bracket to change when i deploy the app bcz is an secret key 
                          ValidAudience = Configuration["Jwt:Audience"],
                          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:key"]))
                      };


                  });

            services.AddControllers();
            services.AddTransient<UsersServices>();
            services.AddTransient<Bookservices>();
            services.AddScoped<Bookservices>();

            services.AddSingleton(Configuration);

            // Adding authorization policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserPolicy", policy => policy.RequireClaim("role", "User"));
                options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("role", "Admin"));
            });
            // Add CORS services
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin() // You can restrict this to specific origins if needed
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });


            // confirgure DBcontext with SQL         
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString")));

            // Add of the AutoMapper 
             services.AddAutoMapper(typeof(Startup));
            //confirgure the services
            
            //services.AddTransient<RolesService>();
            services.AddScoped<AuthenticationServices>();

            //Chat Hub
            services.AddSignalR();

            //services.AddIdentity<UserDTO, IdentityRole>()
            //         .AddEntityFrameworkStores<AppDbContext>()
            //         .AddDefaultTokenProviders();

            //services.AddScoped<I>

            // The Fluent Validator         
            services.AddValidatorsFromAssemblyContaining<UserValidator>();
            services.AddValidatorsFromAssemblyContaining<BookValidator>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookShare", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bookshare v1"));
            }

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat", options =>
                {
                    options.Transports =
                        HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                    options.ApplicationMaxBufferSize = 64 * 1024;
                    options.TransportMaxBufferSize = 32 * 1024;
                });
            });


            AppDbInitializer.Seed(app);
        }
    }
}
