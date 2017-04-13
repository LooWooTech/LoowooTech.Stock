using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LoowooTech.Stock.Common
{
    public static class DialogClass
    {
        public static string OpenFile(string filter,string title)
        {
            var fileName = string.Empty;
            var openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = filter;
            openfileDialog.Title = title;
            if (openfileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openfileDialog.FileName;
            }
            return fileName;
        }

        public static string SaveFile(string filter,string title)
        {
            var saveFile = string.Empty;
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            saveFileDialog.Title = title;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                saveFile = saveFileDialog.FileName;
            }
            return saveFile;
        }

        public static string SelectFolder()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return string.Empty;
        }

        public static List<string> GetSpecialFiles(string folder, string filter)
        {
            var dir = new DirectoryInfo(folder);
            var files = dir.GetFiles(filter);
            return files.Select(e => e.FullName).ToList();
        }
    }
}
