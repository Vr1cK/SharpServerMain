using Microsoft.AspNetCore.Mvc;
using SharpServerMain.Models;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Force.Crc32;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using System.Reflection.Metadata;
using SharpServerMain.Hash;

namespace SharpServerMain.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostEnvironment;
        public HomeController(ILogger<HomeController> logger,
            IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();

        }
        public async Task<IActionResult> Download(IFormFile videoFile, int startSeconds, int endSeconds, int videoHeight, int videoWeight, int flag)
        {
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
                return BadRequest();
            }

            uint hash = CRC32.CalculateCrc32(videoBytes);
            int userId = Convert.ToInt32(HttpContext.User.Identity?.Name);
            string firstTime = FormatValues(startSeconds);
            string secondTime = FormatValues(endSeconds);
            string serverUrl = $"{Request.Scheme}://{Request.Host.Value}";
            Videoinfo videoinfo = new Videoinfo(userId, flag, videoBytes, firstTime, secondTime, hash, videoHeight, videoWeight, serverUrl);
            string json = JsonConvert.SerializeObject(videoinfo, Formatting.Indented);
            string videoUrl = "http://localhost:8080/jsonreq";
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(videoUrl, content);
                if (response.IsSuccessStatusCode) 
                {   
                    var responseData = await response.Content.ReadAsStringAsync();
                    RequestVideo newRequestVideo = JsonConvert.DeserializeObject<RequestVideo>(responseData)!;
                    return File(newRequestVideo.Bytevideo, "application/octet-stream", "file.mp4");
                }
                else
                {
                    return View("Download");
                }
            }
        }
        [HttpPost]
        public IActionResult SaveVideo([FromBody] JsonElement json)
        {
            string byteVideoStr = json.GetProperty("ByteVideo").GetString();
            byte[] byteVideo = Convert.FromBase64String(byteVideoStr);

            uint hash = Convert.ToUInt32(json.GetProperty("Hash").GetString());

            return View("Download", byteVideo);
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