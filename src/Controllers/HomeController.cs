using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Take2.Source.Models;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Take2.Source.Controllers
{
    public class HomeController : Controller
    {
        private readonly Settings _settings;

        public HomeController(IOptionsSnapshot<Settings> settings)
        {
            _settings = settings.Value;
        }

        public IActionResult Index()
        {
            ViewData["BackgroundColor"] = _settings.BackgroundColor;
            ViewData["FontSize"] = _settings.FontSize;
            ViewData["FontColor"] = _settings.FontColor;
            ViewData["Message"] = _settings.Message;


            var json =  ListKeys().Result.Content.ReadAsStringAsync().Result;
            JObject o = JObject.Parse(json);
            ViewData["Keys"] = JsonConvert.DeserializeObject<List<KeyValue>>(o["items"].ToString());
            return View(); 
        }

        public async Task<IActionResult> AddKey(string key, string value)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://take2webappconfig.azconfig.io/kv/" + key),
                    Content = new StringContent("{\"value\":\"" + value + "\"}", Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Put
                };

                byte[] secret = System.Convert.FromBase64String("Kp1fICr9L2U5G8tNiqFkYjfbHdQd+4gTxGfUdM4gv7w=");
                //
                // Sign the request
                request.Sign("0-l4-s0:8Koz6Lm3qL2kkyl+4hzq", secret);

                await client.SendAsync(request);
                return RedirectToAction("Index","Home");
            }
        }

        public async Task<IActionResult> DeleteKey(string key)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://take2webappconfig.azconfig.io/kv/" + key),
                    Method = HttpMethod.Delete
                };

                byte[] secret = System.Convert.FromBase64String("Kp1fICr9L2U5G8tNiqFkYjfbHdQd+4gTxGfUdM4gv7w=");
                //
                // Sign the request
                request.Sign("0-l4-s0:8Koz6Lm3qL2kkyl+4hzq", secret);

                await client.SendAsync(request);
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<HttpResponseMessage> ListKeys()
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("https://take2webappconfig.azconfig.io/kv"),
                    Method = HttpMethod.Get
                };

                byte[] secret = System.Convert.FromBase64String("Kp1fICr9L2U5G8tNiqFkYjfbHdQd+4gTxGfUdM4gv7w=");
                //
                // Sign the request
                request.Sign("0-l4-s0:8Koz6Lm3qL2kkyl+4hzq", secret);

                return await client.SendAsync(request);
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
    static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage Sign(this HttpRequestMessage request, string credential, byte[] secret)
        {
            string host = request.RequestUri.Authority;
            string verb = request.Method.ToString().ToUpper();
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            string contentHash = Convert.ToBase64String(request.Content.ComputeSha256Hash());

            //
            // SignedHeaders
            string signedHeaders = "date;host;x-ms-content-sha256"; // Semicolon separated header names

            //
            // String-To-Sign
            var stringToSign = $"{verb}\n{request.RequestUri.PathAndQuery}\n{utcNow.ToString("r")};{host};{contentHash}";

            //
            // Signature
            string signature;

            using (var hmac = new HMACSHA256(secret))
            {
                signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(stringToSign)));
            }

            //
            // Add headers
            request.Headers.Date = utcNow;
            request.Headers.Add("x-ms-content-sha256", contentHash);
            request.Headers.Authorization = new AuthenticationHeaderValue("HMAC-SHA256", $"Credential={credential}, SignedHeaders={signedHeaders}, Signature={signature}");

            return request;
        }
    }


    static class HttpContentExtensions
    {
        public static byte[] ComputeSha256Hash(this HttpContent content)
        {
            using (var stream = new MemoryStream())
            {
                if (content != null)
                {
                    content.CopyToAsync(stream).Wait();
                    stream.Seek(0, SeekOrigin.Begin);
                }

                using (var alg = SHA256.Create())
                {
                    return alg.ComputeHash(stream.ToArray());
                }
            }
        }
    }

}
