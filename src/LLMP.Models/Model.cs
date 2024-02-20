using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LLMP.Models
{
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
