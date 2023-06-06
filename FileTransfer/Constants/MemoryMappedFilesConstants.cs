using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.Constants
{
    /// <summary>
    /// Constants that are related to logic of work with memory mapped files
    /// </summary>
    internal class MemoryMappedFilesConstants
    {
        /// <summary>
        /// Name of a memory mapped file that contains files' descriptors
        /// </summary>
        public const string FileDescriptorsFileName = "FILES_DESCRIPTORS_MAPPED_FILE";

        /// <summary>
        /// Max count of descriptors that might be stored in the system
        /// </summary>
        public const int MaxDescriptorsCount = 5;
    }
}
