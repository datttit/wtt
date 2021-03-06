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
    
    public partial class Category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Category()
        {
            this.Articles = new HashSet<Article>();
            this.Categories1 = new HashSet<Category>();
        }
    
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> PositionIndex { get; set; }
        public string SlugCategory { get; set; }
        public Nullable<int> ParentCategoryId { get; set; }
        public Nullable<bool> ViewHome { get; set; }
        public Nullable<int> PositionHome { get; set; }
        public Nullable<bool> Published { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Article> Articles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories1 { get; set; }
        public virtual Category Category1 { get; set; }
    }
}
