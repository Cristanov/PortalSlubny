using PortalSlubny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalSlubny.ViewModels
{
    public class ShowCompanyViewModel
    {
        public Company Company { get; set; }

        public ICollection<string> Images { get; set; }

        public double UserGrade { get; set; }

        public Comment NewComment { get; set; }

        public bool ScrollDown { get; set; }
    }
}