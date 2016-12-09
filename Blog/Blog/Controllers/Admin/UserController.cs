using Blog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            using (var db = new BlogDbContext())
            {
                var users = db.Users.ToList();

                ViewBag.Admins = GetAdmins(db);

                return View(users);
            }


        }

        private HashSet<string> GetAdmins(BlogDbContext db)
        {
            var userManager = Request
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            var users = db.Users.ToList();

            var admins = new HashSet<string>();

            foreach (var user in users)
            {
                if (userManager.IsInRole(user.Id, "Admin"))
                {
                    admins.Add(user.Id);
                }
            }

            return admins;

        }

        //get
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id.Equals(id));

                var model = new UserViewModel();
                model.Email = user.Email;
                model.FullName = user.FullName;
                model.Roles = GetUserRoles(user, db);

                return View(model);

            }

        }

        private List<Role> GetUserRoles(ApplicationUser user, BlogDbContext db)
        {
            var rolesInDataBase = db.Roles
                .Select(r => r.Name)
                .OrderBy(r => r)
                .ToList();

            var userManager = Request
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>();


            List<Role> userRoles = new List<Role>();


            foreach (var roleName in rolesInDataBase)
            {
                Role role = new Role() { Name = roleName };
                if (userManager.IsInRole(user.Id, roleName))
                {
                    role.isSelected = true;
                }

                userRoles.Add(role);
            }

            return userRoles;

        }

        [HttpPost]
        public ActionResult Edit(string id, UserViewModel viewmodel)
        {
            //Check if model is valid
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContext())
                {
                    //Get user from database
                    var user = database.Users.FirstOrDefault(u => u.Id == id);

                    //Check if user exists
                    if(user == null)
                    {
                        return HttpNotFound();
                    }

                    //If password field is not empty, change password
                    if(!string.IsNullOrEmpty(viewmodel.Password))
                    {
                        var hasher = new PasswordHasher();
                        var passwordHash = hasher.HashPassword(viewmodel.Password);
                        user.PasswordHash = passwordHash;  
                    }

                    //Set user properties
                    user.Email = viewmodel.Email;
                    user.FullName = viewmodel.FullName;
                    user.UserName = viewmodel.Email;
                    this.SetUserRoles(viewmodel, user, database);

                    //save changes
                    database.Entry(user).State = EntityState.Modified;
                    database.SaveChanges();


                    return RedirectToAction("List");
                }
            }

            return View(viewmodel);
        }


        private void SetUserRoles(UserViewModel model, ApplicationUser user, BlogDbContext db)
        {
            var userManager = Request
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            foreach (var role in model.Roles)
            {
                if(role.isSelected)
                {
                    userManager.AddToRole(user.Id, role.Name);
                }
                else if(!role.isSelected)
                {
                    userManager.RemoveFromRole(user.Id, role.Name);
                }
            }
        }


        //
        //GET: User/Delete
        public ActionResult Delete(string id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContext())
            {
                // Get user from db
                var user = database.Users
                    .Where(u => u.Id.Equals(id))
                    .First();

                // Check if it exists
                if(user == null)
                {
                    return HttpNotFound();
                }

                return View(user);
            }
        }


        //
        //POS: User/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }

            using (var database = new BlogDbContext())
            {
                //Get user from db
                var user = database.Users
                    .Where(u => u.Id.Equals(id))
                    .First();

                //Get user articles from database
                var userArticles = database.Articles
                    .Where(a => a.Author.Id == user.Id);


                //Delete user articles
                foreach (var article in userArticles)
                {
                    database.Articles.Remove(article);
                }

                //Delete user and save changes 

                database.Users.Remove(user);
                database.SaveChanges();

                return RedirectToAction("List");
            }
        }
    }
}