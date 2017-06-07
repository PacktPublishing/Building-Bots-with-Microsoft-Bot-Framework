using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Emotion;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Facial_Identification_Bot
{
    public static class HelperClass
    {
        public static FaceServiceClient faceClient = 
            new FaceServiceClient(ConfigurationManager.AppSettings["FaceKey"].ToString());
        public static EmotionServiceClient emotionClient = 
            new EmotionServiceClient(ConfigurationManager.AppSettings["EmotionKey"].ToString());
        public static Face face = null;
        public static Microsoft.ProjectOxford.Face.Contract.Face 
            FaceclientFace = null;
        
        public static async Task<string> faceAPIAnalysis(Stream attachemntData)
        {
            try
            {
                Microsoft.ProjectOxford.Face.Contract.Face[] 
                    faceDetectionResult  = await faceClient.DetectAsync(
                          attachemntData,
                           true, true, new FaceAttributeType[]
                           {
                      FaceAttributeType.Age,
                      FaceAttributeType.FacialHair,
                      FaceAttributeType.Gender,
                      FaceAttributeType.HeadPose,
                      FaceAttributeType.Smile,
                      FaceAttributeType.Glasses
                           });
                    face = new Face();
                var firstResult = faceDetectionResult.FirstOrDefault();

                if (firstResult != null)
                {
                    var attributes = firstResult.FaceAttributes;
                    
                    var beard1 = LabelFromConfidenceValue(
                      "beard", attributes.FacialHair.Beard);

                    var moustache = LabelFromConfidenceValue(
                      "moustache", attributes.FacialHair.Moustache);

                    var sideburns = LabelFromConfidenceValue(
                      "sideburns", attributes.FacialHair.Sideburns);

                    var smile = LabelFromConfidenceValue(
                      "smile", attributes.Smile);

                    return "Age: "+ attributes.Age + " \r \n "+
                        "Gender: " + attributes.Gender + " \r \n " +
                        "HeadPose: " + attributes.HeadPose + " \r \n " 
                        +beard1+ " \r \n " + moustache + " \r \n " +
                        sideburns+ " \r \n " + smile+ " \r \n " 
                        + attributes.Glasses;
                }
                else
                {
                    return "Unable to process the given image.";
                }
                
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }
        public static async Task<string> emotionAPIAnalysis(Stream attachemntData)
        {
            string emotionList = "";

            var emotionresults = await emotionClient.RecognizeAsync(attachemntData);

            var legend = new StringBuilder();

            foreach (var person in emotionresults)
            {
                var emotionScores = person.Scores.ToRankedList();

                var labelledScores =
                  emotionScores
                  .OrderByDescending(entry => entry.Value)
                  .Select(
                    entry => new KeyValuePair<string, string>(
                     entry.Key,
                      LabelFromConfidenceValue(entry.Key, entry.Value)));

                var listOfScores = string.Join(
                    " \r \n ",
                    labelledScores.Select(entry => entry.Value));

                legend.AppendLine(listOfScores);
                emotionList = legend.ToString();
            }
            if (emotionList != "")
                return emotionList;
            else
                return "Unable to process the given image";
        }
        static string LabelFromConfidenceValue(string label, double confidence)
        {
            var returnLabel = label;

            if (confidence < 0.3)
            {
                returnLabel = $"No {label}";
            }
            
            return (returnLabel);
        }
       
    }

    public class Face
    {
        public FaceAttributes FaceAttributes { get; set; }
        public Guid FaceId { get; set; }
        public FaceLandmarks FaceLandmarks { get; set; }
        public FaceRectangle FaceRectangle { get; set; }
    }
}