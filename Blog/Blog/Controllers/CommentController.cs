using Blog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Blog.Controllers
{
    public class CommentController : Controller
    {

        private BlogDbContext dataBase = new BlogDbContext();

        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create(Article model)
        {
            using (var db = new BlogDbContext())
            {
                var comment = new Comment();
                comment.PostId = model.Id;

                return View(comment);
            }

               
        }

        [HttpPost]
        public ActionResult Create(Comment comment)
        {
            using (var db = new BlogDbContext())
            {

                var opinion = new Comment(comment.Id, comment.Content);

                db.Comments.Add(opinion);         
                db.SaveChanges();
               
                return View(opinion);
            }
                
        }

 
        public ActionResult List()
        {
            using(var db = new BlogDbContext())
            {
                var comments = db.Comments.ToList();

                return View(comments);
            }
        }

    }
}