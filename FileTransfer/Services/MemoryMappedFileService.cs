using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text.Json;
using FileTransfer.Constants;
using FileTransfer.Models;
using FileTransfer.Services.Abstract;

namespace FileTransfer.Services;

internal class MemoryMappedFileService : IMemoryMappedFileService
{
    private MemoryMappedFile descriptorsFile;
    private readonly Dictionary<string, MemoryMappedFile> _memoryMappedFiles = new();

    public MemoryMappedFileService()
    {
        try
        {
            descriptorsFile = MemoryMappedFile.OpenExisting(MemoryMappedFilesConstants.DescriptorsFileName);
        }
        catch (FileNotFoundException)
        {
        }
    }

    public IEnumerable<FileDescriptor>? LoadExistingDescriptors()
    {
        var receivedDescriptors = Array.Empty<FileDescriptor>();

        if (descriptorsFile == null)
        {
            try
            {
                descriptorsFile = MemoryMappedFile.OpenExisting(MemoryMappedFilesConstants.DescriptorsFileName);
            }
            catch (FileNotFoundException)
            {
                return receivedDescriptors;
            }
        }

        ;

        var size = ReadDescriptorsSizeFromMappedFile(descriptorsFile);

        if (size != 0)
        {
            var countCharsInJson = size / sizeof(char);
            receivedDescriptors = ReadFileDescriptorsFromMappedFile(descriptorsFile,
                MemoryMappedFilesConstants.SizeOfSerializedDescriptorsInSharedMemory, countCharsInJson);
        }

        return receivedDescriptors;
    }

    private int ReadDescriptorsSizeFromMappedFile(MemoryMappedFile file)
    {
        using var memoryMappedFileView = file.CreateViewAccessor(0,
            MemoryMappedFilesConstants.SizeOfSerializedDescriptorsInSharedMemory, MemoryMappedFileAccess.Read);

        return memoryMappedFileView.ReadInt32(0);
    }

    private FileDescriptor[] ReadFileDescriptorsFromMappedFile(MemoryMappedFile file, int readOffset,
        int countItemsToRead)
    {
        using var memoryMappedFileView =
            file.CreateViewAccessor(readOffset, countItemsToRead * sizeof(char), MemoryMappedFileAccess.Read);

        var serializedDescriptors = new char[countItemsToRead];
        memoryMappedFileView.ReadArray(0, serializedDescriptors, 0, countItemsToRead);

        return JsonSerializer.Deserialize<FileDescriptor[]>(new string(serializedDescriptors));
    }

    public void WriteDescriptorToMemoryFile(FileDescriptor[] descriptors)
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

        using var viewAccessor = descriptorsFile.CreateViewAccessor(0, fullSize);

        viewAccessor.Write(0, descriptorSizeInBytes);
        viewAccessor.WriteArray(MemoryMappedFilesConstants.SizeOfSerializedDescriptorsInSharedMemory,
            serializedDescriptor, 0, serializedDescriptor.Length);
    }

    public MemoryMappedFile CreateMemoryMappedFileFromFile(string fileName, string fileFullName, long fileLength)
    {
        var memoryMappedFile =
            MemoryMappedFile.CreateFromFile(fileFullName, FileMode.Open, fileName, fileLength == 0 ? 1 : fileLength);
        _memoryMappedFiles.Add(fileName, memoryMappedFile);

        return memoryMappedFile;
    }

    public MemoryMappedViewStream GetFileStreamFromMemoryMappedFile(string fileName, long fileSize)
    {
        var memoryFile = MemoryMappedFile.OpenExisting(fileName);
        var stream = memoryFile.CreateViewStream(0, fileSize);
        return stream;
    }

    public void RemoveFileFromStorageByName(string fileName)
    {
        var file = _memoryMappedFiles.First(t => t.Key == fileName);
        file.Value.Dispose();
        _memoryMappedFiles.Remove(fileName);
    }

    public bool IsFileHostedLocally(string fileName)
    {
        return _memoryMappedFiles.Any(t => t.Key == fileName);
    }

    public bool DoesSpecifiedDescriptorFileExist(string fileName)
    {
        var descriptors = LoadExistingDescriptors();
        return descriptors.Any(t => t.FileName == fileName);
    }

    public void RemoveLocalDescriptorsFromMemoryFile()
    {
        var existingDescriptors = this.LoadExistingDescriptors();
        var allDescriptorsWithoutLocal =
            existingDescriptors.Where(t => !_memoryMappedFiles.ContainsKey(t.FileName)).ToArray();

        WriteDescriptorToMemoryFile(allDescriptorsWithoutLocal);
    }

    public void Dispose()
    {
        descriptorsFile?.Dispose();
        _memoryMappedFiles.Values.ToList().ForEach(item => { item?.Dispose(); });
        _memoryMappedFiles.Clear();
    }
}