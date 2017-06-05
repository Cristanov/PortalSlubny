using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PortalSlubny.Models;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PortalSlubny.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<PortalSlubny.Models.EfDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PortalSlubny.Models.EfDbContext context)
        {
            SeedUsers(context);
            SeedCategories();
            SeedCities(context);
            //SeedCompanies(context);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }

        private void SeedCompanies(EfDbContext context)
        {
            string companyImagesPath = @"C:\Users\Crist_000\Documents\visual studio 2013\Projects\PortalSlubny\PortalSlubny\Images\CompanyImages";
            if (Directory.Exists(companyImagesPath))
            {
                foreach (var item in Directory.GetDirectories(companyImagesPath))
                {
                    Directory.Delete(item, true);
                }
            }
            IRepository repo = ReposioryFactory.GetRepository();
            Random r = new Random();
            string[] logos = Directory.GetFiles(@"C:\Users\Crist_000\Documents\visual studio 2013\Projects\PortalSlubny\PortalSlubny\logo example");

            for (int i = 0; i < 300; i++)
            {
                string name = "Firma" + i;
                string logoFile = logos[r.Next(logos.Length)];
                string dirPath = @"C:\Users\Crist_000\Documents\visual studio 2013\Projects\PortalSlubny\PortalSlubny\Images\CompanyImages\admin_" + name + @"\logo";
                Directory.CreateDirectory(dirPath);
                File.Copy(logoFile, dirPath + "\\" + Path.GetFileName(logoFile));
                foreach (var item in logos)
                {
                    File.Copy(item, @"C:\Users\Crist_000\Documents\visual studio 2013\Projects\PortalSlubny\PortalSlubny\Images\CompanyImages\admin_" + name + "\\" + Path.GetFileName(item), true);
                }
                Company newCompany = new Company()
                {
                    Category = repo.GetCategories()[r.Next(repo.GetCategories().Count)],
                    CreationDate = DateTime.Now,
                    Email = name + "@" + name + ".pl",
                    Name = name,
                    Owner = repo.GetUserByName("admin"),
                    Phone1 = "123123123",
                    Place = repo.GetCities()[r.Next(repo.GetCities().Count)].Name,
                    WWW = "www." + name + ".pl",
                    Logo = @"/Images/CompanyImages/admin_" + name + @"/logo/" + Path.GetFileName(logoFile)
                };
                repo.InsertCompany(newCompany);
            }
        }

        private void SeedCities(EfDbContext context)
        {
            string insertCitiesPath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin", string.Empty) + "\\App_Data"; ;
            context.Database.ExecuteSqlCommand(System.IO.File.ReadAllText(insertCitiesPath + "\\DeleteCities.sql"));
            context.Database.ExecuteSqlCommand(System.IO.File.ReadAllText(insertCitiesPath + "\\InsertCities.sql", Encoding.Default));
        }

        private static void SeedCategories()
        {
            Category cat = new Category() { Id = 1, Name = "Atrakcje weselne" };
            Category cat2 = new Category() { Id = 2, Name = "Bi¿uteria" };
            Category cat3 = new Category() { Id = 3, Name = "Dekoracje" };
            Category cat4 = new Category() { Id = 4, Name = "Fotografia œlubna" };
            Category cat5 = new Category() { Id = 5, Name = "Kwiaciarnie" };

            using (var ctx = new EfDbContext())
            {
                ctx.Categories.AddOrUpdate(cat);
                ctx.Categories.AddOrUpdate(cat2);
                ctx.Categories.AddOrUpdate(cat3);
                ctx.Categories.AddOrUpdate(cat4);
                ctx.Categories.AddOrUpdate(cat5);
                ctx.SaveChanges();
            }
        }

        private static void SeedUsers(EfDbContext context)
        {
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Admin" };

                manager.Create(role);
            }

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "admin" };

                manager.Create(user, "password");
                manager.AddToRole(user.Id, "Admin");
            }
        }
    }
}
