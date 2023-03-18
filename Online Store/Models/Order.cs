//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Online_Store.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.Reviews = new HashSet<Review>();
        }
    
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public int InvoiceID { get; set; }
        public int ProductID { get; set; }
        public short OrderQuantity { get; set; }
        [Display(Name = "Unit Price")]
        public short OrderUnitPrice { get; set; }
        [Display(Name = "Total")]
        public short OrderTotal { get; set; }
        [Display(Name = "DateTime")]
        public System.DateTime OrderDateTime { get; set; }
    
        public virtual Invoice Invoice { get; set; }
        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}