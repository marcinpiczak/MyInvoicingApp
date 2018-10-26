using System.Threading.Tasks;
using MyInvoicingApp.ViewModels;

namespace MyInvoicingApp.Interfaces
{
    public interface IFileHelper
    {
        string GetUniqueFileName(string fileName);

        Task SaveFile(AttachmentViewModel model, string filePath);

        bool FileExists(string filePath);

        bool DeleteFile(string filePath);
    }
}