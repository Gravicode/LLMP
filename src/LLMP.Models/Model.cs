using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LLMP.Models
{
    public class ImageModel
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
    }
    public class AISetting
    {
        public double Temperature { set; get; }
        public double TopP { set; get; }
        public int? MaxToken { set; get; }
    }
    public class CompletionData
    {
        public AISetting Setting { get; set; }
        public string ModelId { get; set; }
        public string Completion { get; set; }
        
    }
    public class ImageGenData
    {
       
        public string ModelId { get; set; }
        public string Completion { get; set; }
        
    }
    public class RagData
    {
        public AISetting Setting { get; set; }
        public string ModelId { get; set; }
        public string SystemMessage { get; set; }
        public List<RAGItem> Items { set; get; }
    }
    public class RAGItem
    {
        public List<SourceItem> Sources { get; set; } = new();
        public string ImageUrl { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class SourceItem
    {
        public string Source { get; set; }
        public string Link { get; set; }
    }
    [DataContract]
    public class InputCls
    {
        [DataMember(Order = 1)]
        public string[] Param { get; set; }
        [DataMember(Order = 2)]
        public Type[] ParamType { get; set; }
    }
    [DataContract]
    public class OutputCls
    {
        [DataMember(Order = 1)]
        public bool Result { get; set; }
        [DataMember(Order = 2)]
        public string Message { get; set; }
        [DataMember(Order = 3)]
        public string Data { get; set; }
    }
}
