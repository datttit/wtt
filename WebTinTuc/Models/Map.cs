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
    
    public partial class Map
    {
        public int MapId { get; set; }
        public string MapName { get; set; }
        public Nullable<double> Lat { get; set; }
        public Nullable<double> Long { get; set; }
        public string ApiKey { get; set; }
        public string MapDescription { get; set; }
        public Nullable<bool> Published { get; set; }
    }
}
