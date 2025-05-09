using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Route("api/pdf-reports")]
[ApiController]
public class PDFReportsController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string _reportsFolderPath;

    public PDFReportsController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        _reportsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "PDFReports");

        if (!Directory.Exists(_reportsFolderPath))
        {
            Directory.CreateDirectory(_reportsFolderPath);
        }
    }

  
    [HttpPost("upload")]
    public async Task<IActionResult> UploadPDF(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
            {
                return BadRequest("Only PDF files are allowed.");
            }

            string sanitizedFileName = Path.GetFileNameWithoutExtension(file.FileName)
                                       .Replace(" ", "_") + Path.GetExtension(file.FileName);

            string filePath = Path.Combine(_reportsFolderPath, sanitizedFileName);

            int counter = 1;
            while (System.IO.File.Exists(filePath))
            {
                string newFileName = $"{Path.GetFileNameWithoutExtension(sanitizedFileName)}_{counter}{Path.GetExtension(sanitizedFileName)}";
                filePath = Path.Combine(_reportsFolderPath, newFileName);
                counter++;
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { fileName = Path.GetFileName(filePath), message = "File uploaded successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("list")]
    public IActionResult GetUploadedPDFs()
    {
        try
        {
            var files = Directory.GetFiles(_reportsFolderPath)
                .Select(Path.GetFileName)
                .ToList();

            return Ok(files);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("download/{fileName}")]
    public IActionResult DownloadPDF(string fileName)
    {
        try
        {
            string filePath = Path.Combine(_reportsFolderPath, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
