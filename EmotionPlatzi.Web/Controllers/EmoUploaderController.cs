using EmotionPlatzi.Web.Models;
using EmotionPlatzi.Web.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EmotionPlatzi.Web.Controllers
{
    public class EmoUploaderController : Controller
    {
        string serverFolderPath;
        EmotionHelper vHelper;
        string key;
        EmotionPlatziWebContext db = new EmotionPlatziWebContext();

        public EmoUploaderController()
        {
            key = ConfigurationManager.AppSettings["UPLOAD_KEY"];
            serverFolderPath = ConfigurationManager.AppSettings["UPLOAD_DIR"];
            vHelper = new EmotionHelper(key);            
        }

        // GET: EmoUploader
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(HttpPostedFileBase file)
        {
            try
            {
                if (file?.ContentLength > 0)
                {
                    var vPictureName = Guid.NewGuid().ToString();
                    vPictureName += Path.GetExtension(file.FileName);

                    var vRoute = Server.MapPath(serverFolderPath);
                    vRoute = vRoute + "/" + vPictureName;

                    file.SaveAs(vRoute);

                    var vPicture = await vHelper.DetectAndExtracFacesAsync(file.InputStream);

                    vPicture.Name = file.FileName;
                    vPicture.Path = serverFolderPath + "/" + vPictureName;

                    db.EmoPictures.Add(vPicture);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Details", "EmoPictures", new { Id = vPicture.Id });
                }                
            }
            catch (Exception exc)
            { 

                throw;
            }
            return View();

        }

    }
}