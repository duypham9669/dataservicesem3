using NewsFeedVn.model_custom;
using NewsFeedVn.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace NewsFeedVn.service
{
    public class DataService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public List<Article> GetArticlesBySourceId(int SourceId)
        {
            Debug.WriteLine("Start get articles by sourceId: " + SourceId);
            List<Article> articles = db.Articles
                   .SqlQuery("Select * from Articles where sourceId= " + SourceId+ " and status = 2")
                   .ToList<Article>();
            return articles;
        }
        public List<Article> GetArticlesByCategoryId(int CategoryId)
        {
            Debug.WriteLine("Start get articles by CategoryId: " + CategoryId);
            List<Article> articles = db.Articles
                   .SqlQuery("Select * from Articles where CategoryID = " + CategoryId + " and status = 2")
                   .ToList<Article>();
            return articles;
        }
        public List<CategoryAndArticles> GetArticlesByCategory()
        {
            //lấy 10 articles của mỗi category
            Debug.WriteLine("Start get articles by Category");
            List<CategoryAndArticles> result = new List<CategoryAndArticles>();
            List<Category> Categories = db.Categories
                   .SqlQuery("Select * from Categories")
                   .ToList<Category>();
            foreach(Category category in Categories)
            {
                List<Article> articles = db.Articles
                   .SqlQuery("Select top 10 * from Articles where CategoryID = " + category.Id + " and status = 2 ")
                   .ToList<Article>();
                CategoryAndArticles data = new CategoryAndArticles()
                {
                    CategoryName = category.Name,
                    Articles = articles
                };
                result.Add(data);
            }
            return result;
        }
    }
}