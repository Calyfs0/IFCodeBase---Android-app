using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IFWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace IFWebAPI.Controllers
{
    public class WishListController : ApiController
    {   
        [HttpPost]
        public HttpResponseMessage GetAllWishList()
        {
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<WishList> wishlist = entities.WishLists.ToList();
                    if (wishlist.Any())
                        return Request.CreateResponse(HttpStatusCode.OK, wishlist);
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Wish Found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage PostWish(JObject jsonData)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData.ToString());

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    String token = list["Token"];
                    String image = String.Empty;

                    User user = entities.Users.FirstOrDefault(i => i.login_token == token);

                    WishList wish = new WishList();

                    wish.WishCategory = list["WishCategory"].Trim();
                    wish.WishLocality = list["WishLocality"].Trim();

                    if (list.TryGetValue("WishImage", out image))
                        wish.WishImage = list["WishImage"];

                    wish.WishDescription = list["WishDescription"].Trim();
                    wish.WishCity = user.City.Trim();
                    wish.UserToken = list["Token"];
                    wish.UserName = user.FullName.Trim();
                    wish.UserPhoneNumber = user.PhoneNumber.ToString().Trim();
                    wish.CreatedOn = DateTime.Now;
                    wish.IsActive = true;
                    wish.IsReported = false;
                    wish.IsDeleted = false;
                    wish.NoOfFulfill = 0;
                    wish.isDisplayPhoneNumber = Convert.ToBoolean(list["DisplayPhoneNumber"]);

                    Locality localityDivision = entities.Localities.FirstOrDefault(i => i.LocalityName == wish.WishLocality);
                    if (localityDivision != null)
                    {
                        wish.WishDivision = localityDivision.LocalityDivision;
                    }

                    entities.WishLists.Add(wish);
                    entities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }



        }

        [HttpPost]
        public HttpResponseMessage GetWishesForCity(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            String token = string.Empty;
            int offSet = 0;

            if (list.Any())
            {
                token = list["Token"];
                offSet = Convert.ToInt32(list["Offset"]);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<WishWithFulfill> wishWithFulfillList = new List<WishWithFulfill>();

                    List<WishFulfill> wishFulfillsList = entities.WishFulfills.Where(i => i.UserToken == token).ToList();

                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        var wishList = entities.WishLists.Where(x => x.WishCity == currentUser.City && x.IsDeleted == false && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        if (wishList.Any())
                        {
                            foreach (WishList w in wishList)
                            {
                                WishWithFulfill wishWithFulfill = new WishWithFulfill();

                                wishWithFulfill.WishId = w.WishId;
                                wishWithFulfill.WishDescription = w.WishDescription;
                                wishWithFulfill.WishCategory = w.WishCategory;
                                wishWithFulfill.WishLocality = w.WishLocality;
                                wishWithFulfill.WishCity = w.WishCity;
                                wishWithFulfill.WishImage = w.WishImage;
                                wishWithFulfill.UserName = w.UserName;
                                wishWithFulfill.UserPhoneNumber = w.UserPhoneNumber;
                                wishWithFulfill.UserToken = w.UserToken;
                                wishWithFulfill.CreatedOn = w.CreatedOn;
                                wishWithFulfill.NoOfFulfill = w.NoOfFulfill;
                                wishWithFulfill.isDisplayPhoneNumber = w.isDisplayPhoneNumber;
                                User vendor = entities.Users.FirstOrDefault(i => i.login_token == w.UserToken);
                                wishWithFulfill.UserBadge = vendor.UserBadge;
                                wishWithFulfill.isFullfill = false;

                                if (wishFulfillsList.Any())
                                {
                                    foreach (WishFulfill ww in wishFulfillsList)
                                    {
                                        if(w.WishId == ww.WishId)
                                            wishWithFulfill.isFullfill = true;
                                    }
                                }

                                wishWithFulfillList.Add(wishWithFulfill);

                            }



                            return Request.CreateResponse(HttpStatusCode.OK, wishWithFulfillList);
                        }

                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetWishesForCityWithCategory(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            String token = string.Empty;
            int offSet = 0;
            String Category = string.Empty;

            if (list.Any())
            {
                token = list["Token"];
                offSet = Convert.ToInt32(list["Offset"]);
                Category = list["Category"].Trim();

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Wish Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<WishWithFulfill> wishWithFulfillList = new List<WishWithFulfill>();

                    List<WishFulfill> wishFulfillsList = entities.WishFulfills.Where(i => i.UserToken == token).ToList();

                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        var WishList = entities.WishLists.Where(x => x.WishCity == currentUser.City && x.WishCategory == Category.Trim() && x.IsDeleted == false && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        if (WishList.Any())
                        {

                            foreach (WishList w in WishList)
                            {
                                WishWithFulfill wishWithFulfill = new WishWithFulfill();

                                wishWithFulfill.WishId = w.WishId;
                                wishWithFulfill.WishDescription = w.WishDescription;
                                wishWithFulfill.WishCategory = w.WishCategory;
                                wishWithFulfill.WishLocality = w.WishLocality;
                                wishWithFulfill.WishCity = w.WishCity;
                                wishWithFulfill.WishImage = w.WishImage;
                                wishWithFulfill.UserName = w.UserName;
                                wishWithFulfill.UserPhoneNumber = w.UserPhoneNumber;
                                wishWithFulfill.UserToken = w.UserToken;
                                wishWithFulfill.CreatedOn = w.CreatedOn;
                                wishWithFulfill.NoOfFulfill = w.NoOfFulfill;
                                wishWithFulfill.isDisplayPhoneNumber = w.isDisplayPhoneNumber;
                                User vendor = entities.Users.FirstOrDefault(i => i.login_token == w.UserToken);
                                wishWithFulfill.UserBadge = vendor.UserBadge;
                                wishWithFulfill.isFullfill = false;

                                if (wishFulfillsList.Any())
                                {
                                    foreach (WishFulfill ww in wishFulfillsList)
                                    {
                                        if (w.WishId == ww.WishId)
                                            wishWithFulfill.isFullfill = true;
                                    }
                                }

                                wishWithFulfillList.Add(wishWithFulfill);

                            }

                            return Request.CreateResponse(HttpStatusCode.OK, wishWithFulfillList);
                        }

                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Wish Found");
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Wish Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetWishesForCurrentUser(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            String token = string.Empty;
            int offSet = 0;
            String Category = string.Empty;

            if (list.Any())
            {
                token = list["Token"];
                offSet = Convert.ToInt32(list["Offset"]);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Wish Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        var WishList = entities.WishLists.Where(x => x.WishCity == currentUser.City && x.IsDeleted == false && x.UserToken == token && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        if (WishList.Any())
                        {

                            return Request.CreateResponse(HttpStatusCode.OK, WishList);
                        }

                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Wish Found");
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Wish Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage SubmitReportForWish(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            String reportReason = string.Empty;
            int wishId = 0;
            String token = string.Empty;

            if (list.Any())
            {
                reportReason = list["ReportReason"];
                wishId = Convert.ToInt32(list["WishId"]);
                token = list["Token"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please enter a reason.");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);

                    WishReport wishReport = new WishReport();
                    wishReport.WishId = wishId;
                    wishReport.ReportReason = reportReason;
                    wishReport.ReportDate = DateTime.Now;
                    wishReport.IsActive = true;
                    wishReport.UserName = currentUser.FullName;
                    wishReport.UserToken = currentUser.login_token;
                    
                    entities.WishReports.Add(wishReport);
                    entities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage MarkWishAsDeleted(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int wishId = 0;

            if (list.Any())
            {
                wishId = Convert.ToInt32(list["WishId"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please enter a reason.");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {

                    WishList wishToBeDeleted = entities.WishLists.FirstOrDefault(x => x.WishId == wishId);
                    wishToBeDeleted.IsDeleted = true;
                    entities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetSingleWishDetails(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            
            int wishId = 0;
           

            if (list.Any())
            {
                wishId = Convert.ToInt32(list["WishId"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Wish Found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    WishList currentWish = entities.WishLists.FirstOrDefault(i => i.WishId == wishId && i.IsActive == true);
                    
                    return Request.CreateResponse(HttpStatusCode.OK, currentWish);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }
        [HttpPost]
        public HttpResponseMessage AddOrRemoveAdFullfill(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int wishId = 0;
            string token = String.Empty;
            if (list.Any())
            {
                wishId = Convert.ToInt32(list["WishId"]);
                token = list["Token"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Wish not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);

                    WishList wishToFulfill = entities.WishLists.FirstOrDefault(i => i.WishId == wishId);

                    User vendorUser = entities.Users.FirstOrDefault(x => x.login_token == wishToFulfill.UserToken);

                   

                    WishFulfill wishFulfill = entities.WishFulfills.FirstOrDefault(i => i.UserToken == token && i.WishId == wishId);

                    if (wishFulfill != null)
                    {
                        entities.WishFulfills.Remove(wishFulfill);
                        wishToFulfill.NoOfFulfill = wishToFulfill.NoOfFulfill - 1;
                        currentUser.Rating = currentUser.Rating - 4;

                        UserNotification existingNotification = entities.UserNotifications.FirstOrDefault(i => i.isFulfill == true && i.UserToken == token && i.AdOrWishId == wishId);
                        if (existingNotification != null)
                            entities.UserNotifications.Remove(existingNotification);

                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.Moved, "Fulfill Removed");
                    }
                    else
                    {
                        WishFulfill newFullfill = new WishFulfill()
                        {

                            WishId = wishId,
                            UserToken = token,
                            FullfillDate = DateTime.Now
                        };

                        UserNotification newNotification = new UserNotification();
                        newNotification.AdOrWishId = wishId;
                        newNotification.isCommentForAd = false;
                        newNotification.isCommentForWish = false;
                        newNotification.isDemand = false;
                        newNotification.isFirstBid = false;
                        newNotification.isFulfill = true;
                        newNotification.isHigherBid = false;
                        newNotification.UserToken = token;
                        newNotification.isNew = true;
                        newNotification.VendorName = vendorUser.FullName;
                        newNotification.VendorToken = vendorUser.login_token;
                        newNotification.UserName = currentUser.FullName;
                        newNotification.UserPhoneNumber = Convert.ToString(currentUser.PhoneNumber);
                        newNotification.NotificationDate = DateTime.Now;
                        newNotification.isRead = false;
                        entities.UserNotifications.Add(newNotification);

                        entities.WishFulfills.Add(newFullfill);
                        wishToFulfill.NoOfFulfill = wishToFulfill.NoOfFulfill + 1;
                        currentUser.Rating = currentUser.Rating + 4;
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "Fulfill added");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetWishesForCityInSearch(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            String token = string.Empty;
            int offSet = 0;
            String searchQuery = string.Empty;

            if (list.Any())
            {
                token = list["Token"];
                offSet = Convert.ToInt32(list["Offset"]);
                searchQuery = list["SearchText"];
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<WishWithFulfill> wishWithFulfillList = new List<WishWithFulfill>();

                    List<WishFulfill> wishFulfillsList = entities.WishFulfills.Where(i => i.UserToken == token).ToList();

                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        var wishList = entities.WishLists.Where(x => x.WishCity == currentUser.City && x.IsDeleted == false && (x.WishDescription.Contains(searchQuery) || x.WishLocality.Contains(searchQuery) || x.WishDivision.Contains(searchQuery)) && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        if (wishList.Any())
                        {
                            foreach (WishList w in wishList)
                            {
                                WishWithFulfill wishWithFulfill = new WishWithFulfill();

                                wishWithFulfill.WishId = w.WishId;
                                wishWithFulfill.WishDescription = w.WishDescription;
                                wishWithFulfill.WishCategory = w.WishCategory;
                                wishWithFulfill.WishLocality = w.WishLocality;
                                wishWithFulfill.WishCity = w.WishCity;
                                wishWithFulfill.WishImage = w.WishImage;
                                wishWithFulfill.UserName = w.UserName;
                                wishWithFulfill.UserPhoneNumber = w.UserPhoneNumber;
                                wishWithFulfill.UserToken = w.UserToken;
                                wishWithFulfill.CreatedOn = w.CreatedOn;
                                wishWithFulfill.NoOfFulfill = w.NoOfFulfill;
                                wishWithFulfill.isDisplayPhoneNumber = w.isDisplayPhoneNumber;
                                User vendor = entities.Users.FirstOrDefault(i => i.login_token == w.UserToken);
                                wishWithFulfill.UserBadge = vendor.UserBadge;

                                wishWithFulfill.isFullfill = false;

                                if (wishFulfillsList.Any())
                                {
                                    foreach (WishFulfill ww in wishFulfillsList)
                                    {
                                        if (w.WishId == ww.WishId)
                                            wishWithFulfill.isFullfill = true;
                                    }
                                }

                                wishWithFulfillList.Add(wishWithFulfill);

                            }



                            return Request.CreateResponse(HttpStatusCode.OK, wishWithFulfillList);
                        }

                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

    }

    
}
