using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace UVAFileStore.Controllers
{
    public class FileStoreController : ApiController
    {
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
        public async Task<HttpResponseMessage> Upload(string fileName, string Desc)
        {
            string userName = User.Identity.Name;

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                var uploadFile = HttpContext.Current.Request.Files[0];

                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    string filPath = HttpContext.Current.Server.MapPath("~/App_Data/Files/" + fileName);

                    // сохраняем файл в папку Files в проекте
                    await Task.Run(() => { uploadFile.SaveAs(filPath); });
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exc)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
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
