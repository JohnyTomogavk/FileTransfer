using FileTransfer.Models;
using FileTransfer.Services.Abstract;

namespace FileTransfer.Services;

internal class UserConfigService : IUserConfigService
{
    private readonly UserConfig _userConfig;

    public UserConfigService(UserConfig userConfig)
    {
        _userConfig = userConfig;
    }

    public string GetDownloadFolder()
    {
        return _userConfig.DownloadDirectory;
    }

    public void SetDownloadFolder(string downloadFolder)
    {
        _userConfig.DownloadDirectory = downloadFolder;
    }
}