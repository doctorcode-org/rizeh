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
    
    public partial class Sites
    {
        public Sites()
        {
            this.tbl_Visits = new HashSet<Visits>();
        }
    
        public int SiteId { get; set; }
        public int OwnerId { get; set; }
        public string Url { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public System.DateTime RegisterDate { get; set; }
        public int OldVisits { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
        public bool Deleted { get; set; }
        public Nullable<System.DateTime> DeleteDate { get; set; }
    
        public virtual Users tbl_Users { get; set; }
        public virtual ICollection<Visits> tbl_Visits { get; set; }
    }
}
