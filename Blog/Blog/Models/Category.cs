using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web;
using Blog.Models;

namespace Blog.Models
{
    public class Category
    {
        private ICollection<Article> articles;

        public Category()
        {
            this.Articles = new HashSet<Article>();
        }
        [Key]
        public int Id { get; set; }

        [Required]  
        public string Name { get; set;}

        public virtual ICollection<Article> Articles
        {
            get { return this.articles; }
            set { this.articles = value; }
        }
    }
}