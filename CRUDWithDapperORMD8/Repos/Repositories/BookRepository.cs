using CRUDWithDapperORMD8.Data;
using CRUDWithDapperORMD8.Models;
using CRUDWithDapperORMD8.Models.DTOs;
using AutoMapper;
using CRUDWithDapperORMD8.Repos.IRepository;
using Dapper;


namespace CRUDWithDapperORMD8.Repos.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly DBcontext _dbContext;
        private readonly IMapper _mapper;
        public BookRepository(DBcontext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// GetAllBooksAsync method to fetch all books from the database.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            try
            {
                using var conn = _dbContext.Connection;
                var books = await conn.QueryAsync<Book>("SELECT * FROM Books");
                var bookDTOs = _mapper.Map<List<BookDTO>>(books);
                if (bookDTOs == null || bookDTOs.Count == 0)
                {
                    throw new Exception("No books found");
                }
                return bookDTOs;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching books", ex);
            }
        }

        /// <summary>
        /// GetBookByIdAsync method to fetch a book by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BookDTO> GetBookByIdAsync(int id)
        {
            try
            {
                using var conn = _dbContext.Connection;
                string sql = "SELECT * FROM Books WHERE Id = @Id";
                var book = await conn.QuerySingleOrDefaultAsync<Book>(sql, new { Id = id });
                BookDTO bookDTO = _mapper.Map<BookDTO>(book);
                if (bookDTO == null)
                {
                    throw new Exception("Book not found");
                }
                return bookDTO;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the book", ex);
            }
        }

        /// <summary>
        /// AddBookAsync method to add a new book to the database.
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> AddBookAsync(BookDTO book)
        {
            try
            {
                using var conn = _dbContext.Connection;
                string sql = "INSERT INTO Books (Title, Author, Price) OUTPUT INSERTED.Id VALUES (@Title, @Author, @Price)";
                var insertedId = await conn.ExecuteScalarAsync(sql, new { Title = book.Title, Author = book.Author, Price = book.Price });

                return insertedId != null && (int)insertedId > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the book", ex);
            }
        }

        /// <summary>
        /// UpdateBookAsync method to update an existing book in the database.
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> UpdateBookAsync(Book book)
        {
            try
            {
                bool isUpdated = true;

                using var conn = _dbContext.Connection;
                string sql = "UPDATE Books SET Title = @Title, Author = @Author, Price = @Price WHERE Id = @Id";
                var updatedId = await conn.ExecuteAsync(sql, new { Title = book.Title, Author = book.Author, Price = book.Price, Id = book.Id });
                if (updatedId > 0)
                {
                    return isUpdated;
                }
                else
                {
                    return !isUpdated;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the book", ex);
            }
        }

        /// <summary>
        /// DeleteBookAsync method to delete a book by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                bool isDeleted = true;
                using var conn = _dbContext.Connection;
                string sql = "DELETE FROM Books WHERE Id = @Id";
                var deletedId = await conn.ExecuteAsync(sql, new { Id = id });
                if (deletedId > 0)
                {
                    return isDeleted;
                }
                else
                {
                    return !isDeleted;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the book", ex);
            }
        }

    }
    
}
