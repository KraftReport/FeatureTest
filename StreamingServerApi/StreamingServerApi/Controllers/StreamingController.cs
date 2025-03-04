using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StreamingServerApi.Controllers.model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StreamingServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamingController : ControllerBase
    {

        [HttpGet]
        [Route("create-short-song-stream-files")]
        public async Task<IActionResult> CreateShortSongStreamFiles()
        {
            var outputBaseFilePath = "C:\\Users\\kraft\\Documents\\General\\streaming\\shortsong\\stream\\";
            var inputBaseFilePath = "C:\\Users\\kraft\\Documents\\General\\streaming\\shortsong\\resource"; 
            //var keyinfoFilePath = "C:\\Users\\kraft\\Documents\\General\\streaming\\key\\enc.keyinfo";
            var keyinfoFilePath = "http://localhost:8989/enc.keyinfo";
            var formattedkeyinfoFilePath = $"\"{keyinfoFilePath}\"";
            var excelFilePath = "C:\\Users\\kraft\\Downloads\\shortSongData.xlsx";
            var dtolist = new List<ExcelModel>();
            using (var wrokBook = new XLWorkbook(excelFilePath))
            { 
                var worksheet = wrokBook.Worksheet(1);
                var row = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach(var r in row)
                {
                    var dto = new ExcelModel()
                    {
                        inputFileName = r.Cell(3).Value.ToString(),
                        outputFileDirectoryName = r.Cell(1).Value.ToString(),
                        outputSongFileDirectoryName = r.Cell(2).Value.ToString(),
                        outputTsFileName = r.Cell(4).Value.ToString(),
                        outputM3u8FileName = r.Cell(5).Value.ToString()
                    };
                    dtolist.Add(dto);
                }
            }

            foreach (var dto in dtolist)
            {
                var outputfolder = $"{outputBaseFilePath}\\{dto.outputFileDirectoryName}\\{dto.outputSongFileDirectoryName}";
               /* if (!Directory.Exists(outputfolder))
                {*/
                    Directory.CreateDirectory(outputfolder);
            /*    }*/
                var outputtsfilepath = $"\"{outputfolder}\\{dto.outputTsFileName}\"";
                var outputm3u8filepath = $"\"{outputfolder}\\{dto.outputM3u8FileName}\"";
                var inputfilePath = $"\"{inputBaseFilePath}\\{dto.inputFileName}\"";
                
                var command = $" -y -i {inputfilePath} -vn -hls_time 7  -hls_key_info_file {formattedkeyinfoFilePath}  -hls_playlist_type vod  -hls_segment_filename {outputtsfilepath} {outputm3u8filepath}";
                var startInfo = new ProcessStartInfo()
                {
                    FileName = "ffmpeg",
                    Arguments = command,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = outputfolder
                };

                try
                {
                    using (var process = new Process() { StartInfo = startInfo })
                    {
                        var processCompletionSource = new TaskCompletionSource<bool>();
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.Start();
                        var outputError = process.StandardError.ReadToEndAsync();
                        var outputTask = process.StandardOutput.ReadToEndAsync();
                        process.WaitForExit();
                        if (process.ExitCode != 0)
                        {
                            var error = await outputError;
                            throw new Exception(error);
                        }
                        var output = await outputTask;  
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return Ok();
        }

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
                return File(newFileBytes, "application/vnd.apple.mpegurl", "index.m3u8");
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

        [HttpGet]
        [Route("editTs")]
        public IActionResult EditTs()
        {
            var filePath = "C:\\Users\\kraft\\Documents\\General\\streaming\\shortsong\\stream\\HLWAN - 001\\1028\\index.m3u8";
            var content = System.IO.File.ReadAllText(filePath);
            
            var updatedContent = Regex.Replace(content, @"(#EXT-X-KEY:METHOD=AES-128,URI="")(.*?)("")", $"$1https://localhost:44325/api/streaming/getKey/keyotp$3");

            var secondUpdatedContent = Regex.Replace(updatedContent, @"(?<=\n)(output\d+\.ts)", "https://localhost:9090/$1");
            
            System.IO.File.WriteAllText(filePath, secondUpdatedContent);
            return Ok();

        }

        [HttpGet]
        [Route("regex-parse")]
        public IActionResult RegexParse()
        {
            var value = "234-987";
            var pattern = @"^(?<songId>\d+)-(?<albumId>\d+)$";
            var match = Regex.Match(value, pattern);
            var songId = match.Groups["songId"].Value;
            var albumId = match.Groups["albumId"].Value;
            var dto = new
            {
                song = songId,
                album = albumId
            };
            return Ok(dto);
        }
    }
}
