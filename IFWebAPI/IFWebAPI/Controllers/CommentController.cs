using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IFWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IFWebAPI.Controllers
{
    public class CommentController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetCommentsForAds(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Comments");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<AdComment> adComments = entities.AdComments.Where(i => i.AdId == adId).OrderBy(i => i.CommentDate).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, adComments);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage PostCommentForAds(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            String AdComment = string.Empty;
            int adId = 0;
            String token = string.Empty;

            if (list.Any())
            {
                AdComment = list["AdComment"];
                adId = Convert.ToInt32(list["AdId"]);
                token = list["Token"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please add a comment");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(x => x.login_token == token);

                    Ad currentAd = entities.Ads.FirstOrDefault(i => i.AdId == adId);

                    User vendorUser = entities.Users.FirstOrDefault(x => x.login_token == currentAd.UserToken);

                    AdComment newComment = new AdComment();

                    newComment.AdId = adId;
                    newComment.Comment = AdComment.Trim();
                    newComment.CommentDate = DateTime.Now;
                    newComment.IsActive = true;
                    newComment.UserName = currentUser.FullName;
                    newComment.UserToken = token;
                    entities.AdComments.Add(newComment);

                    UserNotification newNotification = new UserNotification();
                    newNotification.AdOrWishId = adId;
                    newNotification.isCommentForAd = true;
                    newNotification.isCommentForWish = false;
                    newNotification.isDemand = false;
                    newNotification.isFirstBid = false;
                    newNotification.isFulfill = false;
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

                    entities.SaveChanges();

                    var AdComments = entities.AdComments.Where(y => y.AdId == adId).OrderByDescending(y => y.CommentDate).FirstOrDefault();

                    return Request.CreateResponse(HttpStatusCode.OK, AdComments);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetCommentsForWish(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int wishId = 0;
            if (list.Any())
            {
                wishId = Convert.ToInt32(list["WishId"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Comments");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<WishComment> wishComments = entities.WishComments.Where(i => i.WishId == wishId).OrderBy(i => i.CommentDate).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, wishComments);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage PostCommentForWish(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            String WishComment = string.Empty;
            int wishId = 0;
            String token = string.Empty;

            if (list.Any())
            {
                WishComment = list["WishComment"];
                wishId = Convert.ToInt32(list["WishId"]);
                token = list["Token"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please add a comment");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(x => x.login_token == token);

                    WishList currentWish = entities.WishLists.FirstOrDefault(i => i.WishId == wishId);

                    User vendorUser = entities.Users.FirstOrDefault(x => x.login_token == currentWish.UserToken);

                    

                    WishComment newComment = new WishComment();

                    newComment.WishId = wishId;
                    newComment.Comment = WishComment.Trim();
                    newComment.CommentDate = DateTime.Now;
                    newComment.IsActive = true;
                    newComment.UserName = currentUser.FullName;
                    newComment.UserToken = token;

                    entities.WishComments.Add(newComment);

                    UserNotification newNotification = new UserNotification();
                    newNotification.AdOrWishId = wishId;
                    newNotification.isCommentForAd = false;
                    newNotification.isCommentForWish = true;
                    newNotification.isDemand = false;
                    newNotification.isFirstBid = false;
                    newNotification.isFulfill = false;
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

                    entities.SaveChanges();

                    var WishComments = entities.WishComments.Where(y => y.WishId == wishId).OrderByDescending(y => y.CommentDate).FirstOrDefault();

                    return Request.CreateResponse(HttpStatusCode.OK, WishComments);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }
    }
}
