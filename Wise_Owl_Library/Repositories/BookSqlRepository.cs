using Microsoft.EntityFrameworkCore;
using Wise_Owl_Library.Models;
using Wise_Owl_Library.Extensions.BusinessExtensions;
using Wise_Owl_Library.Models.Entities;

namespace Wise_Owl_Library.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetBooksAsync(string? title, int? stock);
        Task<Book> GetBookAsync(int id);
        Task<List<Book>> AddBooksAsync(List<Book> books);
        Task<Book> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<bool> BookExistsAsync(string title, List<Author> authorNames);
        Task<bool> BookExistsByIdAsync(int id);
    }
    public class BookSqlRepository(WiseOwlLibraryDbContext wiseOwlLibraryDbContext) : IBookRepository
    {
        public async Task<List<Book>> AddBooksAsync(List<Book> books)
        {
            await wiseOwlLibraryDbContext.Books.AddRangeAsync((books.Select(b => b.ToBookEntity())).ToList());
            
            /*
            List<Book> createdBooks = new();
            foreach (Book book in books)
            {
                //TODO: odchytit exception místo null
                
                if(await wiseOwlLibraryDbContext.Books.AddAsync(book.ToBookEntity()) != null)
                {
                    createdBooks.Add(book);
                    
                }
            }
            */
            await wiseOwlLibraryDbContext.SaveChangesAsync();

            return books;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            BookEntity? book = await wiseOwlLibraryDbContext.Books.FindAsync(id);
            if (book == null)
            {
                return false;
            }

            wiseOwlLibraryDbContext.Books.Remove(book);
            await wiseOwlLibraryDbContext.SaveChangesAsync();
            return true; 
        }

        public async Task<Book> GetBookAsync(int id)
        {
            BookEntity? book = await wiseOwlLibraryDbContext.Books
                .Include(b => b.Authors)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return new Book();
            }
            return book.ToBook();
        }

        public async Task<List<Book>> GetBooksAsync(string? title, int? stock)
        {
            IQueryable<BookEntity> query = wiseOwlLibraryDbContext.Books.Include(b => b.Authors);

            if (title != null)
            {
                query = query.Where(b => b.Title == title);
            }

            if (stock != null)
            {
                query = query.Where(b => b.Stock == stock);
            }

            List<BookEntity> books = await query.ToListAsync();
            return books.Select(b => b.ToBook()).ToList();
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            BookEntity? bookEntity = await wiseOwlLibraryDbContext.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == book.Id);

            if (bookEntity == null)
            {
                throw new KeyNotFoundException($"Book with Id {book.Id} not found.");
            }

            bookEntity.Title = book.Title;
            bookEntity.Price = book.Price;
            bookEntity.Stock = book.Stock;
            bookEntity.Authors = book.Authors.Select(a => a.ToAuthorEntity()).ToList();

            wiseOwlLibraryDbContext.Entry(bookEntity).State = EntityState.Modified;
            await wiseOwlLibraryDbContext.SaveChangesAsync();

            return bookEntity.ToBook();
        }

        public async Task<bool> BookExistsAsync(string title, List<Author> authors)
        {
            return await wiseOwlLibraryDbContext.Books
                .Where(b => b.Title == title)
                .AnyAsync(b =>
                        b.Authors.Count == authors.Count &&
                        !b.Authors
                            .Select(a => a.Id)
                            .Except(authors.Select(a => a.ToAuthorEntity().Id))
                            .Any() &&
                        !authors
                            .Select(a => a.ToAuthorEntity().Id)
                            .Except(b.Authors.Select(a => a.Id))
                            .Any()
                );
            // return await wiseOwlLibraryDbContext.Books.AnyAsync(b => b.Title == title && b.Authors.SequenceEqual(authors.Select(a => a.ToAuthorEntity())));
        }

        public async Task<bool> BookExistsByIdAsync(int id)
        {
            return await wiseOwlLibraryDbContext.Books.AnyAsync(b => b.Id == id);
        }
    }
}
