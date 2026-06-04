using System;
using System.Windows.Forms;

namespace HW3.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            Text = "Управление книгами";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new System.Drawing.Size(350, 350);

            Button btnGenres = new Button { Text = "Жанры", Location = new System.Drawing.Point(50, 50), Size = new System.Drawing.Size(220, 40) };
            Button btnBooks = new Button { Text = "Книги", Location = new System.Drawing.Point(50, 110), Size = new System.Drawing.Size(220, 40) };
            Button btnReports = new Button { Text = "Отчёты", Location = new System.Drawing.Point(50, 170), Size = new System.Drawing.Size(220, 40) };
            Button btnExit = new Button { Text = "Выход", Location = new System.Drawing.Point(50, 230), Size = new System.Drawing.Size(220, 40) };

            btnGenres.Click += (s, e) => new GenresForm().ShowDialog();
            btnBooks.Click += (s, e) => new BooksForm().ShowDialog();
            btnReports.Click += (s, e) => new ReportsForm().ShowDialog();
            btnExit.Click += (s, e) => Close();

            Controls.Add(btnGenres);
            Controls.Add(btnBooks);
            Controls.Add(btnReports);
            Controls.Add(btnExit);
        }
    }
}