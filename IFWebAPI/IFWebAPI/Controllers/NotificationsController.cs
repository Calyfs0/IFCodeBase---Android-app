using IFWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web.Http;

namespace IFWebAPI.Controllers
{
    public class NotificationsController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetNotificationsForUser(JObject jsonObject)
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
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notifications Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        List<UserNotification> userNotifications = entities.UserNotifications.Where(x => x.VendorToken == token).OrderByDescending(x => x.NotificationDate).Skip(offSet).Take(10).ToList();
                        if (userNotifications.Any())
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, userNotifications);
                        }

                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notifications Found");
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notifications Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage GetUnReadNotificationsForUser(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            String token = string.Empty;

            if (list.Any())
            {
                token = list["Token"];
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notifications Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        UserNotification unreadUserNotifications = entities.UserNotifications.FirstOrDefault(x => x.VendorToken == token && x.isNew == true);
                        if (unreadUserNotifications!=null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK);
                        }

                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notifications Found");
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notifications Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage MarkNotificationRead(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            int notificationId = 0;

            if (list.Any())
            {

                notificationId = Convert.ToInt32(list["NotificationId"]);

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notification Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    UserNotification currentNotification = entities.UserNotifications.FirstOrDefault(i => i.NotificationId == notificationId);
                    if (currentNotification != null)
                    {
                        currentNotification.isRead = true;
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);


                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notification Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage MarkNotificationOld(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            string token = string.Empty;

            if (list.Any())
            {

                token = list["Token"];

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notification Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<UserNotification> userNotifications = entities.UserNotifications.Where(i => i.VendorToken == token && i.isNew == true).ToList();
                    if (userNotifications.Any())
                    {
                        foreach (var item in userNotifications)
                        {
                            item.isNew = false;
                        }
                        
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);


                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Notification Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }
    }
}
