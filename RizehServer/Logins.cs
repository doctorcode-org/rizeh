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
    
    public partial class Logins
    {
        public int LoginId { get; set; }
        public int UserId { get; set; }
        public System.DateTime LoginDate { get; set; }
        public string IP { get; set; }
        public string SystemId { get; set; }
    
        public virtual Users tbl_Users { get; set; }
    }
}
