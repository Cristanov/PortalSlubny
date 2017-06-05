using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortalSlubny.Models
{
    public class Image
    {
        public string Id { get; set; }

        public string Path { get; set; }

        public string Title { get; set; }

        public bool IsLogo { get; set; }

        public int CompanyId { get; set; }
    }
}