using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UVAFileStore.Models;

namespace UVAFileStore.Controllers
{
    public class FileController : Controller
    {
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
        public async Task<ActionResult> Upload(string fileName, HttpPostedFileWrapper uploadFile)
        {
            Encoding ecode = Request.ContentEncoding;
            //var upload = uploadFile;
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = System.IO.Path.GetFileName(uploadFile.FileName);
                }
                // сохраняем файл в папку Files в проекте
                await Task.Run(() => { uploadFile.SaveAs(Server.MapPath("~/App_Data/Files/" + fileName)); });
            }

            ViewBag.Message = "файл загружен";
            return View();
        }

        public ActionResult Uploads()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Uploads(IEnumerable<HttpPostedFileBase> uploads)
        {
            if (uploads != null && uploads.Count() > 0)
            {
                foreach (var file in uploads)
                {
                    if (file != null)
                    {
                        // получаем имя файла
                        string fileName = System.IO.Path.GetFileName(file.FileName);
                        // сохраняем файл в папку Files в проекте
                        file.SaveAs(Server.MapPath("~/App_Data/Files/" + fileName));
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
        public JsonResult AjaxUpload()
        {
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    // получаем имя файла
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    // сохраняем файл в папку Files в проекте
                    upload.SaveAs(Server.MapPath("~/App_Data/Files/" + fileName));
                }
            }
            return Json("файл загружен");
        }

        public ActionResult Upload3()
        {
            return View();
        }

        [HttpPost]
        public void Upload3(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        // получаем имя файла
                        string fileName = System.IO.Path.GetFileName(file.FileName);
                        // сохраняем файл в папку Files в проекте
                        file.SaveAs(Server.MapPath("~/App_Data/Files/" + fileName));
                    }
                }
                ViewBag.Message = "файл загружен";
            }
        }
    }
}