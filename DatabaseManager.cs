using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

class DatabaseManager
{
    private string _connectionString;

    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }

    public void InitializeDatabase(string genresCsv, string booksCsv)
    {
        CreateTables();

        if (GetAllGenres().Count == 0 && File.Exists(genresCsv))
            ImportGenres(genresCsv);

        if (GetAllBooks().Count == 0 && File.Exists(booksCsv))
            ImportBooks(booksCsv);
    }

    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS genres (
                genre_id INTEGER PRIMARY KEY AUTOINCREMENT,
                genre_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS books (
                book_id INTEGER PRIMARY KEY AUTOINCREMENT,
                genre_id INTEGER NOT NULL,
                book_title TEXT NOT NULL,
                pages INTEGER NOT NULL,
                FOREIGN KEY (genre_id) REFERENCES genres(genre_id)
            );";
        cmd.ExecuteNonQuery();
    }

    private void ImportGenres(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO genres (genre_id, genre_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
    }

    private void ImportBooks(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4) continue;
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO books (book_id, genre_id, book_title, pages) VALUES (@id, @gid, @title, @pages)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@gid", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@title", parts[2]);
            cmd.Parameters.AddWithValue("@pages", int.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
    }

    public List<Genre> GetAllGenres()
    {
        var list = new List<Genre>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT genre_id, genre_name FROM genres ORDER BY genre_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(new Genre(reader.GetInt32(0), reader.GetString(1)));
        return list;
    }

    public List<Book> GetAllBooks()
    {
        var list = new List<Book>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT book_id, genre_id, book_title, pages FROM books ORDER BY book_id";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(new Book(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3)));
        return list;
    }

    public Book GetBookById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT book_id, genre_id, book_title, pages FROM books WHERE book_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return new Book(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3));
        return null;
    }

    public void AddBook(Book book)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO books (genre_id, book_title, pages) VALUES (@gid, @title, @pages)";
        cmd.Parameters.AddWithValue("@gid", book.GenreId);
        cmd.Parameters.AddWithValue("@title", book.Title);
        cmd.Parameters.AddWithValue("@pages", book.Pages);
        cmd.ExecuteNonQuery();
    }

    public void UpdateBook(Book book)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE books SET genre_id = @gid, book_title = @title, pages = @pages WHERE book_id = @id";
        cmd.Parameters.AddWithValue("@id", book.Id);
        cmd.Parameters.AddWithValue("@gid", book.GenreId);
        cmd.Parameters.AddWithValue("@title", book.Title);
        cmd.Parameters.AddWithValue("@pages", book.Pages);
        cmd.ExecuteNonQuery();
    }

    public void DeleteBook(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM books WHERE book_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public (string[] cols, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();

        string[] cols = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            cols[i] = reader.GetName(i);

        var rows = new List<string[]>();
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }
        return (cols, rows);
    }
}