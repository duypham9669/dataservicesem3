using NewsFeedVn.model_custom;
using NewsFeedVn.Models;
using NewsFeedVn.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NewsFeedVn.Controllers
{
    public class ServiceController : ApiController
    {
        // GET: api/StartGetUrl
        [Route("api/Service/StartDataDetail")]
        [HttpGet]
        public IHttpActionResult StartDataDetail()
        {
            Boot2 bot2_service = new Boot2();
            try
            {
                bot2_service.GetDataDetail();
                return Ok();
            }
            catch (Exception ex)
            {
                return Exception(ex.Message);
            }

        }

        private IHttpActionResult Exception(string message)
        {
            throw new NotImplementedException();
        }
        [Route("api/Service/StartGetUrl")]
        [HttpGet]
        public IHttpActionResult StartGetUrl()
        {
            Boot1 bot1_service = new Boot1();
            try
            {
                bot1_service.getData();
                return Ok();
            }
            catch (Exception ex)
            {
                return Exception(ex.Message);
            }
        }
        [Route("api/Service/reviewData")]
        [HttpPost]
        public IHttpActionResult RevirewData(Source source)
        {
            Boot1 bot1_serrvice = new Boot1();
            try
            {
                Article article= bot1_serrvice.ReviewData(source);
                return Ok(article);
            }
            catch (Exception ex)
            {
                return Exception(ex.Message);
            }
        }
        [Route("api/Service/reviewurl")]
        [HttpPost]
        public IHttpActionResult RevirewUrl(Source source)
        {
            Boot1 bot1_service = new Boot1();
            try
            {
                List<String>result = bot1_service.ReviewUrl(source);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Exception(ex.Message);
            }
        }
        [Route("api/Service/GetArticlesBySourceId/{id}")]
        [HttpGet]
        public IHttpActionResult GetArticlesBySourceId(int id)
        {
            DataService Service = new DataService();
            try
            {
                List<Article> result = Service.GetArticlesBySourceId(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Exception(ex.Message);
            }
        }
        [Route("api/Service/GetArticlesByCategoryId/{id}")]
        [HttpGet]
        public IHttpActionResult GetArticlesByCategoryId(int id)
        {
            DataService Service = new DataService();
            try
            {
                List<Article> result = Service.GetArticlesByCategoryId(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Exception(ex.Message);
            }
        }
        [Route("api/Service/GetArticlesByCategory")]
        [HttpGet]
        public IHttpActionResult GetArticlesByCategory()
        {
            DataService Service = new DataService();
            try
            {
                List<CategoryAndArticles> result = Service.GetArticlesByCategory();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Exception(ex.Message);
            }
        }
        [Route("api/Service/ReportBoot/{Start}/{End}")]
        [HttpGet]
        public IHttpActionResult ReportBoot(String Start, String End)
        {
            DataService Service = new DataService();
            try
            {
                ReportBoot result = Service.ReportBoot(Start,End);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Exception(ex.Message);
            }
        }
    }
}
