using FileTransfer.Services.Abstract;
using Microsoft.Win32;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace FileTransfer.Services
{
    internal class DialogService : IDialogService
    {
        public string? GetFileByDialog()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();

            return fileDialog.FileName;
        }

        public string? GetFolderByDialog()
        {
            using var dialog = new FolderBrowserDialog();
            
            var result = dialog.ShowDialog();

            return dialog.SelectedPath;
        }
    }
}
