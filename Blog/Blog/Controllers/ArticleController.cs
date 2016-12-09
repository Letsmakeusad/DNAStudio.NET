using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult List()
        {
            using (var dataBase = new BlogDbContext())
            {

                var articles = dataBase.Articles
                    .Include(a => a.Author)
                    .ToList();

                return View(articles);
            }
           


        }

        public ActionResult Details(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var dataBase = new BlogDbContext())
            {
                var article = dataBase.Articles.Where(a => a.Id == id)
               .Include(a => a.Author).First();

                if(article == null)
                {
                    return HttpNotFound();
                }

                return View(article);

            }

        }

        [HttpGet]
        [Authorize]     
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article article)
        {
            if (ModelState.IsValid)
            {
                using(var database = new BlogDbContext())
                {
                    // Gets Author Id
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First().Id;

                    //Set articles author
                    article.AuthorId = authorId;

                    //Save article to DB
                    database.Articles.Add(article);
                    database.SaveChanges();

                    return RedirectToAction("List");
                }
            }

            return View(article);
        }


        [HttpGet]
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var article = db.Articles.FirstOrDefault(a => a.Id == id);

                if(!this.IsAuthorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                if (article == null)
                {
                    // validate
                }

                return View(article);
            }

        }

        private bool IsAuthorizedToEdit(Article article)
        {
            bool isAuthor = article.isUserAuthor(User.Identity.Name);
            bool isAdmin = User.IsInRole("Admin");

            return isAdmin || isAuthor;
        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit(Article article)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    // the article we received was changed //
                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("List");

                }
            }

            return View(article);
        }


        [Authorize]
        public ActionResult Delete(int? id)
        {
            using (var db = new BlogDbContext())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var article = db.Articles.FirstOrDefault(a => a.Id == id);

                if (article == null)
                {
                    return HttpNotFound();
                }

                return View(article);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var db = new BlogDbContext())
            { 
                if(id== null)
                {
                    // validate
                }
                var article = db.Articles.FirstOrDefault(a => a.Id == id);

                if (article == null)
                {
                    // validate
                }

                db.Articles.Remove(article);
                db.SaveChanges();

                return RedirectToAction("List");
               
            }
        }
    }
}