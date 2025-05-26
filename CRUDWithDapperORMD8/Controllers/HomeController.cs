using System.Diagnostics;
using System.Reflection;
using CRUDWithDapperORMD8.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CRUDWithDapperORMD8.Models.DTOs;
using CRUDWithDapperORMD8.Repos.IRepository;

namespace CRUDWithDapperORMD8.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IBookRepository bookRepository, IMapper mapper)
        {
            _logger = logger;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Index action method to display all books.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var books = await _bookRepository.GetAllBooksAsync();
                var bookDTOs = _mapper.Map<IEnumerable<BookDTO>>(books);
                if (bookDTOs == null)
                {
                    ModelState.AddModelError("", "No books found");
                    return View(Enumerable.Empty<BookDTO>());
                }
                return View("Index", bookDTOs);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while fetching books");
                _logger.LogError(ex, "Error fetching books");
                return View("Index", Enumerable.Empty<BookDTO>());
            }
        }
        /// <summary>
        /// GetBookByIdAsync action method to fetch a book by its ID. Not Implemented in the UI.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBookByIdAsync(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("", "Invalid book ID");
                return View("Index");
            }
            try
            {
                var book = await _bookRepository.GetBookByIdAsync(id);
                if (book == null)
                {
                    ModelState.AddModelError("", "Book not found");
                    return View("Index");
                }
                return View(book);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while fetching the book");
                _logger.LogError(ex, "Error fetching book with ID {Id}", id);
                return View("Index");
            }
        }

        //we define the view method same name as the actual view for redirection.
        public IActionResult AddBookAsyncView()
        {
            return View();
        }
        /// <summary>
        /// AddBookAsync action method to add a new book.
        /// I learned to explicitly add Route because it took me more than 1h to find out why it's not hitting my controller methods.
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Home/AddBookAsync")]
        public async Task<IActionResult> AddBookAsync(BookDTO book)
        {
            if (!ModelState.IsValid || book == null)
            {
                Console.WriteLine("Invalid model state or book data.");
                return View("AddBookAsyncView", book);
            }
            
            try
            {
                bool isAdded = await _bookRepository.AddBookAsync(book);
                if (isAdded)
                {
                    TempData["SuccessMessage"] = "Book added successfully.";
                    Console.WriteLine("Book added successfully.");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Book creation failed");
                    return View("AddBookAsyncView", book);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the book");
                _logger.LogError(ex, "Error creating book");
                return View("AddBookAsyncView", book);
            }
        }

        //Update
        /// <summary>
        /// UpdateBookAsync action method to update an existing book.
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Home/UpdateBookAsync")]
        public async Task<IActionResult> UpdateBookAsync(BookDTO book)
        {
            if (!ModelState.IsValid || book == null)
            {
                TempData["ErrorMessage"] = "Invalid data submitted.";
                return RedirectToAction("Index");
            }
            
            try
            {
                var bookModel = _mapper.Map<Book>(book);
                bool isUpdated = await _bookRepository.UpdateBookAsync(bookModel);
                if (isUpdated)
                {
                    TempData["SuccessMessage"] = "Book updated successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Book update failed.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the book.";
                _logger.LogError(ex, "Error updating book with ID {Id}", book.Id);
                return RedirectToAction("Index");
            }
        }

        //Delete
        /// <summary>
        /// DeleteBookAsync action method to delete a book by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Home/DeleteBookAsync")]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid book ID";
                return RedirectToAction("Index");
            }
            
            try
            {
                bool isDeleted = await _bookRepository.DeleteBookAsync(id);
                if (isDeleted)
                {
                    TempData["SuccessMessage"] = "Book deleted successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Book deletion failed.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the book.";
                _logger.LogError(ex, "Error deleting book with ID {Id}", id);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
