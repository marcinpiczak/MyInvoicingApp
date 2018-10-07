using System;
using System.ComponentModel.DataAnnotations;

namespace MyInvoicingApp.Models
{
    public class Attachment : BaseTable
    {
        [Required]
        public string FileDescription { get; set; }
        
        [Required]
        public string OriginalFileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        public string DocumentId { get; set; }

        public Attachment()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}