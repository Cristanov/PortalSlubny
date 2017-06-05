using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalSlubny.Models
{
    public class Category
    {
        public Category()
        {

        }

        public Category(string name)
        {
            this.Name = name;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name="Nazwa")]
        public string  Name { get; set; }
    }
}