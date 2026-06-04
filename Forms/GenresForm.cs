using System;
using System.Linq;
using System.Windows.Forms;
using HW3.Data;
using HW3.Models;
using Microsoft.VisualBasic;

namespace HW3.Forms
{
    public partial class GenresForm : Form
    {
        public GenresForm()
        {
            Text = "Жанры";
            Size = new System.Drawing.Size(500, 400);
            StartPosition = FormStartPosition.CenterScreen;

            dgvGenres = new DataGridView { Dock = DockStyle.Top, Height = 250 };
            btnAdd = new Button { Text = "Добавить", Location = new System.Drawing.Point(50, 270), Size = new System.Drawing.Size(100, 30) };
            btnEdit = new Button { Text = "Редактировать", Location = new System.Drawing.Point(160, 270), Size = new System.Drawing.Size(100, 30) };
            btnDelete = new Button { Text = "Удалить", Location = new System.Drawing.Point(270, 270), Size = new System.Drawing.Size(100, 30) };
            btnClose = new Button { Text = "Закрыть", Location = new System.Drawing.Point(380, 270), Size = new System.Drawing.Size(100, 30) };

            btnAdd.Click += (s, e) => AddGenre();
            btnEdit.Click += (s, e) => EditGenre();
            btnDelete.Click += (s, e) => DeleteGenre();
            btnClose.Click += (s, e) => Close();

            Controls.Add(dgvGenres);
            Controls.Add(btnAdd);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            Controls.Add(btnClose);

            LoadGenres();
        }

        private DataGridView dgvGenres;
        private Button btnAdd, btnEdit, btnDelete, btnClose;

        private void LoadGenres()
        {
            using var db = new AppDbContext();
            dgvGenres.DataSource = db.Genres.OrderBy(x => x.Name).ToList();
            dgvGenres.Columns["Id"].Width = 50;
            dgvGenres.Columns["Name"].Width = 200;
            dgvGenres.Columns["Books"].Visible = false;
        }

        private void AddGenre()
        {
            string name = Interaction.InputBox("Введите название жанра:", "Добавление");
            if (string.IsNullOrWhiteSpace(name)) return;
            using var db = new AppDbContext();
            db.Genres.Add(new Genre { Name = name });
            db.SaveChanges();
            LoadGenres();
        }

        private void EditGenre()
        {
            if (dgvGenres.CurrentRow == null) return;
            var g = (Genre)dgvGenres.CurrentRow.DataBoundItem;
            string newName = Interaction.InputBox("Редактирование:", "Изменить", g.Name);
            if (string.IsNullOrWhiteSpace(newName)) return;
            using var db = new AppDbContext();
            var genre = db.Genres.Find(g.Id);
            if (genre != null)
            {
                genre.Name = newName;
                db.SaveChanges();
            }
            LoadGenres();
        }

        private void DeleteGenre()
        {
            if (dgvGenres.CurrentRow == null) return;
            var g = (Genre)dgvGenres.CurrentRow.DataBoundItem;
            using var db = new AppDbContext();
            if (db.Books.Any(b => b.GenreId == g.Id))
            {
                MessageBox.Show("Нельзя удалить жанр с книгами!", "Ошибка");
                return;
            }
            if (MessageBox.Show($"Удалить {g.Name}?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                db.Genres.Remove(g);
                db.SaveChanges();
                LoadGenres();
            }
        }
    }
}