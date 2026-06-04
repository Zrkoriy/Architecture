using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HW3.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<Book> Books { get; set; } = new List<Book>();
        public override string ToString() => Name;
    }
}