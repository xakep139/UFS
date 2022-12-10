using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UVAFileStore.Controllers
{
    public class FileStoreController : ControllerBase
    {
		private readonly IWebHostEnvironment _webHostEnvironment;

		public FileStoreController(IWebHostEnvironment webHostEnvironment)
        {
			_webHostEnvironment = webHostEnvironment;
		}

        // GET: api/Default
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Default/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Default
        public async Task<IActionResult> Upload(string fileName, string Desc)
        {
            string userName = User.Identity.Name;

            // Check if the request contains multipart/form-data.
            if (!Request.HasFormContentType && Request.Form.Files.Count > 0)
            {
                return StatusCode(StatusCodes.Status415UnsupportedMediaType);
            }

            try
            {
                var uploadFile = Request.Form.Files[0];

                if (uploadFile != null && uploadFile.Length > 0)
                {
					var path = Path.Combine(_webHostEnvironment.WebRootPath, "Files/", fileName);
					using var serverFileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);

					// сохраняем файл в папку Files в проекте
					await uploadFile.CopyToAsync(serverFileStream);
				}

                return Ok();
            }
            catch (Exception exc)
            {
                return StatusCode(500, new { Error = exc });
            }
        }

        // PUT: api/Default/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Default/5
        public void Delete(int id)
        {
        }
    }
}
