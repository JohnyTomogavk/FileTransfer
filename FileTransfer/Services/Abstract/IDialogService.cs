﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileTransfer.Services.Abstract
{
    internal interface IDialogService
    {
        public string? GetFileByDialog();

        public string? GetFolderByDialog();
    }
}
