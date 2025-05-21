using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MEGATECH.Models;
using MEGATECH.Models.EF;
using Newtonsoft.Json;

namespace MEGATECH.Controllers
{
    public class HomeController : Controller
    {
        private MEGATECHDBContext db = new MEGATECHDBContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Refresh()
        {
            var item = new ThongKeModel();

            ViewBag.Visitors_online = HttpContext.Application["visitors_online"];
            var hn = HttpContext.Application["HomNay"];
            item.HomNay = HttpContext.Application["HomNay"].ToString();
            item.HomQua = HttpContext.Application["HomQua"].ToString();
            item.TuanNay = HttpContext.Application["TuanNay"].ToString();
            item.TuanTruoc = HttpContext.Application["TuanTruoc"].ToString();
            item.ThangNay = HttpContext.Application["ThangNay"].ToString();
            item.ThangTruoc = HttpContext.Application["ThangTruoc"].ToString();
            item.TatCa = HttpContext.Application["TatCa"].ToString();
            return PartialView(item);
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SendMessage(string message)
        {
            try
            {
                string apiKey = System.Configuration.ConfigurationManager.AppSettings["ChatbotKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new Exception("API key rỗng hoặc chưa được cấu hình trong Web.config.");
                }

                var client = new HttpClient();

                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.TryAddWithoutValidation("HTTP-Referer", "https://localhost");
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-Title", "AI Sales Bot");

                var requestBody = new
                {
                    model = "deepseek/deepseek-chat-v3-0324:free",
                    messages = new[]
                    {
                new { role = "system", content = "Bạn là nhân viên tư vấn bán hàng công nghệ." },
                new { role = "user", content = message }
            }
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"OpenAI API lỗi: {response.StatusCode} - {responseString}");
                }

                dynamic result = JsonConvert.DeserializeObject(responseString);
                string reply = result.choices[0].message.content;

                return Json(new { reply = reply }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { reply = "Lỗi server: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}