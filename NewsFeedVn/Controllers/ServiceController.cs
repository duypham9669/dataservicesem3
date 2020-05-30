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
            Boot2 bot2_serrvice = new Boot2();
            try
            {
                bot2_serrvice.GetDataDetail();
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
            Boot1 bot1_serrvice = new Boot1();
            try
            {
                bot1_serrvice.getData();
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
    }
}
