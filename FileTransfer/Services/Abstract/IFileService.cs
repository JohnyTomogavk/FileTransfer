using FileTransfer.Models;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace FileTransfer.Services.Abstract
{
    internal interface IFileService
    {
        public FileDescriptor GetFileDescriptorFromFileInfo(FileInfo fileInfo);

        public void SaveMemoryMappedStreamToLocalFile(MemoryMappedViewStream stream, string downloadPath, string newFileName, long fileLength);
    }
}
