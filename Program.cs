using System;
using System.Windows.Forms;

namespace Unidad4.Logistica
{
    /// <summary>Punto de entrada de la aplicación de la Tarea 4.3 (Dijkstra en logística).</summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
