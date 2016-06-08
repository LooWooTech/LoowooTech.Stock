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
    }
}
