using EmotionPlatzi.Web.Models;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


namespace EmotionPlatzi.Web.Utils
{
    public class EmotionHelper
    {

        public EmotionServiceClient emoClient;
        public FaceServiceClient emoClientFace;

        public EmotionHelper(string vKey)
        {
            //emoClient = new EmotionServiceClient(vKey);
            emoClientFace = new FaceServiceClient(vKey, "https://southcentralus.api.cognitive.microsoft.com/face/v1.0");
        }

        public async Task<EmoPicture> DetectAndExtracFacesAsync(Stream vImageString)
        {
            //Emotion[] vEmotion = await emoClient.RecognizeAsync(vImageString);

            //var vPicture = new EmoPicture();

            //vPicture.Faces = ExtractFaces(vEmotion, vPicture);
            //return vPicture;

            var emoPicture = new EmoPicture();
            try
            {
                IEnumerable<FaceAttributeType> faceAttributes = new FaceAttributeType[] { FaceAttributeType.Emotion };
                Face[] faces = await emoClientFace.DetectAsync(vImageString, false, false, faceAttributes);

                emoPicture.Faces = ExtractFaces(faces, emoPicture); //Face
            }
            catch (Exception exc)
            {

                throw;
            }

            

            return emoPicture;


            }

        private ObservableCollection<EmoFace> ExtractFaces(Emotion[] vEmotion, EmoPicture vPicture)
        {
            var lFaces = new ObservableCollection<EmoFace>();

            foreach (Emotion emotion in vEmotion)
            {
                EmoFace vFace = new EmoFace()
                {
                    X = emotion.FaceRectangle.Left,
                    Y = emotion.FaceRectangle.Top,
                    Width = emotion.FaceRectangle.Width,
                    Height = emotion.FaceRectangle.Height,
                    Picture = vPicture
                };
                
                vFace.Emotions = ProcessEmotions(emotion.Scores, vFace);
                lFaces.Add(vFace);
            }

            return lFaces;
        }

        //Face
        private ObservableCollection<EmoFace> ExtractFaces(Face[] vFaces, EmoPicture vPicture)
        {
            var lFaces = new ObservableCollection<EmoFace>();

            foreach (var face in vFaces)
            {
                var vFace = new EmoFace()
                {
                    X = face.FaceRectangle.Left,
                    Y = face.FaceRectangle.Top,
                    Width = face.FaceRectangle.Width,
                    Height = face.FaceRectangle.Height,
                    Picture = vPicture

                };
                vFace.Emotions = ProcessEmotions(face.FaceAttributes.Emotion, vFace);
                lFaces.Add(vFace);

            }

            return lFaces;
        }

        private ObservableCollection<EmoEmotion> ProcessEmotions(EmotionScores scores, EmoFace vFace)
        {
            ObservableCollection<EmoEmotion> lEmotion = new ObservableCollection<EmoEmotion>();

            var vProperties = scores.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var vFilterProperties = vProperties.Where(x => x.PropertyType == typeof(float));

            var vEmoType = EmoEmotionEnum.Undertermined;            
            foreach (var prop in vFilterProperties)
            {
                if (!Enum.TryParse<EmoEmotionEnum>(prop.Name, out vEmoType))
                    vEmoType = EmoEmotionEnum.Undertermined;

                var vEmotion = new EmoEmotion();
                vEmotion.Score = (float)prop.GetValue(scores);
                vEmotion.EmotionType = vEmoType;
                //vEmotion.EmoFaceId = vFace.Id;
                vEmotion.Face = vFace;

                lEmotion.Add(vEmotion);
            }

            return lEmotion;
        }
        

    }
}