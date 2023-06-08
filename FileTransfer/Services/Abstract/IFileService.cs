using FileTransfer.Models;
using System.IO;

namespace FileTransfer.Services.Abstract
{
    internal interface IFileService
    {
        public FileDescriptor GetFileDescriptorFromFileInfo(FileInfo fileInfo);
    }
}
