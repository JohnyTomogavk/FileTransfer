namespace FileTransfer.Services.Abstract
{
    interface IUserConfigService
    {
        string GetDownloadFolder();
        void SetDownloadFolder(string downloadFolder);
    }
}
