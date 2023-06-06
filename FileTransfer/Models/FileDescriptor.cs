using System;

namespace FileTransfer.Models;

/// <summary>
/// A file's parameters, that are used for file saving
/// </summary>
internal struct FileDescriptor
{
    /// <summary>
    /// Filename
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Length of file in bytes
    /// </summary>
    public long FileLength { get; set; }

    /// <summary>
    /// Date when file was loaded to the system
    /// </summary>
    public DateTime CreatedDate { get; set; }
}
