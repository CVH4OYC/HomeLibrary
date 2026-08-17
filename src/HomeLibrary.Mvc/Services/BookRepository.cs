using System.Data;
using Microsoft.Data.SqlClient;
using HomeLibrary.Mvc.Models;

namespace HomeLibrary.Mvc.Services;

/// <summary>
/// Репозиторий книг на ADO.NET. Все обращения к базе — строго через хранимые процедуры.
/// </summary>
public class BookRepository
{
    private readonly string _connectionString;

    public BookRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("HomeLibrary")
            ?? throw new InvalidOperationException("Строка подключения 'HomeLibrary' не найдена.");
    }

    public async Task<List<Book>> GetAllAsync(string? search = null)
    {
        var list = new List<Book>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("dbo.usp_Book_Select", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 300).Value = search.Trim();
        }
        else
        {
            cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 300).Value = DBNull.Value;
        }

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(ReadBook(reader));
        }

        return list;
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("dbo.usp_Book_GetById", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return ReadBook(reader);
        }

        return null;
    }

    public async Task<int> InsertAsync(Book book)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("dbo.usp_Book_Insert", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 300).Value = book.Title;
        cmd.Parameters.Add("@Author", SqlDbType.NVarChar, 200).Value = book.Author;
        cmd.Parameters.Add("@PublicationYear", SqlDbType.Int).Value = (object?)book.PublicationYear ?? DBNull.Value;
        cmd.Parameters.Add("@Publisher", SqlDbType.NVarChar, 200).Value = (object?)book.Publisher ?? DBNull.Value;
        cmd.Parameters.Add("@ISBN", SqlDbType.NVarChar, 20).Value = (object?)book.ISBN ?? DBNull.Value;
        cmd.Parameters.Add("@PageCount", SqlDbType.Int).Value = (object?)book.PageCount ?? DBNull.Value;
        cmd.Parameters.Add("@Genre", SqlDbType.NVarChar, 100).Value = (object?)book.Genre ?? DBNull.Value;
        cmd.Parameters.Add("@TableOfContents", SqlDbType.Xml).Value = (object?)book.TableOfContents ?? DBNull.Value;

        var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(newIdParam);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        return (int)newIdParam.Value;
    }

    public async Task<int> UpdateAsync(Book book)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("dbo.usp_Book_Update", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = book.Id;
        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 300).Value = book.Title;
        cmd.Parameters.Add("@Author", SqlDbType.NVarChar, 200).Value = book.Author;
        cmd.Parameters.Add("@PublicationYear", SqlDbType.Int).Value = (object?)book.PublicationYear ?? DBNull.Value;
        cmd.Parameters.Add("@Publisher", SqlDbType.NVarChar, 200).Value = (object?)book.Publisher ?? DBNull.Value;
        cmd.Parameters.Add("@ISBN", SqlDbType.NVarChar, 20).Value = (object?)book.ISBN ?? DBNull.Value;
        cmd.Parameters.Add("@PageCount", SqlDbType.Int).Value = (object?)book.PageCount ?? DBNull.Value;
        cmd.Parameters.Add("@Genre", SqlDbType.NVarChar, 100).Value = (object?)book.Genre ?? DBNull.Value;
        cmd.Parameters.Add("@TableOfContents", SqlDbType.Xml).Value = (object?)book.TableOfContents ?? DBNull.Value;

        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 1;
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("dbo.usp_Book_Delete", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 1;
    }

    private static Book ReadBook(SqlDataReader reader)
    {
        return new Book
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Author = reader.GetString(reader.GetOrdinal("Author")),
            PublicationYear = reader.IsDBNull(reader.GetOrdinal("PublicationYear")) ? null : reader.GetInt32(reader.GetOrdinal("PublicationYear")),
            Publisher = reader.IsDBNull(reader.GetOrdinal("Publisher")) ? null : reader.GetString(reader.GetOrdinal("Publisher")),
            ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
            PageCount = reader.IsDBNull(reader.GetOrdinal("PageCount")) ? null : reader.GetInt32(reader.GetOrdinal("PageCount")),
            Genre = reader.IsDBNull(reader.GetOrdinal("Genre")) ? null : reader.GetString(reader.GetOrdinal("Genre")),
            TableOfContents = reader.IsDBNull(reader.GetOrdinal("TableOfContents")) ? null : reader.GetString(reader.GetOrdinal("TableOfContents")),
            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
        };
    }
}
