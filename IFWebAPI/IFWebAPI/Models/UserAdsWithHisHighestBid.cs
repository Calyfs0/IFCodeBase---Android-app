using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFWebAPI.Models
{
    public class UserAdsWithHisHighestBid
    {
        public int AdId { get; set; }
        public String AdTitle { get; set; }
        public String AdLocality { get; set; }
        public DateTime CreatedOn { get; set; }
        public String AdImageOne { get; set; }
        public int? AdSellingPrice { get; set; }
        public int? AdHighestBid { get; set; }
        public int? BidAmount { get; set; }
        public DateTime BidDate { get; set; }
        public String UserToken { get; set; }
        public String VendorPhoneNumber { get; set; }
    }
}