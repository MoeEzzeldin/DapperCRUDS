using System.Diagnostics;
using System.Reflection;
using CRUDWithDapperORMD8.Data;
using CRUDWithDapperORMD8.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace CRUDWithDapperORMD8.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBcontext _dbContext;

        public HomeController(ILogger<HomeController> logger, DBcontext dbcontext)
        {
            _logger = logger;
            _dbContext = dbcontext;
        }


        public IActionResult GetAll()
        {
            using var conn = _dbContext.Connection;
            var books = conn.Query<Book>("SELECT * FROM Books");
            return View(books);
        }

        public IActionResult GetById(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("", "Invalid book ID");
                return View("Index");
            }
            using var conn = _dbContext.Connection;
            string sql = "SELECT * FROM Books WHERE Id = @Id";
            var book = conn.QuerySingleOrDefault<Book>(sql, new { Id = id });
            if (book == null)
            {
                ModelState.AddModelError("", "Book not found");
                return View("Index");
            }
            return View(book);
        }
        //we define the view method same name as the actual view for redirection.
        public IActionResult Create()
        {
            return View();
        }
        // we call the method the same name as the view for redirection.
        [HttpPost]
        public IActionResult Create(Book book)
        {
            using var conn = _dbContext.Connection;
            string sql = "INSERT INTO Books (Title, Author, Price) OUTPUT INSERTED.Id VALUES (@Title, @Author, @Price)";
            var insertedId = conn.Execute(sql, new { Title = book.Title, Author = book.Author, Price = book.Price });
            // int id = Convert.ToInt32(conn.ExecuteScalar(sql, new { Title = title, Author = author, Price = price })); whats the difference between using AddWithParameters and Dapper Dynamic Parameters?
            return View(book);
        }

        public IActionResult Edit (Book book)
        {
            if (book == null)
            {
                ModelState.AddModelError("", "Book not found");
            }
            using var conn = _dbContext.Connection;
            string sql = "UPDATE Books SET Title = @Title, Author = @Author, Price = @Price WHERE Id = @Id";
            var updatedId = conn.Execute(sql, new { Title = book.Title, Author = book.Author, Price = book.Price, Id = book.Id });
            if(updatedId > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Update failed");
            }
            return View(book);
        }

        public IActionResult DeleteBook(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError("", "Invalid book ID");
                return View("Index");
            }
            using var conn = _dbContext.Connection;
            string sql = "DELETE FROM Books WHERE Id = @Id";
            var deletedId = conn.Execute(sql, new { Id = id });
            if (deletedId > 0)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Delete failed");
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
