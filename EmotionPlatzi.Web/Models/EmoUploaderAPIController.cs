using EmotionPlatzi.Web.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace EmotionPlatzi.Web.Models
{
    public class EmoUploaderAPIController : ApiController
    {
        string key;
        string serverFolderPath;
        EmotionHelper vHelper;
        EmotionPlatziWebContext db = new EmotionPlatziWebContext();

        public EmoUploaderAPIController()
        {
            key = ConfigurationManager.AppSettings["UPLOAD_KEY"];
            serverFolderPath = ConfigurationManager.AppSettings["UPLOAD_DIR"];
            vHelper = new EmotionHelper(key);
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostEmoUploader()
        {
            var vHttpRequest = HttpContext.Current.Request;
            foreach (string Archivo in vHttpRequest.Files)
            {
                string PictureFileName = Guid.NewGuid().ToString() + ".jpg";
                var postedFile = vHttpRequest.Files[Archivo];
                var route = HttpContext.Current.Server.MapPath("~/" + serverFolderPath + "/" + PictureFileName);
                Stream imagens = postedFile.InputStream;
                postedFile.SaveAs(route);
                var emopicture = await vHelper.DetectAndExtracFacesAsync(imagens);
                emopicture.Name = PictureFileName;
                emopicture.Path = serverFolderPath + "/" + PictureFileName;
                db.EmoPictures.Add(emopicture);
                await db.SaveChangesAsync();

                return CreatedAtRoute("DefaultApi", new { id = emopicture.Id }, emopicture);
            }

            return Ok();
        }
    }


    /***
     * <namespaceClienteConsola
{
    classProgram
    {
        staticvoidMain(string[] args)
        {
            Console.ReadLine();
            metodo();
            Console.ReadLine();
        }

        staticasyncvoidmetodo()
        {
            while(Console.ReadLine()!="1")
            {
                try
                {

                    string URI = "http://localhost:38107/api/EmoUploaderAPI";
                  

                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.TransferEncodingChunked = true;

                    var content = new MultipartFormDataContent();
                    var imageContent = new StreamContent(new FileStream("c:/3.jpg", FileMode.Open, FileAccess.Read, FileShare.Read));
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    content.Add(imageContent, "3", "c:/3.jpg");

                    var Respuesta=await httpClient.PostAsync(URI, content);
                    Respuesta.EnsureSuccessStatusCode();
                    var contenido = await Respuesta.Content.ReadAsStringAsync();
                    Console.WriteLine("respuesta post a " + URI + "" + Environment.NewLine + (contenido.Replace("{", Environment.NewLine + "\t{")).Replace("[", "[" + Environment.NewLine + "\t\t "));
                    Console.WriteLine("Programa para obtener Post API");

                    Console.ReadLine();

                }
                catch (HttpRequestException hre)
                {
                    string Text = hre.ToString();

                }

                catch (Exception EX)
                {
                    string Text = EX.ToString();

                }
            }

        } 
    }
}>
     */
}
