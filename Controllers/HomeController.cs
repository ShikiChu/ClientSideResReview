using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ClientSideResReview.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using System.Reflection.Metadata;

namespace ClientSideResReview.Controllers
{
    public class HomeController : Controller
    {

        private IConfiguration _configuration;
        private string serviceUrl;
        public HomeController(IConfiguration config)
        {
            _configuration = config;
            serviceUrl = _configuration.GetValue<string>("ServerURL");
        }
        public IActionResult Index()
        {
            // create instance, for http response
            HttpClient httpClient = new HttpClient();
            // async for response
            using var response = httpClient.GetAsync(serviceUrl).Result;
            // check response 
            if (response.StatusCode == HttpStatusCode.OK) 
            {   
                // read contents from response
                string apiResp = response.Content.ReadAsStringAsync().Result;

                // process deserilisation and put into list
                List<RestaurantInfo> restaurantInfos = JsonSerializer.Deserialize<List<RestaurantInfo>>(apiResp);

                return View(restaurantInfos);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound( );
            }
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = httpClient.GetAsync($"http://localhost/RestaurantReviewServiceApi/RestaurantReview/{id}").Result;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // read contents from response
                string apiResp =  response.Content.ReadAsStringAsync().Result;

                RestaurantInfo restaurantInfo = JsonSerializer.Deserialize<RestaurantInfo>(apiResp);

                return View(restaurantInfo);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        [HttpPost]
        public IActionResult Edit(RestaurantInfo restInfo)
        {
            HttpClient httpClient = new HttpClient();

            string restInfoString = JsonSerializer.Serialize<RestaurantInfo>(restInfo);

            StringContent stringContent = new StringContent(restInfoString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = httpClient.PutAsync(serviceUrl, stringContent).Result;

            //Console.WriteLine("hearders:" + response.Headers);
            //Console.WriteLine("content: " + response.Content);
            //Console.WriteLine("Tri header: " + response.TrailingHeaders);
            //Console.WriteLine("code " + response.StatusCode);
            //Console.WriteLine("reason: " + response.ReasonPhrase);
            //Console.WriteLine("request msg: " + response.RequestMessage);
            //Console.WriteLine("bool : " + response.IsSuccessStatusCode);



            if (response.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction("Index");
            }
            else 
            {
                return RedirectToAction("Error");
            }
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New(RestaurantInfo restInfo)
        {
            HttpClient httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Host = "localhost";

            string restInfoString = JsonSerializer.Serialize<RestaurantInfo>(restInfo);
            //Console.WriteLine(restInfoString); investagate json 

            StringContent stringContent = new StringContent(restInfoString, Encoding.UTF8, "application/json");

            //using var response = httpClient.PostAsync(serviceUrl, stringContent).Result;
            using var response = await httpClient.PostAsync(serviceUrl, stringContent);

            //Console.WriteLine("hearders:" + response.Headers);
            //Console.WriteLine("content: "+response.Content);
            //Console.WriteLine("Tri header: "+response.TrailingHeaders);
            //Console.WriteLine("code "+response.StatusCode);
            //Console.WriteLine("reason: "+response.ReasonPhrase);
            //Console.WriteLine("request msg: "+response.RequestMessage);
            //Console.WriteLine("bool : " + response.IsSuccessStatusCode);


            //Console.WriteLine(response.Content.ReadAsStringAsync().Exception);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }


        public IActionResult Delete(int? id)
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = httpClient.DeleteAsync($"http://localhost/RestaurantReviewServiceApi/RestaurantReview/{id}").Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }

}
