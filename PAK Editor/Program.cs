using System;
using System.IO;
using System.Windows.Forms;

namespace PAK_Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                string filePath = args[0];
                if (File.Exists(filePath))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    WinMain window = new WinMain();
                    window.fileOpen_Click(null, null, filePath);
                    Application.Run(window);
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new WinMain());
            }
        }
    }
}
