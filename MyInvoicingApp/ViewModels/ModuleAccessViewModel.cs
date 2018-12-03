using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyInvoicingApp.Models;

namespace MyInvoicingApp.ViewModels
{
    public class ModuleAccessViewModel : BaseViewModel
    {
        [Required]
        public AccessorType AccessorType { get; set; }

        [Required]
        [MaxLength(450)]
        public string AccessorId { get; set; }

        [Required]
        [DisplayName("Moduł")]
        public Models.Controllers Module { get; set; }

        [Required]
        [DisplayName("Lista")]
        public bool Index { get; set; }

        [Required]
        [DisplayName("Dodawanie")]
        public bool Add { get; set; }

        [Required]
        [DisplayName("Edytowanie")]
        public bool Edit { get; set; }

        [Required]
        [DisplayName("Zamykanie")]
        public bool Close { get; set; }

        [Required]
        [DisplayName("Otwieranie")]
        public bool Open { get; set; }

        [Required]
        [DisplayName("Anulowanie")]
        public bool Cancel { get; set; }

        [Required]
        [DisplayName("Wysyłanie do akceptacji")]
        public bool Send { get; set; }

        [Required]
        [DisplayName("Wyświetlanie szczegółów")]
        public bool Details { get; set; }

        [Required]
        [DisplayName("Akceptowanie")]
        public bool Approve { get; set; }

        [Required]
        [DisplayName("Usuwanie")]
        public bool Remove { get; set; }

        public ModuleAccessViewModel()
        {
        }

        public ModuleAccessViewModel(ModuleAccess model)
        {
            Id = model.Id;
            Status = model.Status;
            CreatedBy = model.CreatedBy;
            CreatedDate = model.CreatedDate;
            LastModifiedBy = model.LastModifiedBy;
            LastModifiedDate = model.LastModifiedDate;

            AccessorType = model.AccessorType;
            AccessorId = model.AccessorId;
            Module = model.Module;
            Index = model.Index;
            Add = model.Add;
            Edit = model.Edit;
            Close = model.Close;
            Open = model.Open;
            Cancel = model.Cancel;
            Send = model.Send;
            Details = model.Details;
            Approve = model.Approve;
            Remove = model.Remove;
        }
    }
}