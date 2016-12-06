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
    }
}