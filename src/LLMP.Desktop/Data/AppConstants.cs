﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LLMP.Desktop.Data
{
    public class AppConstants
    {
        public static string OpenAIEndpoint { set; get; } = "https://api.openai.com/v1";
        public static string ImageModel2 = "dall-e-2";
        public static string ImageModel3 = "dall-e-3";
        public static string BlobPrefix = "";
        public static string BlobConn = "";
        public static string ContainerName = "gambar";

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
        public static string GoogleSearchKey = "";
        public static string GoogleSearchID = "";
        public static string AzureSearchApiKey { get; set; }
        public static string AzureSearchEndpoint { get; set; }
        public static string AzureFormApiKey { get; set; }
        public static string AzureFormEndpoint { get; set; }
        public static string AzureAIKey { get; set; }
        public static string AzureAIEndpoint { get; set; }
        public static string OpenAIKey { get; set; }
        public static string OpenAIOrg { get; set; }
        public static string PalmKey { get; set; }
        public static List<string> ModelOpenAIs = new List<string> { "gpt-3.5-turbo-0125", "gpt-4-0125-preview", "gpt-4-vision-preview", "gpt-4o", "gpt-4o-mini", "o1-mini", "o1", "gemini-pro", "gemini-pro-vision", "gemini-1.5-flash", "gemini-1.5-pro", "gemini-2.0-flash-exp", "DeepSeek-R1-tbsft" };        //"chat-bison-001"
        public static List<string> ModelAzureAIs = new List<string> { "DeepSeek-R1-tbsft" };        //"chat-bison-001"

    }
}
