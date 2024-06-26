﻿using bookShareBEnd.Database.DTO;
using bookShareBEnd.Database.Net;
using bookShareBEnd.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace bookShareBEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
       
        private Bookservices _bookservices;
        private IValidator<BookDTO> _validator;
        

        public BookController(Bookservices bookservices, IValidator<BookDTO> validator)
        {
            _bookservices=bookservices;
            _validator=validator;
        }
        [AllowAnonymous]
        [HttpGet("GetAllBooks")]
        public IActionResult GetAllBooks()
        {
            var allBooks = _bookservices.GetAllBooks();
            return Ok(allBooks);
        }

        [AllowAnonymous]

        [HttpGet("GetBookById/{BookId}")]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult GetBookByID(Guid BookId)
        {

            var book = _bookservices.GetBookById(BookId);
            return Ok(book.Result);
        }
        
        [HttpGet("Top10MostLikedBooks")]
        public async Task<IActionResult> Top10MostLikedBooks()
        {
            var books =await _bookservices.Top10MostLikedBooks();
            return Ok(books);
        }

        [HttpGet("RecentBooksUploaded")]
        public async Task<IActionResult> RecentBookUploaded()
        {
            var books = await _bookservices.GetLastBooksUploaded();
            return Ok(books);
        }
        [AllowAnonymous]
        [HttpPost("AddBook")]
       // [Authorize(Policy = "UserPolicy")]
       // [Authorize(Policy = "AdminPolicy")]
        public async  Task<IActionResult> AddBook([FromBody]BookDTO bookDTO)
        {
            /*var user = HttpContext.GetIdFromToken();
            if (user == null)
            {
                return Unauthorized("non sei autorizzato");
            }*/
            var validationResult = await _validator.ValidateAsync(bookDTO);

            if (!validationResult.IsValid)
            {
                // If validation fails, return the validation errors
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }
            bookDTO.DateAdded = DateTime.UtcNow;
            _bookservices.AddBook(bookDTO);
            return Ok("Book added successfully");
        }

        [HttpPut("Admin/UpdateBook/{BookId}")]
        //[Authorize(Policy = "UserPolicy")]
        public async Task <IActionResult> UpdateBookbyID(Guid bookId, [FromBody]BookDTO bookDTO)
        {
            var validationResult =  _validator.Validate(bookDTO);
                if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }
           var updatedbook =  _bookservices.UpdateBookByID(bookId, bookDTO);
            return Ok(updatedbook);
        }
        
        [HttpPost("like/{bookId}")]
        public async Task<IActionResult> LikeBook(Guid bookId)
        {
           
            var user =  HttpContext.GetIdFromToken();
                   
            if (user == null)
            {
                return Unauthorized(); // Return 401 Unauthorized if the user is not authenticated
            }

            // check if the bookid is valid
            if (bookId == Guid.Empty)
            {
                return BadRequest("invalid bookid"); // return 400 bad request if bookid is empty
            }

            // Call your service method to like the book
            try
            {
                await _bookservices.LikeBook(bookId);
                return Ok(); // Return 200 OK if the operation is successful
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while liking the book"); // Return 500 Internal Server Error if an exception occurs
            }
        }


        [HttpPost("unlike/{bookId}")]
        public async Task<IActionResult> UnlikeBook(Guid bookId)
        {
            var user = HttpContext.GetIdFromToken();
            if (user == null)
            {
                return Unauthorized(); // Return 401 Unauthorized if the user is not authenticated
            }
            
            if(bookId == Guid.Empty)
            { 
                return BadRequest();
            }
            await _bookservices.UnlikeBook(bookId);
            return Ok();    
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost("Admin/AddOrUpdate/{BookID}")] // Syncro with Add And Update Methode
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

            var userId = HttpContext.GetIdFromToken();

            if (userId == null || userId == Guid.Empty)
            {
                return BadRequest("Invalid or missing user ID in token.");
            }

            var books = await _bookservices.GetListUserBook(userId);

            if (books == null)
            {
                return NotFound("User doesn't have any books.");
            }

            return Ok(books);
        }

        [HttpDelete("UserDeleteBook/{BookID}")]
        public async Task<IActionResult> UserDeleteBook(Guid BookId)
        {
            var user = HttpContext.GetIdFromToken();  
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            await _bookservices.DeletebookUser(user, BookId);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("SearchBook/{searchItem}")]
        public async Task<IActionResult> SearchBook(string? searchItem)
        {
            if (string.IsNullOrEmpty(searchItem))
            {
                return BadRequest("Search term cannot be null or empty.");
            }

            var books = await _bookservices.SearchBook(searchItem);
            if (books == null || books.Count == 0)
            {
                return NotFound("No books found.");
            }

            return Ok(books);
        }


        [HttpGet("Admin/GetAllBooksLoaned")]
        public async Task<IActionResult> GetAllBooksLoaned()
        {
            await _bookservices.GetAllLoansBook();
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("books/{pageNumber}")]
        public  async Task<IActionResult> GetBooksPagined(int pageNumber)
        {
            var booksPagined = await _bookservices.GetBooksPagined(pageNumber);
            return Ok(booksPagined);
        }

        [HttpPost("takeLoanBook{userId}")]
        public async Task<IActionResult> TakeLoanBook(Guid userid, Guid BookId)
        {
            

            await _bookservices.LoanBookToUser(userid, BookId);
            return Ok();
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
