using ImageSharePasswordWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImageSharePasswordData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;

namespace ImageSharePasswordWeb.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=MyFirstDatabase;Integrated Security=true;";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Upload(IFormFile image, string password)
        {
            string fileName = $"{Guid.NewGuid()}-{image.FileName}";

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            image.CopyTo(fs);

            var repo = new ImagePasswordRepository(_connectionString);
            int id = repo.Add(fileName, password);
            

            return Redirect($"/home/upload?id={id}&password={password}");
        }

        public IActionResult Upload(int id, string password)
        {

            return View(new LinkToImageViewModel
            {
                Id = id,
                Password = password
            }) ;
        }

        public IActionResult PasswordCheck(int id, string password)
        {
            var repo = new ImagePasswordRepository(_connectionString);
            var image = repo.GetImage(id);
            int valid = 0;
            if (password == image.Password)
            {
                HttpContext.Session.Set("password", password);
            }
            else
            {
                valid = -1;
            }
            return Redirect($"/home/viewimage?id={id}&valid={valid}");
        }

        public IActionResult ViewImage(int id, int valid)
        {
            var session = HttpContext.Session.Get<string>("password");
            var repo = new ImagePasswordRepository(_connectionString);
            var image = repo.GetImage(id);

            if(session != null)
            {
                image.Views++;
                repo.EditAmountOfViews(image.Id, image.Views);                
            }         

            return View(new ViewImageViewModel 
            {
                Image = image,
                Valid = valid,
                PasswordSession = session
            });
        }

      

    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
