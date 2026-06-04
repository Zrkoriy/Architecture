using System;
using System.Linq;
using System.Windows.Forms;
using HW3.Data;
using HW3.Models;
using Microsoft.EntityFrameworkCore;

namespace HW3.Forms
{
    public partial class BooksForm : Form
    {
        public BooksForm()
        {
            Text = "Книги";
            Size = new System.Drawing.Size(700, 500);
            StartPosition = FormStartPosition.CenterScreen;

            Label lblFilter = new Label { Text = "Фильтр по жанру:", Location = new System.Drawing.Point(10, 10), Size = new System.Drawing.Size(100, 25) };
            cbGenreFilter = new ComboBox { Location = new System.Drawing.Point(120, 10), Size = new System.Drawing.Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbGenreFilter.SelectedIndexChanged += (s, e) => LoadBooks();

            dgvBooks = new DataGridView { Location = new System.Drawing.Point(10, 45), Size = new System.Drawing.Size(660, 350), AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            btnAdd = new Button { Text = "Добавить", Location = new System.Drawing.Point(10, 410), Size = new System.Drawing.Size(100, 30) };
            btnEdit = new Button { Text = "Редактировать", Location = new System.Drawing.Point(120, 410), Size = new System.Drawing.Size(100, 30) };
            btnDelete = new Button { Text = "Удалить", Location = new System.Drawing.Point(230, 410), Size = new System.Drawing.Size(100, 30) };
            btnClose = new Button { Text = "Закрыть", Location = new System.Drawing.Point(570, 410), Size = new System.Drawing.Size(100, 30) };

            btnAdd.Click += (s, e) => AddBook();
            btnEdit.Click += (s, e) => EditBook();
            btnDelete.Click += (s, e) => DeleteBook();
            btnClose.Click += (s, e) => Close();

            Controls.Add(lblFilter);
            Controls.Add(cbGenreFilter);
            Controls.Add(dgvBooks);
            Controls.Add(btnAdd);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnClose);

            LoadGenresToFilter();
            LoadBooks();
        }

        private DataGridView dgvBooks;
        private ComboBox cbGenreFilter;
        private Button btnAdd, btnEdit, btnDelete, btnClose;

        private void LoadGenresToFilter()
        {
            using var db = new AppDbContext();
            cbGenreFilter.Items.Clear();
            cbGenreFilter.Items.Add("Все жанры");
            foreach (var g in db.Genres.OrderBy(x => x.Name)) cbGenreFilter.Items.Add(g);
            cbGenreFilter.SelectedIndex = 0;
        }

        private void LoadBooks()
        {
            using var db = new AppDbContext();
            var query = db.Books.Include(x => x.Genre).AsQueryable();
            if (cbGenreFilter.SelectedIndex > 0 && cbGenreFilter.SelectedItem is Genre selected)
                query = query.Where(x => x.GenreId == selected.Id);

            var list = query.OrderBy(x => x.Title).Select(x => new { x.Id, Жанр = x.Genre != null ? x.Genre.Name : "", x.Title, Страницы = x.Pages }).ToList();
            dgvBooks.DataSource = list;
            if (dgvBooks.Columns["Id"] != null) dgvBooks.Columns["Id"].Width = 50;
        }

        private void AddBook()
        {
            using var db = new AppDbContext();
            var genres = db.Genres.ToList();
            if (genres.Count == 0) { MessageBox.Show("Сначала добавьте жанры"); return; }

            var form = new Form { Text = "Добавить книгу", Size = new System.Drawing.Size(350, 200), StartPosition = FormStartPosition.CenterParent };
            ComboBox cb = new ComboBox { Location = new System.Drawing.Point(120, 20), Size = new System.Drawing.Size(180, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            TextBox tbTitle = new TextBox { Location = new System.Drawing.Point(120, 55), Size = new System.Drawing.Size(180, 25) };
            TextBox tbPages = new TextBox { Location = new System.Drawing.Point(120, 90), Size = new System.Drawing.Size(180, 25) };
            Button ok = new Button { Text = "OK", Location = new System.Drawing.Point(80, 130), Size = new System.Drawing.Size(80, 30) };
            Button cancel = new Button { Text = "Отмена", Location = new System.Drawing.Point(180, 130), Size = new System.Drawing.Size(80, 30) };

            foreach (var g in genres) cb.Items.Add(g);
            cb.DisplayMember = "Name";
            cb.SelectedIndex = 0;

            ok.Click += (s, e) =>
            {
                if (cb.SelectedItem == null) { MessageBox.Show("Выберите жанр"); return; }
                if (string.IsNullOrWhiteSpace(tbTitle.Text)) { MessageBox.Show("Введите название"); return; }
                if (!int.TryParse(tbPages.Text, out int p) || p < 0) { MessageBox.Show("Введите число >= 0"); return; }
                db.Books.Add(new Book { GenreId = ((Genre)cb.SelectedItem).Id, Title = tbTitle.Text, Pages = p });
                db.SaveChanges();
                LoadBooks();
                form.Close();
            };
            cancel.Click += (s, e) => form.Close();

            form.Controls.Add(new Label { Text = "Жанр:", Location = new System.Drawing.Point(20, 23) });
            form.Controls.Add(cb);
            form.Controls.Add(new Label { Text = "Название:", Location = new System.Drawing.Point(20, 58) });
            form.Controls.Add(tbTitle);
            form.Controls.Add(new Label { Text = "Страницы:", Location = new System.Drawing.Point(20, 93) });
            form.Controls.Add(tbPages);
            form.Controls.Add(ok);
            form.Controls.Add(cancel);
            form.ShowDialog();
        }

        private void EditBook()
        {
            if (dgvBooks.CurrentRow == null) return;
            int id = (int)dgvBooks.CurrentRow.Cells["Id"].Value;
            using var db = new AppDbContext();
            var book = db.Books.Find(id);
            if (book == null) return;

            var genres = db.Genres.ToList();
            var form = new Form { Text = "Редактировать книгу", Size = new System.Drawing.Size(350, 200), StartPosition = FormStartPosition.CenterParent };
            ComboBox cb = new ComboBox { Location = new System.Drawing.Point(120, 20), Size = new System.Drawing.Size(180, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            TextBox tbTitle = new TextBox { Location = new System.Drawing.Point(120, 55), Size = new System.Drawing.Size(180, 25), Text = book.Title };
            TextBox tbPages = new TextBox { Location = new System.Drawing.Point(120, 90), Size = new System.Drawing.Size(180, 25), Text = book.Pages.ToString() };
            Button ok = new Button { Text = "OK", Location = new System.Drawing.Point(80, 130), Size = new System.Drawing.Size(80, 30) };
            Button cancel = new Button { Text = "Отмена", Location = new System.Drawing.Point(180, 130), Size = new System.Drawing.Size(80, 30) };

            foreach (var g in genres) cb.Items.Add(g);
            cb.DisplayMember = "Name";
            int idx = 0;
            for (int i = 0; i < cb.Items.Count; i++) if (((Genre)cb.Items[i]).Id == book.GenreId) idx = i;
            cb.SelectedIndex = idx;

            ok.Click += (s, e) =>
            {
                if (cb.SelectedItem == null) { MessageBox.Show("Выберите жанр"); return; }
                if (string.IsNullOrWhiteSpace(tbTitle.Text)) { MessageBox.Show("Введите название"); return; }
                if (!int.TryParse(tbPages.Text, out int p) || p < 0) { MessageBox.Show("Введите число >= 0"); return; }
                book.GenreId = ((Genre)cb.SelectedItem).Id;
                book.Title = tbTitle.Text;
                book.Pages = p;
                db.SaveChanges();
                LoadBooks();
                form.Close();
            };
            cancel.Click += (s, e) => form.Close();

            form.Controls.Add(new Label { Text = "Жанр:", Location = new System.Drawing.Point(20, 23) });
            form.Controls.Add(cb);
            form.Controls.Add(new Label { Text = "Название:", Location = new System.Drawing.Point(20, 58) });
            form.Controls.Add(tbTitle);
            form.Controls.Add(new Label { Text = "Страницы:", Location = new System.Drawing.Point(20, 93) });
            form.Controls.Add(tbPages);
            form.Controls.Add(ok);
            form.Controls.Add(cancel);
            form.ShowDialog();
        }

        private void DeleteBook()
        {
            if (dgvBooks.CurrentRow == null) return;
            int id = (int)dgvBooks.CurrentRow.Cells["Id"].Value;
            if (MessageBox.Show("Удалить книгу?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using var db = new AppDbContext();
                var book = db.Books.Find(id);
                if (book != null)
                {
                    db.Books.Remove(book);
                    db.SaveChanges();
                    LoadBooks();
                }
            }
        }
    }
}