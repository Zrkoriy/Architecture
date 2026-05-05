using System;
using System.IO;

class Program
{
    static void Main()
    {
        string dbPath = "books.db";
        string genresCsv = "genres.csv";
        string booksCsv = "books.csv";

        var db = new DatabaseManager(dbPath);
        db.InitializeDatabase(genresCsv, booksCsv);

        string choice;
        do
        {
            Console.WriteLine("\n=== Управление книгами ===");
            Console.WriteLine("1 - Показать все жанры");
            Console.WriteLine("2 - Показать все книги");
            Console.WriteLine("3 - Добавить книгу");
            Console.WriteLine("4 - Редактировать книгу");
            Console.WriteLine("5 - Удалить книгу");
            Console.WriteLine("6 - Отчёты");
            Console.WriteLine("7 - Фильтр по жанру");
            Console.WriteLine("0 - Выход");
            Console.Write("Выбор: ");
            choice = Console.ReadLine();

            switch (choice)
            {
                case "1": ShowGenres(db); break;
                case "2": ShowBooks(db); break;
                case "3": AddBook(db); break;
                case "4": EditBook(db); break;
                case "5": DeleteBook(db); break;
                case "6": ReportsMenu(db); break;
                case "7": FilterByGenre(db); break;
                case "0": Console.WriteLine("До свидания!"); break;
                default: Console.WriteLine("Неверный выбор"); break;
            }
        } while (choice != "0");
    }

    static void ShowGenres(DatabaseManager db)
    {
        var list = db.GetAllGenres();
        foreach (var g in list)
            Console.WriteLine(g);
        Console.WriteLine($"Всего: {list.Count}");
    }

    static void ShowBooks(DatabaseManager db)
    {
        var list = db.GetAllBooks();
        foreach (var b in list)
            Console.WriteLine(b);
        Console.WriteLine($"Всего: {list.Count}");
    }

    static void AddBook(DatabaseManager db)
    {
        Console.WriteLine("Доступные жанры:");
        foreach (var g in db.GetAllGenres())
            Console.WriteLine(g);

        Console.Write("ID жанра: ");
        if (!int.TryParse(Console.ReadLine(), out int gid))
        {
            Console.WriteLine("Ошибка");
            return;
        }

        Console.Write("Название книги: ");
        string title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Ошибка");
            return;
        }

        Console.Write("Количество страниц: ");
        if (!int.TryParse(Console.ReadLine(), out int pages))
        {
            Console.WriteLine("Ошибка");
            return;
        }

        try
        {
            db.AddBook(new Book(0, gid, title, pages));
            Console.WriteLine("Книга добавлена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static void EditBook(DatabaseManager db)
    {
        Console.Write("ID книги: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Ошибка");
            return;
        }

        var book = db.GetBookById(id);
        if (book == null)
        {
            Console.WriteLine("Книга не найдена");
            return;
        }

        Console.WriteLine($"Текущие данные: {book}");
        Console.WriteLine("Enter - оставить без изменений");

        Console.Write($"Название [{book.Title}]: ");
        string input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
            book.Title = input;

        Console.Write($"ID жанра [{book.GenreId}]: ");
        input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int gid))
            book.GenreId = gid;

        Console.Write($"Страницы [{book.Pages}]: ");
        input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int pages))
        {
            try
            {
                book.Pages = pages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return;
            }
        }

        db.UpdateBook(book);
        Console.WriteLine("Книга обновлена");
    }

    static void DeleteBook(DatabaseManager db)
    {
        Console.Write("ID книги: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Ошибка");
            return;
        }

        var book = db.GetBookById(id);
        if (book == null)
        {
            Console.WriteLine("Книга не найдена");
            return;
        }

        Console.Write($"Удалить '{book.Title}'? (да/нет): ");
        if (Console.ReadLine()?.ToLower() == "да")
        {
            db.DeleteBook(id);
            Console.WriteLine("Книга удалена");
        }
    }

    static void ReportsMenu(DatabaseManager db)
    {
        string choice;
        do
        {
            Console.WriteLine("\n--- Отчёты ---");
            Console.WriteLine("1 - Список книг с жанрами");
            Console.WriteLine("2 - Количество книг по жанрам");
            Console.WriteLine("3 - Среднее страниц по жанрам");
            Console.WriteLine("0 - Назад");
            Console.Write("Выбор: ");
            choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    new ReportBuilder(db)
                        .Query("SELECT b.book_title AS Название, g.genre_name AS Жанр, b.pages AS Страницы FROM books b JOIN genres g ON b.genre_id = g.genre_id ORDER BY b.book_title")
                        .Title("Книги по жанрам")
                        .Header("Название", "Жанр", "Страницы")
                        .ColumnWidths(30, 20, 10)
                        .Numbered()
                        .Print();
                    break;
                case "2":
                    new ReportBuilder(db)
                        .Query("SELECT g.genre_name AS Жанр, COUNT(*) AS Количество FROM books b JOIN genres g ON b.genre_id = g.genre_id GROUP BY g.genre_name ORDER BY Количество DESC")
                        .Title("Количество книг по жанрам")
                        .Header("Жанр", "Кол-во")
                        .ColumnWidths(25, 10)
                        .Numbered()
                        .Print();
                    break;
                case "3":
                    new ReportBuilder(db)
                        .Query("SELECT g.genre_name AS Жанр, ROUND(AVG(b.pages), 1) AS Среднее FROM books b JOIN genres g ON b.genre_id = g.genre_id GROUP BY g.genre_name ORDER BY Среднее DESC")
                        .Title("Среднее страниц по жанрам")
                        .Header("Жанр", "Среднее")
                        .ColumnWidths(25, 15)
                        .Numbered()
                        .Print();
                    break;
            }
        } while (choice != "0");
    }

    static void FilterByGenre(DatabaseManager db)
    {
        Console.WriteLine("Доступные жанры:");
        foreach (var g in db.GetAllGenres())
            Console.WriteLine(g);

        Console.Write("Введите ID жанра: ");
        if (!int.TryParse(Console.ReadLine(), out int genreId))
        {
            Console.WriteLine("Ошибка");
            return;
        }

        var books = db.GetBooksByGenreId(genreId);
        if (books.Count == 0)
        {
            Console.WriteLine("В этом жанре нет книг");
            return;
        }

        var genre = db.GetAllGenres().Find(g => g.Id == genreId);
        Console.WriteLine($"\nКниги жанра {genre?.Name}:");
        foreach (var b in books)
            Console.WriteLine(b);
        Console.WriteLine($"Всего: {books.Count}");
    }
}