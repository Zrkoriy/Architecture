using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HW3.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public int GenreId { get; set; }
        public virtual Genre? Genre { get; set; }
        public string Title { get; set; } = "";
        private int _pages;
        public int Pages
        {
            get => _pages;
            set
            {
                if (value < 0) throw new ArgumentException("Страниц не может быть меньше 0");
                _pages = value;
            }
        }
        public override string ToString() => Title;
    }
}