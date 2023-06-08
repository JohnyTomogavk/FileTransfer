namespace FileTransfer.Constants;

/// <summary>
/// Constants that are related to logic of work with memory mapped files
/// </summary>
internal class MemoryMappedFilesConstants
{
    /// <summary>
    /// Name of a memory mapped file that contains files' descriptors
    /// </summary>
    public const string DescriptorsFileName = "FILES_DESCRIPTORS_MAPPED_FILE";

    /// <summary>
    /// Size of variable in bytes that contains count of file descriptors created in memory
    /// </summary>
    public const int SizeOfSerializedDescriptorsInSharedMemory = sizeof(int);
}