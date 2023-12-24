using Microsoft.AspNetCore.Mvc;
using SharpServerMain.Models;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Force.Crc32;
using Newtonsoft.Json;
using System;
namespace SharpServerMain.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string filePath;
        public HomeController(ILogger<HomeController> logger,
            IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
            filePath = "ex5.exe";
        }
        public IActionResult Index()
        {
            return View();

        }
        public async Task<IActionResult> Download(IFormFile videoFile, int startSeconds, int endSeconds)
        {
            Console.WriteLine("Метод Download");
            byte[] videoBytes;
            if (videoFile != null && videoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    videoFile.CopyTo(memoryStream);
                    videoBytes = memoryStream.ToArray();
                }
            }
            else {
                Console.WriteLine("Проблема с видео"); 
                return BadRequest();
            }
            uint hash = CalculateCrc32(videoBytes);
            int userId = Convert.ToInt32(HttpContext.User.Identity?.Name);
            string firstTime = FormatValues(startSeconds);
            string secondTime = FormatValues(endSeconds);
            string serverUrl = $"{Request.Scheme}://{Request.Host.Value}";
            int? height = null;
            int? width = null;
            Console.WriteLine("Перед созданием класса");
            Videoinfo videoinfo = new Videoinfo(userId, 1, videoBytes, firstTime, secondTime, hash, height, width, serverUrl);
            string json = JsonConvert.SerializeObject(videoinfo, Formatting.Indented);
            string videoUrl = "https://example.com/api/videos";
            Console.WriteLine("Перед хттп");
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(videoUrl, content);
                Console.WriteLine("Перед ифами");
                if (response.IsSuccessStatusCode) 
                {   
                    var responseData = await response.Content.ReadAsStringAsync();
                    RequestVideo newRequestVideo = JsonConvert.DeserializeObject<RequestVideo>(responseData)!;

                    return View("Download", newRequestVideo.Bytevideo);
                }
                else
                {
                    Console.WriteLine("Ошибка");
                    return View("Download");
                }
            }
        }
        public static uint CalculateCrc32(byte[] bytes)
        {
            using (Crc32Algorithm crc32 = new Crc32Algorithm())
            {
                byte[] hashBytes = crc32.ComputeHash(bytes);
                return BitConverter.ToUInt32(hashBytes, 0);
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
        public static string FormatValues(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        static byte[] StringToByteArray(string hexString)
    {
        int numberChars = hexString.Length;
        byte[] bytes = new byte[numberChars / 2];
        for (int i = 0; i < numberChars; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
        }
        return bytes;
    }

    static void SaveByteArrayToFile(byte[] byteArray, string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            fs.Write(byteArray, 0, byteArray.Length);
        }
    }
}
}