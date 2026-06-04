using Microsoft.EntityFrameworkCore;
using HW3.Models;

namespace HW3.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=app.db");

        public void Initialize()
        {
            Database.EnsureCreated();

            if (!Genres.Any())
            {
                Genres.AddRange(
                    new Genre { Name = "Роман" },
                    new Genre { Name = "Детектив" },
                    new Genre { Name = "Фантастика" },
                    new Genre { Name = "Исторический" },
                    new Genre { Name = "Поэзия" }
                );
                SaveChanges();
            }

            if (!Books.Any())
            {
                Books.AddRange(
                    new Book { GenreId = 1, Title = "Война и мир", Pages = 1300 },
                    new Book { GenreId = 1, Title = "Анна Каренина", Pages = 800 },
                    new Book { GenreId = 1, Title = "Преступление и наказание", Pages = 670 },
                    new Book { GenreId = 2, Title = "Убийство в Восточном экспрессе", Pages = 250 },
                    new Book { GenreId = 2, Title = "Смерть на Ниле", Pages = 300 },
                    new Book { GenreId = 2, Title = "Десять негритят", Pages = 280 },
                    new Book { GenreId = 3, Title = "Дюна", Pages = 600 },
                    new Book { GenreId = 3, Title = "Автостопом по галактике", Pages = 200 },
                    new Book { GenreId = 3, Title = "1984", Pages = 330 },
                    new Book { GenreId = 4, Title = "Троя", Pages = 500 },
                    new Book { GenreId = 4, Title = "Александр Македонский", Pages = 450 },
                    new Book { GenreId = 4, Title = "Спартак", Pages = 420 },
                    new Book { GenreId = 5, Title = "Евгений Онегин", Pages = 350 }
                );
                SaveChanges();
            }
        }
    }
}