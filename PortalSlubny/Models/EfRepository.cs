using PortalSlubny.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;

namespace PortalSlubny.Models
{
    public class EfRepository : IRepository
    {
        private EfDbContext DB;

        public EfRepository()
        {
            DB = new EfDbContext();
        }

        #region Companies

        public List<Company> GetCompanies()
        {
            return DB.Companies.ToList();
        }

        public List<Company> GetCompanies(string search, int? category, string place)
        {
            List<Company> companies = DB.Companies.ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                companies = (from c in companies
                             where c.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || !string.IsNullOrWhiteSpace(c.Describtion) && c.Describtion.Contains(search, StringComparison.OrdinalIgnoreCase)
                             select c).ToList();
            }
            if (category.HasValue)
            {
                companies = companies.Where(x => x.Category.Id == category).ToList();
            }
            if (!string.IsNullOrWhiteSpace(place))
            {
                companies = companies.Where(x => x.Place == place).ToList();
            }
            return companies;
        }

        public List<Company> GetLastAddedCompanies(int count)
        {
            List<Company> lastAddedCompanies = DB.Companies.OrderByDescending(x => x.CreationDate).Take(count).ToList();
            return lastAddedCompanies;
        }

        public Company GetCompany(int id)
        {
            return DB.Companies.Include(x => x.Images).FirstOrDefault(x => x.Id == id);
        }

        public Company GetCompany(string name)
        {
            return DB.Companies.Include(x => x.Images).FirstOrDefault(x => x.Name == name);
        }

        public int InsertCompany(Company company)
        {
            company.Describtion = WebUtility.HtmlDecode(company.Describtion);
            company.CreationDate = DateTime.Now;
            company = DB.Companies.Add(company);
            DB.SaveChanges();
            return company.Id;
        }

        public void UpdateCompany(Company company)
        {
            company.Describtion = WebUtility.HtmlDecode(company.Describtion);
            DB.Entry(company).State = EntityState.Modified;
            DB.SaveChanges();
        }

        public bool IsCompanyExist(string Name)
        {
            return DB.Companies.Any(x => x.Name == Name);
        }

        public void Detach(Company company)
        {
            DB.Entry(company).State = EntityState.Detached;
        }

        #endregion

        #region Categories

        public List<Category> GetCategories()
        {
            return DB.Categories.OrderBy(x => x.Name).ToList();
        }

        public Category GetCategory(int id)
        {
            return DB.Categories.FirstOrDefault(x => x.Id == id);
        }

        public Category GetCategory(string name)
        {
            return DB.Categories.FirstOrDefault(x => x.Name == name);
        }

        public Result DeleteCategory(int id)
        {
            Category category = DB.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return new Result("Kategoria o podanym identyfikatorze nie istnieje");
            }
            else
            {
                if (DB.Companies.Any(x => x.Category.Id == id))
                {
                    return new Result("Nie można usunąć kategorii, która jest w użyciu");
                }
                DB.Categories.Remove(category);
                DB.SaveChanges();
                return new Result();
            }
        }

        public bool IsCategoryExist(Category category)
        {
            return DB.Categories.Any(x => x.Name == category.Name);
        }

        public void EditCategory(Category category)
        {
            DB.Categories.FirstOrDefault(x => x.Id == category.Id).Name = category.Name;
            DB.SaveChanges();
        }

        public int InsertCategory(Category newCategory)
        {
            newCategory = DB.Categories.Add(newCategory);
            DB.SaveChanges();
            return newCategory.Id;
        }

        #endregion

        #region Cities

        public List<City> GetCities()
        {
            return DB.Cities.OrderBy(x => x.Name).ToList();
        }

        public List<City> GetUsedCities()
        {
            List<Company> companies = DB.Companies.ToList();
            List<City> cities = new List<City>();
            List<string> citiesNames = new List<string>();
            foreach (var company in companies)
            {
                citiesNames.Add(company.Place);
            }
            citiesNames = citiesNames.Distinct().ToList();
            foreach (var cityName in citiesNames)
            {
                cities.Add(DB.Cities.Single(x => x.Name == cityName));
            }
            return cities;
        }

        public City GetCityByName(string cityName)
        {
            return (from x in DB.Cities
                    where x.Name == cityName
                    select x).SingleOrDefault();
        }

        #endregion

        #region Users

        public ApplicationUser GetUserById(string id)
        {
            return DB.Users.FirstOrDefault(x => x.Id == id);
        }

        public ApplicationUser GetUserByName(string userName)
        {
            return DB.Users.FirstOrDefault(x => x.UserName == userName);
        }

        #endregion

        #region Grades

        public int AddGrade(Grade grade)
        {
            grade = DB.Grades.Add(grade);
            DB.SaveChanges();
            UpdateCompanyGrade(grade.Company.Id);
            return grade.Id;
        }

        public double GetGrade(int companyId)
        {
            return DB.Companies.FirstOrDefault(x => x.Id == companyId).Grade;
        }

        public double GetGrade(int companyId, string userId)
        {
            Grade grade = DB.Grades.FirstOrDefault(x => x.Company.Id == companyId && x.User.Id == userId);
            return (grade != null) ? grade.Value : 0;
        }

        public bool IsRated(int companyId, string userId)
        {
            return DB.Grades.Any(x => x.Company.Id == companyId && x.User.Id == userId);
        }

        public void UpdateGrade(int companyId, string userId, double newValue)
        {
            Grade grade = DB.Grades.FirstOrDefault(x => x.Company.Id == companyId && x.User.Id == userId);
            if (grade != null)
            {
                grade.Value = newValue;
                DB.SaveChanges();
                UpdateCompanyGrade(companyId);
            }
        }

        private void UpdateCompanyGrade(int companyId)
        {
            Company company = GetCompany(companyId);
            if (company != null)
            {
                List<Grade> grades = DB.Grades.Where(x => x.Company.Id == companyId).ToList();
                double avgGrade = grades.Sum(x => x.Value) / grades.Count;
                company.Grade = avgGrade;
                company.GradeCount = grades.Count;
                DB.SaveChanges();
            }
        }

        #endregion

        #region Comments

        public List<Comment> GetComments(int companyId)
        {
            return DB.Comments.Where(x => x.CompanyId == companyId).ToList();
        }

        public int InsertComment(Comment comment)
        {
            comment = DB.Comments.Add(comment);
            DB.SaveChanges();
            return comment.Id;
        }

        #endregion

        #region Images

        public void InsertImage(Image image)
        {
            DB.Images.Add(image);
            DB.SaveChanges();
        }

        public void UpdateImage(Image image)
        {
            DB.Entry(image).State = EntityState.Modified;
            DB.SaveChanges();
        }
        public Image GetImage(string id)
        {
            return DB.Images.FirstOrDefault(x => x.Id == id);
        }

        public List<Image> GetImages(int companyId)
        {
            return DB.Images.Where(x => x.CompanyId == companyId).ToList();
        }

        public void DeleteImage(Image image)
        {
            DB.Images.Remove(image);
            DB.SaveChanges();
        }

        #endregion

        public void Dispose()
        {
            if (DB != null)
            {
                this.Dispose();
            }
        }






    }
}