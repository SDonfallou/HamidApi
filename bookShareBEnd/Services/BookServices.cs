using AutoMapper;
using bookShareBEnd.Database;
using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace bookShareBEnd.Services
{
    public class Bookservices
    {
        private AppDbContext _context;
        private IMapper _mapper;
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
            if (_book is not  null) 
            {
              _mapper.Map(book,_book);
                _context.SaveChanges();
                return _mapper.Map<BookDTO>(_book);
            }
            return null;
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
    

        public async Task<UserLoansDTO> GetUserLoans(Guid userId)
        {
            var userLoans = await _context.users
                .Where(u => u.UserId == userId)
                .Select(u => new UserLoansDTO
                {
                    UserId = userId,
                    UserName = u.UserName,
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

        public void DeleteBook(Guid bookId)
        {
            var book = _context.books.FirstOrDefault(b=>b.Id == bookId);
            if(book != null)
            {
                _context.books.Remove(book);
                _context.SaveChanges();
            }
        }


    }
}
