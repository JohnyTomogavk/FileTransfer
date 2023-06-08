using System;
using System.IO;

namespace FileTransfer.Models;

internal class UserConfig
{
    public string DownloadDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
}