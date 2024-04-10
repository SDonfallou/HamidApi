using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;
using bookShareBEnd.Database;

namespace bookShareBEnd.Services
{
    
        public class RolesService
        {
            private AppDbContext _context;

            public RolesService(AppDbContext context)
            {
                _context = context;
            }

            public List<Roles> GetAllRoles()
            {
                return _context.roles.ToList();
            }

            public Roles GetRoleById(Guid id)
            {
                return _context.roles.FirstOrDefault(r => r.Id == id);
            }

            public void AddRole(RolesDTO role)
            {
                try
                {
                    var newRole = new Roles()
                    {
                        Id = Guid.NewGuid(), // Generate a new unique identifier for the role
                        Label = role.Label // Set the label of the role based on the view model
                    };

                    _context.roles.Add(newRole);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                    Console.WriteLine($"Error occurred while adding role: {ex.Message}");
                }
            }

            public void UpdateRole(Roles role)
            {
                try
                {
                    var existingRole = _context.roles.FirstOrDefault(r => r.Id == role.Id);
                    if (existingRole != null)
                    {
                        // Update properties of the existing role
                        existingRole.Label = role.Label;

                        _context.roles.Update(existingRole);
                        _context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine($"Role with ID {role.Id} not found.");
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                    Console.WriteLine($"Error occurred while updating role: {ex.Message}");
                }
            }


            public void DeleteRoleById(Guid id)
            {
                var role = _context.roles.FirstOrDefault(r => r.Id == id);
                if (role != null)
                {
                    _context.roles.Remove(role);
                    _context.SaveChanges();
                }
            }
        }
    
}
