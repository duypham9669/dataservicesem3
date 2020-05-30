using NewsFeedVn.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Fizzler.Systems.HtmlAgilityPack;
using static NewsFeedVn.Models.Source;
using static NewsFeedVn.Models.Article;
using System.Collections;

namespace NewsFeedVn.service
{
    public class Boot1 : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                getData();
            });
            return task;
        }
        public void getData()
        {
            var task = Task.Run(() =>
            {
                Debug.WriteLine("start get api");
                try
                {
                    List<Source> sources = db.Sources.ToList();
                    for (int i = 0; i < sources.Count; i++)
                    {
                        Debug.WriteLine(sources[i].Status);
                        if (sources[i].Status.ToString().Equals("ACTIVE"))
                        {
                            Debug.WriteLine("url active");
                            var web = new HtmlAgilityPack.HtmlWeb();
                            var document = web.Load(sources[i].Domain + sources[i].Path);
                            var page = document.DocumentNode;

                            foreach (var item in page.QuerySelectorAll(sources[i].LinkSelector))
                            {
                                var url = item.GetAttributeValue("href", "");
                                Debug.WriteLine(url);
                                //Id,CategoryID,SourceId,Title,Content,Status,Url,CreatedAt,EditedAt,DeletedAt
                                Article article = new Article()
                                {
                                    CreatedAt = DateTime.Now,
                                    SourceId = sources[i].Id,
                                    CategoryID = 1,
                                    Url = url,
                                    Status = ArticleStatus.DEACTIVE
                                };
                                db.Articles.Add(article);
                                db.SaveChanges();
                            }
                        }
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }
        public Article ReviewData(Source source)
        {
            Debug.WriteLine("Start get data from selecter");
            var web = new HtmlAgilityPack.HtmlWeb();
            var document = web.Load(source.Domain + source.Path);
            var page = document.DocumentNode;
            Article article = new Article();
            foreach (var item in page.QuerySelectorAll(source.LinkSelector))
            {
                var url = item.GetAttributeValue("href", "");
                Debug.WriteLine(url);
                var document2 = web.Load(url);
                var page2 = document2.DocumentNode;
                //Id,CategoryID,SourceId,Title,Content,Status,Url,CreatedAt,EditedAt,DeletedAt
                try
                {
                    String title = page2.QuerySelector(source.TitleSelector).InnerText;
                    String content = page2.QuerySelector(source.ContentSelector).InnerHtml;

                    //List<string> removalTest = new ArrayList
                    
                    
                      var nodes  = page2.QuerySelector(source.RemovalSelector);
                    String descriptionSelector = page2.QuerySelector(source.DescriptionSelector).InnerText;
                    if (title != null && title != "" &&
                        content != null && content != "")
                    {
                        Debug.WriteLine(title);
                        Debug.WriteLine(content);
                        article.Title = title;
                        article.Content = content;
                        article.EditedAt = DateTime.Now;
                        article.Description = descriptionSelector;
                        article.Url = url;
                        }
                    return article;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Can't not get detail data from ArtiURL");
                    Debug.WriteLine(ex.Message);
                }
            }
            return null;
        }
    }
}   