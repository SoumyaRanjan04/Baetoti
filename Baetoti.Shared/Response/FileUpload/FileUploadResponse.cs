using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.FileUpload
{
    public class FileUploadResponse
    {
        public string Path { get; set; }
        public string FileName { get; set; }
        public string PathwithFileName { get; set; }
        public string Message { get; set; }
    }
}
