using System;

class Book
{
    public int Id { get; set; }
    public int GenreId { get; set; }
    public string Title { get; set; }

    private int _pages;

    public int Pages
    {
        get { return _pages; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Страниц не может быть меньше 0");
            _pages = value;
        }
    }

    public Book(int id, int genreId, string title, int pages)
    {
        Id = id;
        GenreId = genreId;
        Title = title;
        Pages = pages;
    }

    public Book() : this(0, 0, "", 0) { }

    public override string ToString()
    {
        return $"[{Id}] {Title}, жанр #{GenreId}, страниц: {Pages}";
    }
}