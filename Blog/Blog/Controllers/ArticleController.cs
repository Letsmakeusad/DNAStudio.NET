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

            return RedirectToRoute("List");
        }

        public ActionResult List()
        {
            using (var dataBase = new BlogDbContext())
            {

                var articles = dataBase.Articles
                    .Include(a => a.Author)
                    .Include(a => a.Tags)
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
               .Include(a => a.Author).Include(a=> a.Tags).First();

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
            using (var db = new BlogDbContext())
            {
                
                var model = new ArticleViewModel();
                model.Categories = db.Categories.ToList();

                return View(model);
            }

               
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArticleViewModel model)
        {
          
                using(var database = new BlogDbContext())
                {
                // Gets Author Id
                var user = database.Users.FirstOrDefault(u => u.UserName.Equals(this.User.Identity.Name));

                // passing the data to the View
                var article = new Article(user.Id, model.Title, model.Content, model.CategoryId);

                this.SetArticleTags(article, model, database); 
                   
                    //Save article to DB
                    database.Articles.Add(article);
                    database.SaveChanges();

                    return RedirectToAction("List");
                }
            
        }

        public void SetArticleTags(Article article, ArticleViewModel model, BlogDbContext database)
        {
            var tagsStrings = model.Tags
                .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct();


            article.Tags.Clear();

            foreach (var tagString in tagsStrings)
            {
                Tag tag = database.Tags.FirstOrDefault(t => t.Name.Equals(tagString));

                if(tag == null)
                {
                    tag = new Tag() { Name = tagString };
                    database.Tags.Add(tag);
                }

                article.Tags.Add(tag);
            }
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
                    return HttpNotFound();
                }

                var model = new ArticleViewModel();
                model.AuthorId = article.AuthorId;
                model.Title = article.Title;
                model.Content = article.Content;
                model.CategoryId = article.CategoryId;
                model.Categories = db.Categories.ToList();
                model.Tags = string.Join(",", article.Tags.Select(t => t.Name));

                return View(model);
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
        public ActionResult Edit(int? id,ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new BlogDbContext())
                {
                    var article = db.Articles.FirstOrDefault(a => a.Id == id);

                    article.Title = model.Title;
                    article.Content = model.Content;
                    article.CategoryId = model.CategoryId;
                    this.SetArticleTags(article, model, db);


                    // the article we received was changed //
                    db.Entry(article).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("List");

                }
            }

            return View(model);
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

                var article = db.Articles.Include(a=> a.Category).Include(a=> a.Tags).FirstOrDefault(a => a.Id == id);

                ViewBag.Tags = string.Join(", ", article.Tags.Select(t => t.Name));

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