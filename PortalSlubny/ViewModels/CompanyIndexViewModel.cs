using PagedList;
using PortalSlubny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalSlubny.ViewModels
{
    public class CompanyIndexViewModel
    {
        public SelectList Categories { get; set; }

        public SelectList Places { get; set; }

        public string Search { get; set; }

        public string Sort { get; set; }

        public string CurrentPlace { get; set; }

        public int? CurrentCategories { get; set; }

        public IPagedList<Company> Companies { get; set; }

        public SelectList SortList { get; set; }
    }
}