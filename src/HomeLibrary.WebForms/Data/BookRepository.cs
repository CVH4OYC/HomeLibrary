using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using HomeLibrary.WebForms.Models;

namespace HomeLibrary.WebForms.Data
{
    public class BookRepository
    {
        private readonly string _connectionString;

        public BookRepository()
        {
            var setting = ConfigurationManager.ConnectionStrings["HomeLibrary"];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
            {
                throw new InvalidOperationException("Строка подключения 'HomeLibrary' не найдена в Web.config.");
            }
            _connectionString = setting.ConnectionString;
        }

        public List<Book> GetAll(string search = null)
        {
            var list = new List<Book>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_Book_Select", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 300).Value = search.Trim();
                }
                else
                {
                    cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 300).Value = DBNull.Value;
                }

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(ReadBook(reader));
                    }
                }
            }
            return list;
        }

        public Book GetById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_Book_GetById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadBook(reader);
                    }
                }
            }
            return null;
        }

        public int Insert(Book book)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_Book_Insert", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 300).Value = book.Title;
                cmd.Parameters.Add("@Author", SqlDbType.NVarChar, 200).Value = book.Author;
                cmd.Parameters.Add("@PublicationYear", SqlDbType.Int).Value = (object)book.PublicationYear ?? DBNull.Value;
                cmd.Parameters.Add("@Publisher", SqlDbType.NVarChar, 200).Value = (object)book.Publisher ?? DBNull.Value;
                cmd.Parameters.Add("@ISBN", SqlDbType.NVarChar, 20).Value = (object)book.ISBN ?? DBNull.Value;
                cmd.Parameters.Add("@PageCount", SqlDbType.Int).Value = (object)book.PageCount ?? DBNull.Value;
                cmd.Parameters.Add("@Genre", SqlDbType.NVarChar, 100).Value = (object)book.Genre ?? DBNull.Value;
                cmd.Parameters.Add("@TableOfContents", SqlDbType.Xml).Value = (object)book.TableOfContents ?? DBNull.Value;

                var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(newIdParam);

                conn.Open();
                cmd.ExecuteNonQuery();
                return (int)newIdParam.Value;
            }
        }

        public int Update(Book book)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_Book_Update", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = book.Id;
                cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 300).Value = book.Title;
                cmd.Parameters.Add("@Author", SqlDbType.NVarChar, 200).Value = book.Author;
                cmd.Parameters.Add("@PublicationYear", SqlDbType.Int).Value = (object)book.PublicationYear ?? DBNull.Value;
                cmd.Parameters.Add("@Publisher", SqlDbType.NVarChar, 200).Value = (object)book.Publisher ?? DBNull.Value;
                cmd.Parameters.Add("@ISBN", SqlDbType.NVarChar, 20).Value = (object)book.ISBN ?? DBNull.Value;
                cmd.Parameters.Add("@PageCount", SqlDbType.Int).Value = (object)book.PageCount ?? DBNull.Value;
                cmd.Parameters.Add("@Genre", SqlDbType.NVarChar, 100).Value = (object)book.Genre ?? DBNull.Value;
                cmd.Parameters.Add("@TableOfContents", SqlDbType.Xml).Value = (object)book.TableOfContents ?? DBNull.Value;

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 1;
            }
        }

        public int Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.usp_Book_Delete", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 1;
            }
        }

        private static Book ReadBook(SqlDataReader reader)
        {
            return new Book
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Author = reader.GetString(reader.GetOrdinal("Author")),
                PublicationYear = reader.IsDBNull(reader.GetOrdinal("PublicationYear")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PublicationYear")),
                Publisher = reader.IsDBNull(reader.GetOrdinal("Publisher")) ? null : reader.GetString(reader.GetOrdinal("Publisher")),
                ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString(reader.GetOrdinal("ISBN")),
                PageCount = reader.IsDBNull(reader.GetOrdinal("PageCount")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("PageCount")),
                Genre = reader.IsDBNull(reader.GetOrdinal("Genre")) ? null : reader.GetString(reader.GetOrdinal("Genre")),
                TableOfContents = reader.IsDBNull(reader.GetOrdinal("TableOfContents")) ? null : reader.GetString(reader.GetOrdinal("TableOfContents")),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
