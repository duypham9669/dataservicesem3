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
using System.Text.RegularExpressions;
using System.Text;

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
                    DateTime date_now = DateTime.Now;
                    List<Article> articles = db.Articles
                            .SqlQuery("Select * from Articles where CreatedAt > '" + date_now.AddDays(-4).ToString("yyyy/MM/dd")+" '")
                            .ToList<Article>();

                    List<Source> sources = db.Sources
                    .SqlQuery("Select * from Sources where status = 1")
                    .ToList();

                    for (int i = 0; i < sources.Count; i++)
                    {
                            var web = new HtmlAgilityPack.HtmlWeb();
                            var document = web.Load(sources[i].Domain + sources[i].Path);
                            var page = document.DocumentNode;

                            foreach (var item in page.QuerySelectorAll(sources[i].LinkSelector))
                            {
                                try
                                {
                                var Url = item.GetAttributeValue("href", "");
                                //check existed url -> not add to articles
                                if (CheckUrl(Url, articles))
                                {
                                    Article article = new Article()
                                    {
                                        CreatedAt = DateTime.Now,
                                        SourceId = sources[i].Id,
                                        CategoryID = sources[i].CategoryID ?? default(int),
                                        Url = Url,
                                        Status = ArticleStatus.INITIAL
                                    };
                                    db.Articles.Add(article);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    Debug.WriteLine("Url existed: "+Url);
                                }
                                
                            }
                            catch(Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                                
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            });
        }
        private Boolean CheckUrl(String Url, List<Article> ListArticles )
        {
            foreach( Article article in ListArticles)
            {
                if (article.Url.Equals(Url))
                {
                    return false;
                }
            }
            return true;
        }
        public List<String> ReviewUrl(Source source)
        {
            var web = new HtmlAgilityPack.HtmlWeb();
            var document = web.Load(source.Domain + source.Path);
            var page = document.DocumentNode;
            List<String> ListUrl = new List<string>();
            foreach (var item in page.QuerySelectorAll(source.LinkSelector))
            {
                try
                {
                    var url = item.GetAttributeValue("href", "");
                    Debug.WriteLine(url);
                   if (url!=null && url != "")
                    {
                        ListUrl.Add(url);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return ListUrl;
        }
        public Article ReviewData(Source source)
        {
            Debug.WriteLine("Start get data from selecter");
            var web = new HtmlAgilityPack.HtmlWeb();
            var document = web.Load(source.Domain + source.Path);
            var page = document.DocumentNode;
            Article article = new Article();
            var item = page.QuerySelector(source.LinkSelector);
               var url = item.GetAttributeValue("href", "");
                Debug.WriteLine(url);
            if (url.StartsWith("/"))
            {
                url = source.Domain + url;
            }
                var document2 = web.Load(url);
                var page2 = document2.DocumentNode;
                //Id,CategoryID,SourceId,Title,Content,Status,Url,CreatedAt,EditedAt,DeletedAt
                try
                {
                    String title = page2.QuerySelector(source.TitleSelector).InnerHtml;
                    String content = page2.QuerySelector(source.ContentSelector).InnerHtml;
                    String descriptionSelector = page2.QuerySelector(source.DescriptionSelector).InnerHtml;
                    string[] arrListStr = content.Split(new string[] { "<img" }, StringSplitOptions.None);
                string imgLink = "";
                for (int j = 1; j < arrListStr.Length; j++)
                {
                    string[] arrListStr2 = arrListStr[j].Split(new string[] { ">" }, StringSplitOptions.None);
                    String arrImg = arrListStr2[0];
                    String arrImg2 = arrListStr2[0].Substring(arrListStr2[0].IndexOf("data-src") + 5, arrImg.Length - arrListStr2[0].IndexOf("data-src") - 5);
                    arrListStr2[0] = "<br";
                    arrListStr[j] = "<img " + arrImg2 + ConvertStringArrayToStringImg(arrListStr2);
                    if (j == 1)
                    {
                        string[] src = arrImg2.Split(new string[] { "\"" }, StringSplitOptions.None);
                        imgLink = src[1];
                    }
                }

                //string[] arrListStr2 = arrListStr[1].Split(new string[] { ">" }, StringSplitOptions.None);
                //String arrImg = arrListStr2[0];
                //String arrImg2 = arrListStr2[0].Substring(arrListStr2[0].IndexOf("data-src") + 5, arrImg.Length - arrListStr2[0].IndexOf("data-src") - 5);
                //arrListStr[1] = "<img " + arrImg2;
                String ContentResult = ConvertStringArrayToString(arrListStr);
                Debug.WriteLine("ContenResult: "+ContentResult);
                if (title == null)
                {
                    Debug.WriteLine("title null");
                }
                if (ContentResult == null)
                {
                    Debug.WriteLine("content null");
                }
                if (content == null)
                {
                    Debug.WriteLine("content null");
                }
                var nodes = page2.QuerySelector(source.ContentSelector);
                var removedNode = nodes.QuerySelectorAll(source.RemovalSelector).ToList();
                foreach (var node in removedNode)
                {
                    node.Remove();
                }                
                    if (title != null && title != "" &&
                        content != null && content != ""&&
                        descriptionSelector!=null
                        )
                    {
                        article.Title = title;
                        article.Content = ContentResult;
                        article.EditedAt = DateTime.Now;
                        article.Img = imgLink;
                        article.Description = descriptionSelector;
                        article.Url = url;
                        }
                    return article;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Can't not get detail data from ArtiURL");
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            
            return null;
        }
        static string ConvertStringArrayToString(string[] array)
        {
            // Concatenate all the elements into a StringBuilder.
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
            }
            return builder.ToString();
        }
        static string ConvertStringArrayToStringImg(string[] array)
        {
            // Concatenate all the elements into a StringBuilder.
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append('>');
                builder.Append(value);
                
            }
            return builder.ToString();
        }
    }
}   