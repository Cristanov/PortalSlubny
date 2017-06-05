using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortalSlubny.Models;

namespace PortalSlubny.Controllers
{
    public class AdminController : Controller
    {
        public AdminController()
        {
            Repository = ReposioryFactory.GetRepository();
        }

        ~AdminController()
        {
            Repository.Dispose();
        }

        public IRepository Repository { get; set; }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Categories()
        {
            return View(Repository.GetCategories());
        }

        [HttpPost]
        [ActionName("Categories")]
        public ActionResult CategoriesAdd(FormCollection form)
        {
            string newCategoryName = form["newCategory"];
            if (string.IsNullOrWhiteSpace(newCategoryName))
            {
                ModelState.AddModelError("", "Nazwa kategorii jest wymagana");
                return View(Repository.GetCategories());
            }
            Category newCategory = new Category(newCategoryName);
            if (!Repository.IsCategoryExist(newCategory))
            {
                Repository.InsertCategory(newCategory);
                return View(Repository.GetCategories());
            }
            else
            {
                string errorMessage = string.Format("Kategoria o nazwie {0} już istnieje.", newCategoryName);
                return RedirectToAction("Index", "Error", new { errorMessage = errorMessage, redirectAction = "Categories", redirectControler = "Admin" });
            }
        }

        public ActionResult CategoryDelete(int id)
        {
            Result result;
            if ((result = Repository.DeleteCategory(id)).IsSuccess)
            {
                return RedirectToAction("Categories");
            }
            else
            {
                return RedirectToAction("Index", "Error", new { errorMessage = result.ErrorMessage, redirectAction = "Categories", redirectControler = "Admin" });
            }
        }


        public ActionResult CategoryEdit(int id)
        {
            Category category = Repository.GetCategory(id);

            return View(category);
        }

        [HttpPost]
        [ActionName("CategoryEdit")]
        public ActionResult CategoryEditPost(Category category)
        {
            if ((Repository.IsCategoryExist(category)))
            {
                string errorMessage = string.Format("Kategoria o nazwie {0} już istnieje", category.Name);
                return RedirectToAction("Index", "Error", new { errorMessage = errorMessage, redirectAction = "Categories", redirectControler = "Admin" });
            }
            else
            {
                Repository.EditCategory(category);
                return RedirectToAction("Categories");
            }
            //Category category = Repository.GetCategory(id);
            //Result result;
            //if (result = Repository.EditCategory(category))
            //{
            //    return Redirect("Categproes");
            //}
        }
    }
}