using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using UVAFileStore.Models;

namespace UVAFileStore.Controllers
{
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload(string fileName, IFormFile uploadFile)
        {
            if (uploadFile != null && uploadFile.Length > 0)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = Path.GetFileName(uploadFile.FileName);
                }

                var path = Path.Combine(_webHostEnvironment.WebRootPath, "Files/", fileName);
                using var serverFileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);

                // сохраняем файл в папку Files в проекте
                await uploadFile.CopyToAsync(serverFileStream);
            }

            ViewBag.Message = "файл загружен";
            return View();
        }

        public ActionResult Uploads()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Uploads(IEnumerable<IFormFile> uploads)
        {
            if (uploads != null && uploads.Any())
            {
                foreach (var file in uploads)
                {
                    if (file != null)
					{
						var path = Path.Combine(_webHostEnvironment.WebRootPath, "Files/", file.FileName);
						using var serverFileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);

						// сохраняем файл в папку Files в проекте
						await file.CopyToAsync(serverFileStream);
					}
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult AjaxUpload(string id)
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AjaxUpload()
        {
            foreach (var file in HttpContext.Request.Form.Files)
            {
                if (file != null)
				{
					var path = Path.Combine(_webHostEnvironment.WebRootPath, "Files/", file.FileName);
					using var serverFileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);

					// сохраняем файл в папку Files в проекте
					await file.CopyToAsync(serverFileStream);
				}
            }

            return Json("файл загружен");
        }

        public ActionResult Upload3()
        {
            return View();
        }

        [HttpPost]
        public async Task Upload3(IEnumerable<IFormFile> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
					{
						var path = Path.Combine(_webHostEnvironment.WebRootPath, "Files/", file.FileName);
						using var serverFileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);

						// сохраняем файл в папку Files в проекте
						await file.CopyToAsync(serverFileStream);
					}
                }

                ViewBag.Message = "файл загружен";
            }
        }
    }
}