//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebTinTuc.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AttachModel
    {
        public long AttachId { get; set; }
        public string AttachFileName { get; set; }
        public string AttachFilePath { get; set; }
        public string ContentType { get; set; }
        public Nullable<int> ContentLength { get; set; }
        public Nullable<long> DetailId { get; set; }
    
        public virtual DocumentDetail DocumentDetail { get; set; }
    }
}
