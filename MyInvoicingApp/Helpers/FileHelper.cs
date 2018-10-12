using System;
using System.IO;
using System.Threading.Tasks;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Helpers
{
    public class FileHelper
    {
        public string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString()
                   + Path.GetExtension(fileName);
        }

        public async Task SaveFile(AttachmentViewModel model, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                await model.File.CopyToAsync(stream);
            }
        }

        public bool FileExists(string filePath)
        {
            var exists = System.IO.File.Exists(filePath);

            return exists;
        }

        public bool DeleteFile(string filePath)
        {

            var exists = FileExists(filePath);

            if (exists)
            {
                System.IO.File.Delete(filePath);

                return true;
            }
            
            return false;
        }
    }
}