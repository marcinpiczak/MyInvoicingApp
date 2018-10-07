using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoicingApp.Models
{
    public class DocumentNumber : BaseTable
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        public string DocumentSequenceId { get; set; }

        [ForeignKey("DocumentSequenceId")]
        public DocumentSequence DocumentSequence { get; set; }

        [Required]
        [MaxLength(5)]
        public string DocumentNumberPart1 { get; set; }

        [Required]
        [MaxLength(1)]
        public string DocumentNumberPart1Separator { get; set; }

        public DocumentNumber()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}