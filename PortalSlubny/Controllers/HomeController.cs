using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PortalSlubny.Models;
using PortalSlubny.ViewModels;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace PortalSlubny.Controllers
{
    public class HomeController : Controller
    {
        private IRepository Repository;

        public HomeController()
        {
            Repository = ReposioryFactory.GetRepository();
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            HomeIndexViewModel homeIndexVM = new HomeIndexViewModel();
            homeIndexVM.Categories = Repository.GetCategories().OrderBy(x => x.Name).ToList();
            homeIndexVM.LastCompanies = Repository.GetLastAddedCompanies(5).ToList();
            if (User.Identity.IsAuthenticated)
            {
                homeIndexVM.Email = new Email()
                {
                    UserName = User.Identity.Name,
                    From = UserManager.FindById(User.Identity.GetUserId()).Email
                };
            }
            return View(homeIndexVM);
        }

        public ActionResult SendMessage(Email email)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            MailMessage mail = new MailMessage();
            mail.To.Add("wlodarzmar@gmail.com");
            mail.From = new MailAddress(email.From);
            mail.Subject = "Góralski Ślub Mailer " + email.Subject;
            mail.Body = "<b>Góralski Ślub - wiadomość z formularza kontaktowego</b><hr /> " + email.Text;
            mail.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential()
                {
                    // TODO: zrobić coś z tym
                    UserName = "testWlodi@gmail.com",
                    Password = "dupaDUPA"
                };

                smtp.UseDefaultCredentials = false;

                smtp.Credentials = credential;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            return RedirectToAction("Index");
        }
    }
}