﻿using System.ComponentModel.DataAnnotations;

namespace MyInvoicingApp.Models
{
    public class ModuleAccess : BaseTable
    {
        [Required]
        public AccessorType AccessorType { get; set; }

        [Required]
        [MaxLength(450)]
        public string AccessorId { get; set; }

        [Required]
        public Controllers Module { get; set; }

        [Required]
        public bool Index { get; set; }

        [Required]
        public bool Add { get; set; }

        [Required]
        public bool Edit { get; set; }

        [Required]
        public bool Close { get; set; }

        [Required]
        public bool Open { get; set; }

        [Required]
        public bool Cancel { get; set; }

        [Required]
        public bool Send { get; set; }

        [Required]
        public bool Details { get; set; }

        [Required]
        public bool Approve { get; set; }

        [Required]
        public bool Remove { get; set; }
    }
}