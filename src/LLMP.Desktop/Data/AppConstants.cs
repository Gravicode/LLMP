using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLMP.Desktop.Data
{
    public class AppConstants
    {
        public static string BlobPrefix = "https://magicpocket.blob.core.windows.net/gambar/";//"-- Search Key --";
        public static string BlobConn = "DefaultEndpointsProtocol=https;AccountName=magicpocket;AccountKey=9RN2G/QUHfUPylJaimcOwWeRjXQmU4bRH7BZOz8fXEjZWrmfBTfcAK4cSbvi7NPUcGlcrwOFHreY+AStmoYmYg==;EndpointSuffix=core.windows.net";//"-- Search Key --";
        public static string ContainerName = "gambar";//"-- Search Key --";

        public static bool InternetOK {set;get;}

        public static long MaxAllowedFileSize = 500 * 1024000;
        public static string StorageEndpoint = "";
        public static string StorageAccess = "";
        public static string StorageSecret = "";
        public static string StorageBucket = "";

        public static string UploadUrlPrefix = "https://storagemurahaje.blob.core.windows.net/WaterPositive";
     
        
        public static string DefaultModel = "gpt-3.5-turbo-0125";
     
        public const string GemLic = "EDWG-SKFA-D7J1-LDQ5";
        public static string OpenAIKey { get; set; }
        public static string OpenAIOrg { get; set; }
        public static string PalmKey { get; set; }
        public static List<string> ModelOpenAIs = new List<string> { "gpt-3.5-turbo-0125", "gpt-4-0125-preview", "gpt-4-vision-preview", "chat-bison-001" };        
        
    }
}
