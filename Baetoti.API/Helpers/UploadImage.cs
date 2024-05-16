using Baetoti.Shared.Extentions;
using Baetoti.Shared.Response.FileUpload;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.API.Helpers
{
    public class UploadImage
    {
        public async Task<FileUploadResponse> UploadImageFile(IFormFile file, string DirectoryName)
        {
            FileUploadResponse _result = new FileUploadResponse();
            try
            {
                if (CheckIfOnlyImageFile(file))
                {
                    string fileName = null;
                    var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                    fileName = DateTime.Now.ToTimeZoneTime("Arab Standard Time").Ticks + extension; //Create a new Name for the file due to security reasons.
                    var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\Uploads\\{DirectoryName}");
                    if (!Directory.Exists(pathBuilt))
                    {
                        Directory.CreateDirectory(pathBuilt);
                    }
                    var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\Uploads\\{DirectoryName}", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    _result.FileName = fileName;
                    _result.Path = $"/Uploads/{DirectoryName}";
                    _result.PathwithFileName = $"/Uploads/{DirectoryName}/{fileName}";
                    return _result;
                }
                else
                {
                    _result.Message = "File format is incorrect! (only .png,.jpg,.jpeg) is Supported";
                    return _result = new FileUploadResponse();
                }
            }
            catch (Exception ex)
            {
                _result.Message = ex.Message;
                return _result;
            }
        }

        //Get Image File Extention
        private bool CheckIfOnlyImageFile(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension.ToUpper() == ".PNG" || extension.ToUpper() == ".JPG" || extension.ToUpper() == ".JPEG"); // Change the extension based on your need
        }
    }
}
