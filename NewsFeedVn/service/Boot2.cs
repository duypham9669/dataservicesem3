using Fizzler.Systems.HtmlAgilityPack;
using NewsFeedVn.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static NewsFeedVn.Models.Article;

namespace NewsFeedVn.service
{
    public class Boot2 : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                GetDataDetail();

            });
            return task;
        }

        public void GetDataDetail()
        {
            Debug.WriteLine("start get Detail news");
            try
            {
                DateTime date_now = DateTime.Now;
                Debug.WriteLine("Get data from" + date_now.AddDays(-2).ToString("yyyy/MM/dd"));

                List<Article> articles = db.Articles
                    .SqlQuery("Select * from Articles where CreatedAt > " + date_now.AddDays(-2).ToString("yyyy/MM/dd"))
                    .ToList<Article>();

                //List<Article> articles = db.Articles.ToList();
                Debug.WriteLine("Start get url");
                for (int i = 0; i < articles.Count; i++)
                {
                    Debug.WriteLine(articles[i].Status);
                    if (articles[i].Status.ToString().Equals("DEACTIVE"))
                    {
                        var web = new HtmlAgilityPack.HtmlWeb();
                        var document = web.Load(articles[i].Url);
                        var page = document.DocumentNode;

                        Source source = db.Sources.Find(articles[i].SourceId);
                        Debug.WriteLine(source.TitleSelector);
                        try
                        {
                            String title = page.QuerySelector(source.TitleSelector).InnerText;
                            String content = page.QuerySelector(source.ContentSelector).InnerText;
                            if (title != null && title != "" &&
                                content != null && content != "")
                            {
                                Debug.WriteLine(title);
                                Debug.WriteLine(content);
                                articles[i].Title = title;
                                articles[i].Content = content;
                                articles[i].Status = ArticleStatus.ACTIVE;
                                articles[i].EditedAt = DateTime.Now;
                            }
                            else
                            {
                                articles[i].Status = ArticleStatus.DEACTIVE;
                            }
                        }catch(Exception ex)
                        {
                            Debug.WriteLine("Not get detail data from ArticlesId: " + articles[i].Id);
                            Debug.WriteLine(ex.Message);
                        }
                        
                        db.Entry(articles[i]).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                            Debug.WriteLine("Get data from url: " + document + " done");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("ERROR Update Articles. \n"+ex.Message);
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}