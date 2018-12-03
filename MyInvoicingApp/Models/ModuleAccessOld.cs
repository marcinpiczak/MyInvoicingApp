using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyInvoicingApp.Models
{
    public class ModuleAccessOld : BaseTable
    {
        [Required]
        public string RoleId { get; set; }

        [ForeignKey("RoleId")]
        public ApplicationRole Role { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public Controllers Module { get; set; }

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

        public ModuleAccessOld()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}