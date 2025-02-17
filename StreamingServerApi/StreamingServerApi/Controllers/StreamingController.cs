using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.RegularExpressions;

namespace StreamingServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamingController : ControllerBase
    {
        [HttpGet]
        [Route("getStreamFile/{otp}")]
        public IActionResult GetStreamFile(string otp)
        {
            if(otp== "m3u8otp")
            {
                var indexm3u8FilePath = "C:\\Users\\kraft\\Documents\\General\\streaming\\shortsong\\stream\\ccr\\index.m3u8";
                var fileBytes = System.IO.File.ReadAllBytes(indexm3u8FilePath);
                var fileContent = Encoding.UTF8.GetString(fileBytes);
                var pattern = @"(#EXT-X-KEY:METHOD=AES-128,URI="")(.*?)("")";
                var newKeyFileLocation = "https://localhost:44325/api/streaming/getKey/keyotp";
                var updatedFileContent = Regex.Replace(fileContent, pattern, $"$1{newKeyFileLocation}$3");
                var newFileBytes = Encoding.UTF8.GetBytes(updatedFileContent);
                return File(newFileBytes, "application/x-mpegURL", "index.m3u8");
            }
            return NotFound();
        }

        [HttpGet]
        [Route("getKey/{otp}")]
        public IActionResult GetKey(string otp)
        {
            if(otp == "keyotp")
            {
                var filePath = "C:\\Users\\kraft\\Documents\\General\\streaming\\key\\enc.key";
                var fileByte = System.IO.File.ReadAllBytes(filePath);
                return File(fileByte, "application/octet-stream","enc.key");
            }
            return NotFound();
        }
    }
}
