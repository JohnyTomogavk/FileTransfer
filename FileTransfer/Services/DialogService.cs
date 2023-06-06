using Microsoft.Win32;

namespace FileTransfer.Services
{
    internal class DialogService
    {
        public static string? GetFileNameFromUser()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();

            return fileDialog.FileName;
        }
    }
}
