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
    
    public partial class Vote
    {
        public int VoteId { get; set; }
        public Nullable<int> Normal { get; set; }
        public Nullable<int> Good { get; set; }
        public Nullable<int> VeryGood { get; set; }
        public Nullable<bool> Published { get; set; }
    }
}