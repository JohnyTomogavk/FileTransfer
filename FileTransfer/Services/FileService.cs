using System;
using System.IO;
using FileTransfer.Models;

namespace FileTransfer.Services
{
    internal class FileService
    {
        public FileDescriptor GetFileDescriptor(FileInfo fileInfo)
        {
            return new FileDescriptor()
            {
                FileName = fileInfo.Name,
                FileLength = fileInfo.Length,
                CreatedDate = DateTime.Now,
            };
        }
    }
}
