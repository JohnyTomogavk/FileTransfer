using System;
using System.IO;
using System.IO.MemoryMappedFiles;
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
                FileName = fileInfo.Name,
                FileLength = fileInfo.Length,
                CreatedDate = DateTime.Now,
            };
        }

        public void SaveMemoryMappedStreamToLocalFile(MemoryMappedViewStream stream, string downloadPath, string newFileName, long fileLength)
        {
            using var fileStream = File.Create($"{downloadPath}/{newFileName}");
            using var reader = new BinaryReader(stream);
            var readedBytes = reader.ReadBytes((int)fileLength);
            fileStream.Write(readedBytes);
        }
    }
}
