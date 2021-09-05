using System;
using System.IO;
using Azure;
using DigitalLibrary.Models.Entities;
using DigitalLibrary.Models.ProfileModels;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibrary.API.Services.FileManager
{
    public class FileManager
    {
        private readonly IWebHostEnvironment _env;

        public FileManager(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void SingleFile(IFormFile file)
        {
            var dir = _env.ContentRootPath;

            using (var fileStream =
                new FileStream(Path.Combine(dir, "file.png"), FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }
        }

        public void UploadImage(IFormFile file, string Id)
        {
            var dir = Path.Combine(_env.WebRootPath, "images", "profile");

            var contentType = file.ContentType;

            var type = "";

            switch (contentType)
            {
                case "image/png":
                    type = ".jpeg";
                    break;
                case "image/jpg":
                    type = ".jpeg";
                    break;
                case "image/jpeg":
                    type = ".jpeg";
                    break;
                default:
                    return;
            }

            using (var fileStream =
                new FileStream(Path.Combine(dir, string.Concat(Id, type)), FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }
        }

        public FileStream GetImageStream(string id)
        {
            var path = Path.Combine(_env.WebRootPath, "images", "profile", string.Concat(id, ".jpeg"));

            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}
