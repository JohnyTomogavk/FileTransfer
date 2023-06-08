using System;
using System.IO;
using FileTransfer.Models;
using System.IO.MemoryMappedFiles;
using System.Text.Json;
using FileTransfer.Constants;
using FileTransfer.Services.Abstract;
using System.Collections.Generic;

namespace FileTransfer.Services;

internal class MemoryMappedFileService : IMemoryMappedFileService
{
    private MemoryMappedFile descriptorsFile = null;
    private readonly List<MemoryMappedFile> fileContent = new();

    public MemoryMappedFileService()
    {
        try
        {
            descriptorsFile = MemoryMappedFile.OpenExisting(MemoryMappedFilesConstants.DescriptorsFileName);
        }
        catch (FileNotFoundException)
        {
            // Do nothing
        }
    }

    public IEnumerable<FileDescriptor>? GetExistingDescriptors()
    {
        var receivedDescriptors = Array.Empty<FileDescriptor>();

        if (descriptorsFile == null) return receivedDescriptors;

        var size = ReadDescriptorsSizeFromMappedFile(descriptorsFile);
        var countCharsInJson = size / sizeof(char);

        if (size != 0)
        {
            receivedDescriptors = ReadFileDescriptorsFromMappedFile(descriptorsFile,
                MemoryMappedFilesConstants.SizeOfSerializedDescriptorsInSharedMemory, countCharsInJson);
        }

        return receivedDescriptors;
    }

    #region Descriptor Helpers

    private int ReadDescriptorsSizeFromMappedFile(MemoryMappedFile file)
    {
        var memoryMappedFileView = file.CreateViewAccessor(0,
            MemoryMappedFilesConstants.SizeOfSerializedDescriptorsInSharedMemory, MemoryMappedFileAccess.Read);

        return memoryMappedFileView.ReadInt32(0);
    }

    private FileDescriptor[] ReadFileDescriptorsFromMappedFile(MemoryMappedFile file, int readOffset, int countItemsToRead)
    {
        var memoryMappedFileView = file.CreateViewAccessor(readOffset, countItemsToRead * sizeof(char), MemoryMappedFileAccess.Read);

        var serializedDescriptors = new char[countItemsToRead];
        memoryMappedFileView.ReadArray<char>(0, serializedDescriptors, 0, countItemsToRead);

        return JsonSerializer.Deserialize<FileDescriptor[]>(new string(serializedDescriptors));
    }

    public void WriteDescriptorToFile(FileDescriptor[] descriptors)
    {
        var serializedDescriptor = JsonSerializer.Serialize(descriptors).ToCharArray();
        var descriptorSizeInBytes = serializedDescriptor.Length * sizeof(char);
        var fullSize = descriptorSizeInBytes + MemoryMappedFilesConstants.SizeOfSerializedDescriptorsInSharedMemory;

        if (descriptorsFile == null)
        {
            try
            {
                descriptorsFile = MemoryMappedFile.OpenExisting(MemoryMappedFilesConstants.DescriptorsFileName);
            }
            catch (FileNotFoundException)
            {
                descriptorsFile = MemoryMappedFile.CreateNew(MemoryMappedFilesConstants.DescriptorsFileName, fullSize);
            }
        }

        var viewAccessor = descriptorsFile.CreateViewAccessor(0, fullSize);

        viewAccessor.Write(0, descriptorSizeInBytes);
        viewAccessor.WriteArray(MemoryMappedFilesConstants.SizeOfSerializedDescriptorsInSharedMemory,
            serializedDescriptor, 0, serializedDescriptor.Length);
    }

    #endregion

    public MemoryMappedFile CreateMemoryMappedFileFromFile(string fileName, string fileFullName)
    {
        var memoryMappedFile = MemoryMappedFile.CreateFromFile(fileFullName);
        fileContent.Add(memoryMappedFile);

        return memoryMappedFile;
    }

    public void Dispose()
    {
        descriptorsFile?.Dispose();
        fileContent.ForEach(item =>
        {
            item?.Dispose();
        });
    }
}
