using System;
using FileTransfer.Models;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;

namespace FileTransfer.Services.Abstract
{
    internal interface IMemoryMappedFileService : IDisposable
    {
        public IEnumerable<FileDescriptor>? LoadExistingDescriptors();

        public void WriteDescriptorToMemoryFile(FileDescriptor[] descriptor);

        public MemoryMappedFile CreateMemoryMappedFileFromFile(string fileName, string fileFullName, long fileLength);

        public MemoryMappedViewStream GetFileStreamFromMemoryMappedFile(string fileName, long fileSize);

        public void RemoveFileFromStorageByName(string fileName);

        public bool DoesSpecifiedDescriptorFileExist(string fileName);

        public bool IsFileHostedLocally(string fileName);

        public void RemoveLocalDescriptorsFromMemoryFile();
    }
}
