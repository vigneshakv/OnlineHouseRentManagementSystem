//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineHouseRentManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class booking_details
    {
        public int si_no { get; set; }
        public int property_id { get; set; }
        public int customer_id { get; set; }
        public string booking_status { get; set; }
        public System.DateTime book_date { get; set; }
    
        public virtual customer customer { get; set; }
        public virtual property property { get; set; }
    }
}
