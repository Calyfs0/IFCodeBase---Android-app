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
    
    public partial class User
    {
        public int Userid { get; set; }
        public string login_token { get; set; }
        public string FullName { get; set; }
        public string UserProfileImage { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public Nullable<long> PhoneNumber { get; set; }
        public Nullable<long> Rating { get; set; }
        public Nullable<int> NumberOfBids { get; set; }
        public Nullable<int> NumberOfSales { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime IsUpdated { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime LastLogin { get; set; }
        public Nullable<bool> IsLoggedIn { get; set; }
        public int UserBadge { get; set; }
        public Nullable<decimal> FeedbackRating { get; set; }
    }
}
