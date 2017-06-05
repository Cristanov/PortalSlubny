using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalSlubny.Models
{
    public interface IRepository : IDisposable
    {
        #region Companies

        int InsertCompany(Company company);
        List<Company> GetCompanies();
        List<Company> GetCompanies(string search, int? category, string place);
        List<Company> GetLastAddedCompanies(int p);
        Company GetCompany(int id);
        Company GetCompany(string name);
        void UpdateCompany(Company company);
        bool IsCompanyExist(string Name);

        void Detach(Company company);

        #endregion

        #region Categories

        List<Category> GetCategories();
        Category GetCategory(int id);
        Category GetCategory(string name);
        Result DeleteCategory(int id);
        bool IsCategoryExist(Category category);
        void EditCategory(Category category);
        int InsertCategory(Category newCategory);


        #endregion

        #region Cities

        List<City> GetCities();
        List<City> GetUsedCities();
        City GetCityByName(string Places);

        #endregion

        #region Users

        ApplicationUser GetUserById(string id);
        ApplicationUser GetUserByName(string userName);


        #endregion

        #region Grades

        int AddGrade(Grade grade);

        double GetGrade(int companyId);

        double GetGrade(int companyId, string userId);

        void UpdateGrade(int companyId, string userId, double newValue);


        bool IsRated(int companyId, string userId);
        #endregion

        #region Comments

        List<Comment> GetComments(int companyId);
        int InsertComment(Comment comment);

        #endregion

        #region Images
        void InsertImage(Image image);
        void UpdateImage(Image image);
        Image GetImage(string id);
        List<Image> GetImages(int companyId);
        void DeleteImage(Image image);

        #endregion



        
    }
}
