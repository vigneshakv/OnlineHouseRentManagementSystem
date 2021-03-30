using OnlineHouseRentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineHouseRentManagementSystem.ViewModel
{
    public class SellerViewModel
    {
        public IEnumerable<customers_status> customers_Statuses { get; set; }
        public IEnumerable<booked_properties> booked_Properties { get; set; }

        public int TotalRequests { get; set; }
        public int TotalBooking { get; set; }
    }
}