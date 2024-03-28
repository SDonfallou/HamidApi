using AutoMapper;
using bookShareBEnd.Database;
using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;

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
            _context.Add(_book);
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
