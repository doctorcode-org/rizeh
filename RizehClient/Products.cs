//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Parsnet
{
    using System;
    using System.Collections.Generic;
    [Serializable]
    public partial class Products
    {
        public Products()
        {
            this.tbl_Payments = new HashSet<Payments>();
        }
    
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public int Discount { get; set; }
        public System.DateTime RegisterDate { get; set; }
    
        public virtual ICollection<Payments> tbl_Payments { get; set; }
        public virtual ProductType tbl_ProductType { get; set; }
    }
}
