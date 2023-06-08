using System;
using FileTransfer.Models;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;

namespace FileTransfer.Services.Abstract
{
    internal interface IMemoryMappedFileService : IDisposable
    {
        /// <summary>
        /// Tries to extract created File Descriptors from shared memory if they are created
        /// </summary>
        /// <returns>Created descriptors or empty array</returns>
        public IEnumerable<FileDescriptor>? GetExistingDescriptors();

        /// <summary>
        /// Writes descriptor to memory mapped file
        /// </summary>
        /// <param name="descriptor">File descriptor</param>
        /// <returns>Written descriptor</returns>
        public void WriteDescriptorToFile(FileDescriptor[] descriptor);

        public MemoryMappedFile CreateMemoryMappedFileFromFile(string fileName, string fileFullName);
    }
}
