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
    
    public partial class Adv
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Adv()
        {
            this.AdvContents = new HashSet<AdvContent>();
        }
    
        public int AdvId { get; set; }
        public string AdvTitle { get; set; }
        public string TypeAdv { get; set; }
        public Nullable<bool> Published { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdvContent> AdvContents { get; set; }
    }
}
