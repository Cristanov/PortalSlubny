using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortalSlubny.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(string errorMessage, string redirectAction, string redirectControler)
        {
            ViewBag.ErrorMessage = errorMessage;
            ViewBag.RedirectAction = redirectAction;
            ViewBag.RedirectControler = redirectControler;
            return View();
        }
    }
}