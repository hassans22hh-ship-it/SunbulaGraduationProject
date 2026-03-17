using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Exceptions
{
    public sealed class FolderNotFoundException:Exception
    {
        public FolderNotFoundException(Guid folderId)
    : base($"Folder with ID '{folderId}' was not found")
        {
            FolderId = folderId;
        }

        public Guid FolderId { get; }
    }
}
