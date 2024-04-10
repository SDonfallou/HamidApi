using bookShareBEnd.Database;
using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Model;
using bookShareBEnd.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("Get-All-Books")]
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

        [HttpPost("Add-book")]
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

        [HttpPut("Update-book")]
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


        [HttpPost("Add-Or-Update/{BookID}")] // Syncro with Add And Update Methode
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

                if(bookUpdated == null) 
                {
                    return BadRequest("Book not Found ");
                }
                return Ok("Book updated successfully");    
            }

        }

        [HttpGet("GetAllBooksLoaned")]
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
            return null;
        }


        [HttpDelete("DeleteBookLoaned/{BookLoanedId}")]
        public async Task<IActionResult> DeleteBookLoaned(Guid BookLoanedID)
        {
            await _bookservices.DeleteLoan(BookLoanedID);
            return Ok();
        }

        [HttpDelete("Delete-book/{bookId}")]
        public async Task<IActionResult> DeleteBookByID(Guid BookId)
        {
           await _bookservices.GetBookById(BookId);
            return Ok();
        }
    }
}
