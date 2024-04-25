using AutoMapper;
using bookShareBEnd.Database;
using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;
using bookShareBEnd.Database.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace bookShareBEnd.Services
{
    public class Bookservices
    {
        private readonly AppDbContext _context;
       
        private IMapper _mapper;
        private readonly TimeSpan _debounceDelay = TimeSpan.FromSeconds(1);
        private CancellationTokenSource _debounceCancellationTokenSource;
        public Bookservices(AppDbContext context, IMapper mapper)
        {
            _context = context;  
            _mapper = mapper;
           

        }
        
        public   List<BookDTO> GetAllBooks()
        {
            var books =  _context.books.ToList();
            var booksDTOS =  _mapper.Map<List<BookDTO>>(books);
            
            return booksDTOS;   
        }

        public async Task<List<BookDTO>> GetBooksPagined(int pageNumber)
        {
            var pageSize = 15; 
            var books = _context.books.AsQueryable();

            
            int itemsToSkip = (pageNumber - 1) * pageSize;

            
            var paginatedBooks = await books
                                        .OrderBy(x => x.Id) 
                                        .Skip(itemsToSkip)
                                        .Take(pageSize)
                                        .ToListAsync();

            var paginatedBooksDTO = paginatedBooks.Select(book => _mapper.Map<BookDTO>(book)).ToList();

            return paginatedBooksDTO;
        }

        public async Task<BookDTO> GetBookById(Guid bookId) 
        {
           var book = await _context.books.FindAsync(bookId);
            return _mapper.Map<BookDTO>(book);
        }

        public void AddBook(BookDTO book)
        {
            var _book = _mapper.Map<BookDTO, Books>(book);
            _context.books.Add(_book);
            _context.SaveChanges();
        }

        public BookDTO UpdateBookByID(Guid bookId,BookDTO book)
        {
            var _book = _context.books.FirstOrDefault(b => b.Id == bookId);
            if (_book is  null) 
            {
                throw new Exception("Il Libro che Vuoi cancellare Non Esistente");
            }
              _mapper.Map(book,_book);
                _context.SaveChanges();
                return _mapper.Map<BookDTO>(_book);
           
        }

       

        public async Task<List<BookLoanDTO>> GetAllLoansBook() // prendere tutti gli libri in prestita 
        {
            var bookLoaned     = _context.bookLoan.ToList();
            return _mapper.Map<List<BookLoanDTO>>(bookLoaned);

        }

        public async Task LoanBookToUser(Guid bookId, Guid userId)
        {
            // Check if the book and user exist
            var book = await _context.bookLoan.FindAsync(bookId);
            var user = await _context.users.FindAsync(userId);

            if (book == null)
            {
                throw new Exception("Book not found.");
            }

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Create a new BookLoan entity
            var bookLoan = new BookLoan
            {
                LoanId = Guid.NewGuid(), // Generate a new unique identifier for the loan
                BookLoanId = bookId,
                UserId = userId,
                LoanDate = DateTimeOffset.Now // Set the loan date to the current date and time
            };

            // Add the book loan to the database context
            _context.bookLoan.Add(bookLoan);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task<List<BookDTO>> Top10MostLikedBooks()
        {
            var top10LikedBooks = await _context.books
                                                 .OrderByDescending(x => x.Likes)
                                                 .Take(10)
                                                 .ToListAsync();

            var top10LikedBooksDTO = top10LikedBooks.Select(book => _mapper.Map<BookDTO>(book)).ToList();

            if (top10LikedBooks is  null){
                throw new Exception("Non ci sono libri con piu like");
            }

            return top10LikedBooksDTO;
        }



        public async Task<BookDTO> SearchBook(string searchTerm)
        {
            try
            {
                // Cancel the previous debounce task if it exists
                _debounceCancellationTokenSource?.Cancel();

                // Create a new cancellation token source for the debounce task
                _debounceCancellationTokenSource = new CancellationTokenSource();

                // Wait for the debounce delay
                await Task.Delay(_debounceDelay, _debounceCancellationTokenSource.Token);

                // Actual search logic here
                var book = await SearchBooksAsync(searchTerm);

                // If book is not found, return null
                if (book == null) throw new Exception("User Doesn't Exist");
                    
                // Convert the Book entity to BookDTO
                var bookDto = new BookDTO
                {
                    Title = book.Title,
                    Author = book.Author,
                    // Map other properties as needed
                };

                return bookDto;
            }
            catch (TaskCanceledException)
            {
                // The task was canceled due to debounce reset, return null
                throw new Exception("The Book Sheared Doesn't exist");
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                Console.WriteLine($"Error searching for book: {ex.Message}");
                throw;
            }
        }

        public async Task likeBook(int bookId)
        {
            var book = await _context.books.FindAsync(bookId);

            if (book == null)
            {
                throw new Exception("book not Found");
            }
                book.Likes++;
                await _context.SaveChangesAsync();

        }

        public async Task UnlikeBook(int bookId)
        {
            var book = await _context.books.FindAsync(bookId);
            if (book == null)
            {
                throw new Exception("Book not found");
            }

            if (book != null && book.Likes > 0)
            {
                book.Likes--; 
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<BookDTO>> GetListUserBook(ClaimsPrincipal user)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming user ID is stored as a NameIdentifier claim
            if (userIdString == null)
            {
                throw new Exception("User ID not found in claims.");
            }

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                throw new Exception("Invalid user ID format.");
            }

            var userDB = await _context.users.FindAsync(userId);
            if (userDB is null)
            {
                throw new Exception("User doesn't exist");
            }

            var userBooks = await _context.books
                                          .Where(b => b.UserId == userId)
                                          .Select(b => new BookDTO
                                          {
                                           
                                              Title = b.Title,
                                              Author = b.Author,
                                              //BookCity = b.bookCity,
                                              ShortDescription = b.ShortDescription,
                                              FullDescription = b.FullDescription,
                                              YearPublished = b.YearPublished,
                                              Cover = b.Cover,
                                              Category = b.Category,
                                              Likes = b.Likes,

                                          })
                                          .ToListAsync();

            return userBooks;
        }

        public async Task DeletebookUser(ClaimsPrincipal user, Guid idBook)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user ID directly from the ClaimsPrincipal

            if (string.IsNullOrEmpty(userIdString))
            {
                throw new Exception("User ID not found in claims.");
            }

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                throw new Exception("Invalid user ID format.");
            }

            var userDB = await _context.users.FindAsync(userId);

            if (userDB is null)
            {
                throw new Exception("User doesn't exist");
            }

            // Ensure that the book belongs to the user
            var book = await _context.books.FirstOrDefaultAsync(b => b.Id == idBook && b.UserId == userId);

            if (book == null)
            {
                throw new Exception("Book not found or doesn't belong to the user.");
            }

            // Delete the book from the database
            _context.books.Remove(book);
            await _context.SaveChangesAsync();
        }




        public async Task<UserLoansDTO> GetUserLoans(Guid userId)
        {
            var userLoans = await _context.users
                .Where(u => u.UserId == userId)
                .Select(u => new UserLoansDTO
                {
                    UserId = userId,
                    Name = u.Name,
                    Loans = u.BookLoans.Select(b1 => new BookLoanDTO
                    {
                        LoanId = b1.LoanId,
                        BookLoanId = b1.BookLoanId,
                        LoanerBookId = b1.LoanerBookId,
                        UserId = b1.UserId
                    }).ToList()
                }).FirstOrDefaultAsync();

            return userLoans;
        }

        public async Task DeleteLoan(Guid loanId)
        {
            var loan = await _context.bookLoan.FindAsync(loanId);

            if (loan == null)
            {
                throw new ArgumentException("Loan not found.");
            }

            _context.bookLoan.Remove(loan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBook(Guid bookId)
        {
            var book = _context.books.FirstOrDefault(b=>b.Id == bookId);
            if(book == null)
            {
                throw new Exception("book doesn't exist");
            }
            _context.books.Remove(book);
            _context.SaveChanges();
        }



        private async Task<BookDTO> SearchBooksAsync(string searchTerm)
        {
            var searchIndb = _context.books.FirstOrDefault(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm));
            var search = _mapper.Map<BookDTO>(searchIndb);
            return search;
            
        }


    }
}
