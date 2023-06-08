using System;
using System.IO;
using FileTransfer.Models;
using FileTransfer.Services.Abstract;

namespace FileTransfer.Services
{
    internal class FileService : IFileService
    {
        public FileDescriptor GetFileDescriptorFromFileInfo(FileInfo fileInfo)
        {
            return new FileDescriptor
            {
                FileNameLength = fileInfo.Name.Length,
                FileName = fileInfo.Name,
                FileLength = fileInfo.Length,
                CreatedDate = DateTime.Now,
            };
        }
    }
}
