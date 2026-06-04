using System;
using System.Windows.Forms;
using HW3.Data;
using HW3.Forms;

namespace HW3
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            using var db = new AppDbContext();
            db.Initialize();
            Application.Run(new MainForm());
        }
    }
}