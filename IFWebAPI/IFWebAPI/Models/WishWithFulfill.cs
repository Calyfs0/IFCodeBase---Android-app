using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IFWebAPI.Models
{
    public class WishWithFulfill
    {
        public int WishId { get; set; }
        public string WishDescription { get; set; }
        public string WishCategory { get; set; }
        public string WishLocality { get; set; }
        public string WishCity { get; set; }
        public string WishImage { get; set; }
        public string UserName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserToken { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> NoOfFulfill { get; set; }
        public bool isFullfill { get; set; }
        public int UserBadge { get; set; }
        public bool isDisplayPhoneNumber { get; set; }
    }
}