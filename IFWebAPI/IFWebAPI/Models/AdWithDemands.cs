using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFWebAPI.Models
{
    public class AdWithDemands
    {
        public int AdId { get; set; }
        public string AdTitle { get; set; }
        public string AdCategory { get; set; }
        public string AdLocality { get; set; }
        public string AdCity { get; set; }
        public string UserToken { get; set; }
        public string VendorName { get; set; }
        public string VendorPhoneNumber { get; set; }
        public string AdImageOne { get; set; }
        public int AdSellingPrice { get; set; }
        public Nullable<int> AdHighestBid { get; set; }
        public string Filter { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> NoOfDemands { get; set; }
        public bool isDemanded { get; set; }
        public int UserBadge { get; set; }
        public bool isDisplayPhoneNumber { get; set; }
    }
}