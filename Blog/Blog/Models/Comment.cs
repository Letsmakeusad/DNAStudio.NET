﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Comment
    {
       
        public Comment()
        {
          
        }

        public Comment(int postId, string content, string author)
        {
            
            this.Content = content;
            this.ArticleId = postId;
            this.CommentAuthor = author;
          
            
        }

        [Key]
        public int Id { get; set;}

        [MaxLength(300)]
        public string Content { get; set; }

        public string CommentAuthor { get; set; }

        public int PostId { get; set; }

        public virtual Article Article { get; set; }

        [ForeignKey("Article")]
        public int ArticleId { get; set; }

        

    }
}