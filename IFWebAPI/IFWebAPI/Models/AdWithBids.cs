using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFWebAPI.Models
{
    public class AdWithBids
    {
        public String AdTitle { get; set; }
        public String AdImageOne { get; set; }
        public String AdImageTwo { get; set; }
        public String AdImageThree { get; set; }
        public String AdImageFour { get; set; }
        public String AdImageFive { get; set; }
        public String SellerName { get; set; }
        public String SellerLocality { get; set; }
        public int SellingPrice { get; set; }
        public int FirstBidAmount { get; set; }
        public int SecondBidAmount { get; set; }
        public int ThirdBidAmount { get; set; }
        public String AdDescription { get; set; }
        public bool isDisplayPhoneNumber { get; set; }
        public int NoOfBids { get; set; }
        public DateTime Date { get; set; }
    }
}