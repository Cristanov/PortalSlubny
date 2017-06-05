using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalSlubny.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Autor")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string AuthorEmail { get; set; }

        [Required]
        [Display(Name = "Treść komentarza")]
        public string Text { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }
    }
}