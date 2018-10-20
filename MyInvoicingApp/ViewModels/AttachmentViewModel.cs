using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class AttachmentViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Opis dla pliku musi zostać uzupełniony")]
        [DisplayName("Opis dla pliku/ów")]
        public string FileDescription { get; set; }

        [DisplayName("Nazwa pliku")]
        public string OriginalFileName { get; set; }

        [Required(ErrorMessage = "Typ dokumentu musi zostać podany")]
        [DisplayName("Typ dokumentu")]
        public DocumentType DocumentType { get; set; }

        [Required(ErrorMessage = "Id dokumentu źródłowego musi być wprowadzone")]
        [DisplayName("Typ dokumentu")]
        public string DocumentId { get; set; }

        [Required(ErrorMessage = "Nalezy wybrać jakiś plik")]
        [DisplayName("Pliki")]
        public IFormFile File { get; set; }

        public AttachmentViewModel()
        {
        }

        public AttachmentViewModel(Attachment model)
        {
            Id = model.Id;
            Status = model.Status;
            CreatedBy = model.CreatedBy;
            CreatedDate = model.CreatedDate;
            FileDescription = model.FileDescription;
            OriginalFileName = model.OriginalFileName;
            DocumentType = model.DocumentType;
            DocumentId = model.DocumentId;
        }
    }
}