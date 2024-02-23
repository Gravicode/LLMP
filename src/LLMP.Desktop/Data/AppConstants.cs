using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLMP.Desktop.Data
{
    public class AppConstants
    {
        public static string ImageModel2 = "dall-e-2";
        public static string ImageModel3 = "dall-e-3";
        public static string BlobPrefix = "";//"-- Search Key --";
        public static string BlobConn = "";//"-- Search Key --";
        public static string ContainerName = "gambar";//"-- Search Key --";

        public static bool InternetOK {set;get;}

        public static long MaxAllowedFileSize = 500 * 1024000;
        public static string StorageEndpoint = "";
        public static string StorageAccess = "";
        public static string StorageSecret = "";
        public static string StorageBucket = "";

        //public static string UploadUrlPrefix = "https://storagemurahaje.blob.core.windows.net/WaterPositive";
     
        public static string DefaultModel = "gpt-3.5-turbo-0125";
        public static string EmbeddingModel = "text-embedding-ada-002";
     
        public const string GemLic = "";
        public static string GoogleSearchKey = "";//"-- Search Key --";
        public static string GoogleSearchID = "";//"-- Search Key --";
        public static string OpenAIKey { get; set; }
        public static string OpenAIOrg { get; set; }
        public static string PalmKey { get; set; }
        public static List<string> ModelOpenAIs = new List<string> { "gpt-3.5-turbo-0125", "gpt-4-0125-preview", "gpt-4-vision-preview", "gemini-pro" };        //"chat-bison-001"

    }
}
