using CRUDWithDapperORMD8.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CRUDWithDapperORMD8.Models;


namespace CRUDWithDapperORMD8.Repos.IRepository
{
    public interface IBookRepository
    {
        public Task<IEnumerable<BookDTO>> GetAllBooksAsync();
        public Task<BookDTO> GetBookByIdAsync(int id);
        public Task<bool> AddBookAsync(BookDTO book);
        public Task<bool> UpdateBookAsync(Book book);
        public Task<bool> DeleteBookAsync(int id);
    }
}
