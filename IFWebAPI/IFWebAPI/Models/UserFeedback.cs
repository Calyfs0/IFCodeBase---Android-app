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
    
    public partial class UserFeedback
    {
        public int UserFeedbackId { get; set; }
        public string CurrentUserToken { get; set; }
        public string CurrentUserName { get; set; }
        public string ReviewerUserToken { get; set; }
        public string ReviewerUserName { get; set; }
        public string FeedbackText { get; set; }
        public decimal FeedbackRating { get; set; }
        public System.DateTime ReviewedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
