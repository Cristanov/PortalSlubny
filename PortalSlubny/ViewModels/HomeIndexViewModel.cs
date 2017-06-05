using PortalSlubny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortalSlubny.ViewModels
{
    public class HomeIndexViewModel
    {
        public ICollection<Category> Categories { get; set; }

        public ICollection<Company> LastCompanies { get; set; }

        public Email Email { get; set; }
    }

    public class Email
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string From { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Text { get; set; }
    }
}