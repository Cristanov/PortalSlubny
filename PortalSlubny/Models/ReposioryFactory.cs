using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalSlubny.Models
{
    public class ReposioryFactory
    {
        public static IRepository GetRepository()
        {
            return new EfRepository();
        }
    }
}