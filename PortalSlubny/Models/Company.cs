using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalSlubny.Models
{
    public class Company
    {
        public Company()
        {
            Comments = new List<Comment>();
            Images = new List<Image>();
        }

        public Company(string name, string place)
        {
            this.Name = name;
            this.Place = place;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name="Nazwa")]
        [StringLength(250, ErrorMessage = "{0} musi mieć przynajmniej {2} znaki", MinimumLength = 3)]
        [Remote("CanSetCompanyName", "Company", HttpMethod = "Post", ErrorMessage = "Firma o takiej nazwie już istnieje", AdditionalFields = "InitialName")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Miejscowość")]
        public string Place { get; set; }

        [Phone]
        [Display(Name="Telefon 1")]
        public string  Phone1 { get; set; }

        [Phone]
        [Display(Name = "Telefon 2")]
        public string Phone2 { get; set; }

        [EmailAddress]
        [Display(Name="Email")]
        public string Email { get; set; }

        [Display(Name="Strona www")]
        public string WWW { get; set; }

        [Display(Name="Opis")]
        [AllowHtml]
        public string Describtion { get; set; }

        public string Logo { get; set; }

        public string Address { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public double Grade { get; set; }

        public int GradeCount { get; set; }

        [Display(Name = "Kategoria")]
        [Required]
        public int CategoryId { get; set; }

        [Display(Name="Kategoria")]
        public virtual Category Category { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public  virtual ICollection<Image> Images { get; set; }
    }
}