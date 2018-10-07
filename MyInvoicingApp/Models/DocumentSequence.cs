using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyInvoicingApp.Models
{
    public class DocumentSequence : BaseTable
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        public int StartNumber { get; set; }

        public int NextNumber { get; set; }

        public int MaxNumber { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime EffectiveTo { get; set; }

        public ICollection<DocumentNumber> DocumentNumbers { get; set; }

        public DocumentSequence()
        {
            Id = Guid.NewGuid().ToString();
        }

    }
}