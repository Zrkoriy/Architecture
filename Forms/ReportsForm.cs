using System;
using System.Linq;
using System.Windows.Forms;
using HW3.Data;
using Microsoft.EntityFrameworkCore;

namespace HW3.Forms
{
    public partial class ReportsForm : Form
    {
        public ReportsForm()
        {
            Text = "Отчёты";
            Size = new System.Drawing.Size(800, 700);
            StartPosition = FormStartPosition.CenterScreen;

            Label lbl1 = new Label { Text = "1. Список книг:", Location = new System.Drawing.Point(10, 10), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            dgv1 = new DataGridView { Location = new System.Drawing.Point(10, 35), Size = new System.Drawing.Size(760, 180), AllowUserToAddRows = false };

            Label lbl2 = new Label { Text = "2. Количество по жанрам:", Location = new System.Drawing.Point(10, 230), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            dgv2 = new DataGridView { Location = new System.Drawing.Point(10, 255), Size = new System.Drawing.Size(760, 150), AllowUserToAddRows = false };

            Label lbl3 = new Label { Text = "3. Среднее страниц по жанрам:", Location = new System.Drawing.Point(10, 420), Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold) };
            dgv3 = new DataGridView { Location = new System.Drawing.Point(10, 445), Size = new System.Drawing.Size(760, 180), AllowUserToAddRows = false };

            Controls.Add(lbl1);
            Controls.Add(dgv1);
            Controls.Add(lbl2);
            Controls.Add(dgv2);
            Controls.Add(lbl3);
            Controls.Add(dgv3);

            LoadReports();
        }

        private DataGridView dgv1, dgv2, dgv3;

        private void LoadReports()
        {
            using var db = new AppDbContext();

            var r1 = db.Books.Include(x => x.Genre).OrderBy(x => x.Title).Select(x => new { Название = x.Title, Жанр = x.Genre != null ? x.Genre.Name : "", Страницы = x.Pages }).ToList();
            dgv1.DataSource = r1;

            var r2 = db.Books.Include(x => x.Genre).GroupBy(x => x.Genre != null ? x.Genre.Name : "Без жанра").Select(g => new { Жанр = g.Key, Количество = g.Count() }).OrderBy(x => x.Жанр).ToList();
            dgv2.DataSource = r2;

            var r3 = db.Books.Include(x => x.Genre).Where(x => x.Genre != null).GroupBy(x => x.Genre!.Name).Select(g => new { Жанр = g.Key, Среднее = Math.Round(g.Average(x => x.Pages), 1) }).OrderByDescending(x => x.Среднее).ToList();
            dgv3.DataSource = r3;

            foreach (var dgv in new[] { dgv1, dgv2, dgv3 }) dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}