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

        public HomeController(ILogger<HomeController> logger, IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;
        }

        public IActionResult Index()
        {
            return View("Index");
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBooksAsync()
        {
            try
            {
                var books = await _bookRepository.GetAllBooksAsync();
                if (books == null)
                {
                    ModelState.AddModelError("", "No books found");
                    return View(Enumerable.Empty<BookDTO>());
                }
                return View("Index",books);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while fetching books");
                _logger.LogError(ex, "Error fetching books");
                return View("Index",Enumerable.Empty<BookDTO>());
            }
        }

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
                BookDTO book = await _bookRepository.GetBookByIdAsync(id);
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

        [HttpPost]
        public async Task<IActionResult> AddBookAsync(BookDTO book)
        {
            if (book == null)
            {
                ModelState.AddModelError("", "Book not found");
                return View("Index");
            }
            try
            {
                bool isAdded = await _bookRepository.AddBookAsync(book);
                if (isAdded)
                {
                    return RedirectToAction("GetAllBooksAsync");
                }
                else
                {
                    ModelState.AddModelError("", "Book creation failed");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the book");
                _logger.LogError(ex, "Error creating book");
            }

            return RedirectToAction("GetAllBooksAsync");
        }

        //Update
        [HttpPost]
        public async Task<IActionResult> UpdateBookAsync(Book book)
        {
            if (book == null)
            {
                ModelState.AddModelError("", "Book not found");
            }
            try
            {
                bool isUpdated = await _bookRepository.UpdateBookAsync(book);
                if (isUpdated)
                {
                    return RedirectToAction("GetAllBooksAsync");
                }
                else
                {
                    ModelState.AddModelError("", "Book update failed");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the book");
                _logger.LogError(ex, "Error updating book with ID {Id}", book.Id);
            }

            return View(book);
        }
        //Delete
        [HttpPost]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("", "Invalid book ID");
                return View("Index");
            }
            try
            {
                bool isDeleted = await _bookRepository.DeleteBookAsync(id);
                if (isDeleted)
                {
                    return RedirectToAction("GetAllBooksAsync");
                }
                else
                {
                    ModelState.AddModelError("", "Book deletion failed");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the book");
                _logger.LogError(ex, "Error deleting book with ID {Id}", id);
            }

            return View();
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
