using FileTransfer.Services.Abstract;
using Microsoft.Win32;

namespace FileTransfer.Services
{
    internal class DialogService : IDialogService
    {
        public string? SelectFileDialog()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();

            return fileDialog.FileName;
        }
    }
}
