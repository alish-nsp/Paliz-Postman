using Newtonsoft.Json;
using PostManPaliz.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PostManPaliz.Controllers
{
    public class PostManController : Controller
    {
        static string _token;


        public ActionResult Index()
        {

            return View();
        }

        public JsonResult CallApi(string url, string user, string pass, int method, string body)
        {
            try
            {
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(body, (typeof(DataTable)));
                var client = new RestClient(url);
                _token = getToket("http://" + client.BaseUrl.Host + ":" + client.BaseUrl.Port + "/token", user, pass);
                var r = new RestRequest();
                if (method == 1)
                {
                    r.Method = Method.GET;
                }
                else if (method == 2)
                {
                    r.Method = Method.POST;
                }

                r.RequestFormat = DataFormat.Json;
                r.AddHeader("Authorization", _token);
                if (dt.Rows.Count != 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        r.AddParameter(dt.Rows[i].ItemArray[0].ToString(), dt.Rows[i].ItemArray[1].ToString());
                    }
                }
                IRestResponse res = client.Execute(r);
                var result = new
                {
                    Code = res.StatusCode,
                    Message = res.Content
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    Code = 0,
                    Message = ex.Message
                };
                return Json(result);
            }
        }


        public string getToket(string url, string user, string pass)
        {
            var client = new RestClient(url);
            var r = new RestRequest(Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddParameter("username", user);
            r.AddParameter("password", pass);
            r.AddParameter("grant_type", "password");
            IRestResponse res = client.Execute(r);
            if (!(res.IsSuccessful))
            {
                return (res.Content);
            }
            else
            {
                Token a = JsonConvert.DeserializeObject<Token>(res.Content);
                return (a.token_type + " " + a.access_token);
            }
        }


    }
}