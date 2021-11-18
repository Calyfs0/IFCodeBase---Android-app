//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IFWebAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ad
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
        public string AdImageTwo { get; set; }
        public string AdImageThree { get; set; }
        public string AdImageFour { get; set; }
        public string AdImageFive { get; set; }
        public string AdDescription { get; set; }
        public int AdSellingPrice { get; set; }
        public Nullable<int> AdHighestBid { get; set; }
        public string Filter { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IsSold { get; set; }
        public Nullable<System.DateTime> SoldDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public Nullable<bool> IsEligibleForRepost { get; set; }
        public Nullable<int> NoOfDemands { get; set; }
        public string AdDivision { get; set; }
        public bool isDisplayPhoneNumber { get; set; }
        public int NoOfBids { get; set; }
        public int SoldPrice { get; set; }
    }
}
