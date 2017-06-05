using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using PortalSlubny.Models;
using PortalSlubny.ViewModels;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PortalSlubny.Controllers
{
    public class CompanyController : Controller
    {
        private const string RELATIVECOMPANYIMAGEPATH = "Images\\CompanyImages";
        private static HttpPostedFileBase tempLogo;
        private ApplicationUserManager _userManager;

        public CompanyController()
        {
            Repository = ReposioryFactory.GetRepository();
        }

        ~CompanyController()
        {
            Repository.Dispose();
        }

        #region Properties

        public IRepository Repository { get; set; }

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

        #endregion

        #region Controllers

        public ActionResult Index(int? Categories, string Search, string Places, string Sort, int? page)
        {
            List<Company> companies = Repository.GetCompanies(Search, Categories, Places);
            companies = SortCompanies(companies, Sort);

            int pageNumber = (page ?? 1);

            CompanyIndexViewModel companyIndexVm = new CompanyIndexViewModel()
            {
                Categories = new SelectList(Repository.GetCategories(), "Id", "Name", Categories),
                Places = new SelectList(GetUsedCities(), Places),
                Search = Search,
                Sort = Sort,
                CurrentPlace = Places,
                CurrentCategories = Categories,
                Companies = companies.ToPagedList(pageNumber, 12),
                SortList = new SelectList(GetSortList(), "Value", "Text", Sort)
            };

            return View(companyIndexVm);
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.Categories = new SelectList(Repository.GetCategories(), "Id", "Name");
            ViewBag.Cities = new SelectList(Repository.GetCities(), "Name", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Add(Company company, FormCollection formCollection, HttpPostedFileBase logo)
        {
            ViewBag.Categories = new SelectList(Repository.GetCategories(), "Id", "Name");
            ViewBag.Cities = new SelectList(Repository.GetCities(), "Name", "Name");

            if (ModelState.IsValid)
            {
                company.Category = Repository.GetCategory(int.Parse(formCollection["CategoryId"]));
                company.Owner = Repository.GetUserById(User.Identity.GetUserId());

                if (logo == null && tempLogo != null)
                {
                    logo = tempLogo;
                }

                Image logoImg = SaveLogo(company, logo);

                int companyId = Repository.InsertCompany(company);

                return RedirectToAction("ShowCompany", "Company", new { id = companyId });
            }
            else
            {
                SetTempLogo(logo);
                return View(company);
            }
        }

        public ActionResult SaveUploadedFile(string companyName)
        {
            Directory.CreateDirectory(GetAbsoluteImagesPath());
            bool isSavedSuccessfully = true;
            string fName = "";

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                fName = file.FileName;

                if (file != null && file.ContentLength > 0)
                {
                    string imageId = GetImageId();
                    string path = Path.Combine(GetAbsoluteImagesPath(), imageId) + Path.GetExtension(file.FileName);

                    file.SaveAs(path);

                    Image image = new Image()
                    {
                        Id = imageId,
                        Path = Path.Combine(GetRelativeImagesPath(), imageId) + Path.GetExtension(file.FileName),
                        IsLogo = false,
                        Title = Path.GetFileNameWithoutExtension(file.FileName)
                    };

                    Company company = Repository.GetCompany(companyName);
                    company.Images.Add(image);
                    Repository.UpdateCompany(company);
                }
            }

            if (isSavedSuccessfully)
            {
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        public ActionResult ShowCompany(int id)
        {
            ShowCompanyViewModel showCompanyVM = GetShowCompanyVM(id);
            return View(showCompanyVM);
        }

        public ActionResult GetAttachments(string companyName)
        {
            Company company = Repository.GetCompany(companyName);

            var images = new List<Attachment>();

            foreach (var item in company.Images.Where(x=>!x.IsLogo))
            {
                DirectoryInfo infos = new DirectoryInfo(GetAbsoluteImagesPath());
                var fileInfo = infos.GetFiles().FirstOrDefault(x => x.Name == Path.GetFileName(item.Path));
                var attachment = new Attachment(item.Id, item.Title, item.Path, fileInfo.Length);
                images.Add(attachment);
            }

            return Json(new { Data = images }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteFile(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                try
                {
                    Image image = Repository.GetImage(fileName);

                    string path = Path.Combine(GetAbsoluteImagesPath(), Path.GetFileName(image.Path));
                    DeleteFileFromDisk(path);

                    Repository.DeleteImage(image);
                }
                catch (Exception ex)
                {
                    return Json(new { Message = ex.Message });
                }
                return Json(new { Message = string.Format("File deleted: {0}", fileName) });
            }
            return Json(new { Message = "Wrong arguments" });
        }

        [Authorize]
        public ActionResult EditCompany(int id)
        {
            Company company = Repository.GetCompany(id);

            if (!IsOwnerOrAdmin(company))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.LogoFileName = Path.GetFileName(company.Logo);
            ViewBag.Categories = new SelectList(Repository.GetCategories(), "Id", "Name");
            ViewBag.Cities = new SelectList(Repository.GetCities(), "Name", "Name");

            return View(company);
        }

        [HttpPost]
        [ActionName("EditCompany")]
        [Authorize]
        public ActionResult EditCompanyPost(Company company, FormCollection collection, HttpPostedFileBase logo)
        {
            if (!IsOwnerOrAdmin(company))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.LogoFileName = Path.GetFileName(company.Logo);
            ViewBag.Categories = new SelectList(Repository.GetCategories(), "Id", "Name");
            ViewBag.Cities = new SelectList(Repository.GetCities(), "Name", "Name");

            if (!ModelState.IsValid)
            {
                return View(company);
            }

            if (logo != null)
            {
                Image img = Repository.GetImages(company.Id).First(x => x.IsLogo);

                string path = Path.Combine(GetAbsoluteImagesPath(), Path.GetFileName(img.Path));
                DeleteFileFromDisk(path);

                string logoPath = SaveImageOnDisk(img.Id + Path.GetExtension(logo.FileName), logo);

                img.Path = Path.Combine(GetRelativeImagesPath(), Path.GetFileName(logoPath)).Replace('\\', '/');
                img.Title = Path.GetFileNameWithoutExtension(logo.FileName);

                Repository.UpdateImage(img);

                company.Logo = img.Path;
            }
            Repository.UpdateCompany(company);

            return RedirectToAction("ShowCompany", new { id = company.Id });
        }

        [HttpPost]
        public JsonResult CanSetCompanyName(string name, string initialName)
        {
            if (name == initialName)
            {
                return Json(true);
            }
            return Json(!Repository.IsCompanyExist(name));
        }

        public ActionResult SetGrade(string companyId, string value)
        {
            //użytkownik niezarejestrowany nie może oceniać,
            //jeżeli user już ocenił daną firmę to ocena jest aktualizowana i obliczana ocena ogólna i wstawiana do danej firmy
            //jeżeli użytkownik nie oceniał firmy to dodawana jest nowa ocena i obliczana ocena ogólna i wstawiana do danej firmy

            ApplicationUser currentUser = Repository.GetUserById(User.Identity.GetUserId());
            Company company = Repository.GetCompany(int.Parse(companyId));

            if (currentUser != null && company != null)
            {
                double val = double.Parse(value.Replace('.', ','));
                if (Repository.IsRated(company.Id, currentUser.Id))
                {
                    Repository.UpdateGrade(company.Id, currentUser.Id, val);
                }
                else
                {
                    Grade grade = new Grade(val, currentUser, company);
                    Repository.AddGrade(grade);
                }
                double actualGrade = Repository.GetGrade(company.Id);
                double userGrade = Repository.GetGrade(company.Id, currentUser.Id);
                ViewBag.UserGrade = userGrade;
                return Json(new { ActualGrade = actualGrade }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Message = "Zaloguj się, aby móc oceniać" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddComment([Bind(Include = "NewComment")]ShowCompanyViewModel model, FormCollection collection)
        {
            int companyId = int.Parse(collection["Company.Id"]);

            RecaptchaVerificationResult captchaResult = ValidateCaptcha();
            if (captchaResult != RecaptchaVerificationResult.Success)
            {
                GetCaptchaError(captchaResult);
                var newModel = GetShowCompanyVM(companyId);
                newModel.ScrollDown = true;
                return View("ShowCompany", newModel);
            }

            if (ModelState.IsValid)
            {
                Comment newComment = model.NewComment;
                newComment.CompanyId = companyId;
                newComment.Date = DateTime.Now;

                Repository.InsertComment(newComment);

                return RedirectToAction("ShowCompany", "Company", new { id = companyId });
            }
            else
            {
                var newModel = GetShowCompanyVM(companyId);
                newModel.ScrollDown = true;
                return View("ShowCompany", newModel);
            }
        }

        #endregion

        #region Methods

        private ICollection<SelectListItem> GetSortList()
        {
            List<SelectListItem> sortList = new List<SelectListItem>();
            sortList.Add(new SelectListItem() { Text = "Nazwa", Value = "Name" });
            sortList.Add(new SelectListItem() { Text = "Nazwa malejąco", Value = "NameDesc" });
            sortList.Add(new SelectListItem() { Text = "Miejscowość", Value = "Place" });
            sortList.Add(new SelectListItem() { Text = "Miejscowość malejąco", Value = "PlaceDesc" });
            sortList.Add(new SelectListItem() { Text = "Ocena", Value = "Rate" });
            sortList.Add(new SelectListItem() { Text = "Ocena malejąco", Value = "RateDesc" });

            return sortList;
        }

        private List<Company> SortCompanies(List<Company> companies, string Sort)
        {
            if (Sort == "Name")
            {
                return companies.OrderBy(x => x.Name).ToList();
            }
            else if (Sort == "NameDesc")
            {
                return companies.OrderByDescending(x => x.Name).ToList();
            }
            else if (Sort == "Place")
            {
                return companies.OrderBy(x => x.Place).ToList();
            }
            else if (Sort == "PlaceDesc")
            {
                return companies.OrderByDescending(x => x.Place).ToList();
            }
            else if (Sort == "Rate")
            {
                return companies.OrderBy(x => x.Grade).ToList();
            }
            else if (Sort == "RateDesc")
            {
                return companies.OrderByDescending(x => x.Grade).ToList();
            }
            else
            {
                return companies.OrderBy(x => x.Name).ToList();
            }
        }

        private List<string> GetUsedCities()
        {
            List<string> cities = new List<string>();
            foreach (var city in Repository.GetUsedCities())
            {
                cities.Add(city.Name);
            }
            return cities;
        }

        private void SetTempLogo(HttpPostedFileBase logo)
        {
            if (logo != null)
            {
                tempLogo = logo;
                ViewBag.LogoFileName = logo.FileName;
            }
            else
            {
                ViewBag.LogoFileName = tempLogo.FileName;
            }
        }

        private Image GetDefaultLogo(int categoryId)
        {
            // TODO
            string path = Path.Combine("/Images/DefaultLogos/cake.png");
            return new Image() { Id = GetImageId(), Title = "Cake", IsLogo = true, Path = path };
        }

        private bool IsOwnerOrAdmin(Company company)
        {
            if (User.IsInRole("Admin") || User.Identity.GetUserId() == company.Owner.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GetCaptchaError(RecaptchaVerificationResult captchaResult)
        {
            if (captchaResult == RecaptchaVerificationResult.NullOrEmptyCaptchaSolution)
            {
                ModelState.AddModelError("Recaptcha", "Kod captcha nie może być pusty");
            }
            else
            {
                ModelState.AddModelError("Recaptcha", "Niepoprawny kod captcha.");
            }
        }

        private RecaptchaVerificationResult ValidateCaptcha()
        {
            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();

            if (String.IsNullOrEmpty(recaptchaHelper.Response))
            {
                return RecaptchaVerificationResult.NullOrEmptyCaptchaSolution;
            }

            RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                return RecaptchaVerificationResult.IncorrectCaptchaSolution;
            }

            return RecaptchaVerificationResult.Success;
        }

        private ShowCompanyViewModel GetShowCompanyVM(int companyId)
        {
            ShowCompanyViewModel model = new ShowCompanyViewModel();
            model.Company = Repository.GetCompany(companyId);
            //model.Images = GetImagesForCompany(model.Company);
            model.UserGrade = Repository.GetGrade(companyId, User.Identity.GetUserId());
            if (User.Identity.IsAuthenticated)
            {
                model.NewComment = new Models.Comment()
                {
                    AuthorEmail = UserManager.FindById(User.Identity.GetUserId()).Email,
                    UserName = User.Identity.Name
                };
            }
            return model;
        }

        private Image SaveLogo(Company company, HttpPostedFileBase logo)
        {
            Image image;
            if (logo == null)
            {
                image = GetDefaultLogo(company.CategoryId);
            }
            else
            {
                string logoId = GetImageId();
                string logoPath = SaveImageOnDisk(logoId + Path.GetExtension(logo.FileName), logo);

                image = new Image()
                {
                    Id = logoId,
                    Path = Path.Combine(GetRelativeImagesPath(), Path.GetFileName(logoPath)).Replace('\\', '/'),
                    Title = Path.GetFileNameWithoutExtension(logo.FileName),
                    IsLogo = true
                };
            }

            company.Logo = image.Path;
            company.Images.Add(image);
            return image;
        }

        private string GetImageId()
        {
            return Guid.NewGuid().ToString();
        }

        private string SaveImageOnDisk(string fileName, HttpPostedFileBase logo)
        {
            string imagesDirPath = GetAbsoluteImagesPath();
            Directory.CreateDirectory(imagesDirPath);
            string resultPath = Path.Combine(imagesDirPath, fileName);
            logo.SaveAs(resultPath);
            return resultPath;
        }

        private static void DeleteFileFromDisk(string path)
        {
            if (System.IO.File.Exists(path) && !path.Contains("DefaultLogos"))
            {
                System.IO.File.Delete(path);
            }
        }

        private string GetAbsoluteImagesPath()
        {
            return Path.Combine(Server.MapPath(@"\"), RELATIVECOMPANYIMAGEPATH);
        }

        private string GetRelativeImagesPath()
        {
            return Path.Combine("/", RELATIVECOMPANYIMAGEPATH).Replace('\\', '/');
        }

        #endregion        
    }
}