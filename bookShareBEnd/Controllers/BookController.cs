using bookShareBEnd.Database;
using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;
using bookShareBEnd.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bookShareBEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
       
        private Bookservices _bookservices;
        private IValidator<BookDTO> _validator;

        public BookController(Bookservices bookservices, IValidator<BookDTO> validator )
        {
            
            _bookservices=bookservices;
            _validator=validator;
        }

        [HttpGet("GetAllBooks")]
        public IActionResult GetAllBooks()
        {
            var allBooks = _bookservices.GetAllBooks();
            return Ok(allBooks);
        }

        [HttpGet("Get-book-byId/{BookID}")]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult GetBookByID(Guid BookId)
        {

            var book = _bookservices.GetBookById(BookId);
            return Ok(book);
        }

        [HttpPost("AddBook")]
        [Authorize(Policy = "UserPolicy")]
        public async  Task<IActionResult> AddBook([FromBody]BookDTO bookDTO)
        {
            var validationResult = await _validator.ValidateAsync(bookDTO);

            if (!validationResult.IsValid)
            {
                // If validation fails, return the validation errors
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }
            _bookservices.AddBook(bookDTO);
            return Ok("Book added successfully");
        }

        [HttpPut("Admin/UpdateBook")]
        [Authorize(Policy = "UserPolicy")]
        public  IActionResult UpdateBookbyID(Guid bookId, [FromBody]BookDTO bookDTO)
        {
            var validationResult =  _validator.Validate(bookDTO);
                if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }
           var updatedbook= _bookservices.UpdateBookByID(bookId, bookDTO);
            return Ok(updatedbook);
        }


        [HttpPost("Admin/AddOrUpdate/{BookID}")] // Syncro with Add And Update Methode
        [Authorize(Policy = "UserPolicy")]
        public IActionResult AddOrUpdate(Guid? BookId, [FromBody]BookDTO book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Check if user data is provided in the request body
            if (book == null)
            {
                return BadRequest("Book data is missing.");
            }

            if (!BookId.HasValue)
            {

                _bookservices.AddBook(book);
                return Ok("Book Added Successfully");
            }else
            {

                if(BookId == null || BookId == Guid.Empty )
                {
                    return BadRequest("Invalid Book ID provided.");
                }
               var bookUpdated = _bookservices.UpdateBookByID(BookId.Value,book);

                
                return Ok("Book updated successfully");    
            }

        }

        [HttpGet("UserBooks")]
        public async Task<IActionResult> GetUserBooks()
        {
            var user = HttpContext.User;
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var books = await _bookservices.GetListUserBook(user);

            if (books == null)
            {
                throw new Exception("User doesn't have Book");
            }
            return Ok(books);

        }


        [HttpDelete("UserDeleteBook")]
        public async Task<IActionResult> UserDeleteBook(Guid BookId)
        {
            var user = HttpContext.User;
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            await _bookservices.DeletebookUser(user, BookId);
            return Ok();
        }


        [HttpGet("SearchBook")]
        public async Task<IActionResult> SearchBook(string? searchItem)
        {
            if (searchItem == null)
            {
                throw new ArgumentNullException(nameof(searchItem));
            }

            await _bookservices.SearchBook(searchItem);
            return Ok();
        }

        [HttpGet("Admin/GetAllBooksLoaned")]
        public async Task<IActionResult> GetAllBooksLoaned()
        {
            await _bookservices.GetAllLoansBook();
            return Ok();
        }

        [HttpPost("takeLoanBook{userId}")]
        public async Task<IActionResult> TakeLoanBook(Guid userid, Guid BookId)
        {
            

            await _bookservices.LoanBookToUser(userid, BookId);
            return Ok();
        }

        [HttpGet("GetUserLoans{userId}")]
        public async Task<IActionResult> GetUserLoans(Guid userId)
        {
            return null; //TODO
        }


        [HttpDelete("DeleteBookLoaned/{BookLoanedId}")]
        public async Task<IActionResult> DeleteBookLoaned(Guid BookLoanedID)
        {
            await _bookservices.DeleteLoan(BookLoanedID);
            return Ok();
        }

        [HttpDelete("Admin/Delete-book/{bookId}")]
        public async Task<IActionResult> DeleteBookByID(Guid BookId)
        {
            var user = HttpContext.User;

            if (user == null)
            {
                throw new Exception("User doesn't exist");
            }

            // Check if the user is an admin
            if (!user.IsInRole("Admin")) // Assuming role is stored as a claim named "role"
            {
                return BadRequest("Only admin users can delete books.");
            }

            // Await the asynchronous method call
            await _bookservices.DeleteBook(BookId);
            return Ok();
        }

    }
}
