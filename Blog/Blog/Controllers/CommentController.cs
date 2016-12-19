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

                return RedirectToAction("Details", "Article", new { @id = comment.Id });
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

        [HttpGet]
        public ActionResult Delete(Comment index)
        {
      
                return View(index);
       
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db = new BlogDbContext())
            {
                var comment = db.Comments.FirstOrDefault(a => a.Id == id );

                if(comment == null)
                {
                    return HttpNotFound();
                }

                db.Comments.Remove(comment);
                db.SaveChanges();

                return RedirectToAction("List", "Article");
            }
        }


    }


}