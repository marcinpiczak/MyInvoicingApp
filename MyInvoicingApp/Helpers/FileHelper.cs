using System;
using System.IO;
using System.Threading.Tasks;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Helpers
{
    public class FileHelper
    {
        /// <summary>
        /// Adds guid to file name to make it unique.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>Unique file name with extension</returns>
        public string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString()
                   + Path.GetExtension(fileName);
        }

        /// <summary>
        /// Saves file from attachment View Model to given path.
        /// </summary>
        /// <param name="model">Attachment View Model that contains attachment file</param>
        /// <param name="filePath">File path where file need to be saved</param>
        /// <returns></returns>
        public async Task SaveFile(AttachmentViewModel model, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                await model.File.CopyToAsync(stream);
            }
        }

        /// <summary>
        /// Checks if file exists
        /// </summary>
        /// <param name="filePath">File path of the file that need to be checked</param>
        /// <returns>bool indicating if file exists or not</returns>
        public bool FileExists(string filePath)
        {
            var exists = System.IO.File.Exists(filePath);

            return exists;
        }

        /// <summary>
        /// Delete file 
        /// </summary>
        /// <param name="filePath">File path of the file that need to be checked</param>
        /// <returns>bool indicating if file was deleted or not</returns>
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