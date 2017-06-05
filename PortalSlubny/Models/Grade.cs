using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalSlubny.Models
{
    public class Grade
    {
        public Grade()
        {
        }

        public Grade(double value, ApplicationUser user, Company company)
        {
            this.Value = value;
            this.User = user;
            this.Company = company;
        }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public double Value { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public Company Company { get; set; }
    }
}