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

namespace IFWebAPI.Controllers
{
    public class UserController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage UserRegistration(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult.ToString());

            User user = new User();

            String[] st = new String[5];
            int i = 0;

            foreach (KeyValuePair<String, String> s in list)
            {
                st[i] = s.Value;
                i++;
            }

            DateTime dt = DateTime.Now;

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    String email = st[1];
                    var entity = entities.Users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

                    if (entity == null)
                    {
                        User newUser = new User();
                        newUser.FullName = st[0].Trim();
                        newUser.Email = st[1].Trim();
                        newUser.Password = st[2].Trim();
                        newUser.IsActive = true;
                        newUser.CreatedOn = DateTime.Now;
                        newUser.LastLogin = DateTime.Now;
                        newUser.IsLoggedIn = true;
                        newUser.IsUpdated = DateTime.Now;
                        newUser.PhoneNumber = long.Parse(st[4].Trim());
                        newUser.City = st[3].Trim();
                        newUser.login_token = Convert.ToString(Guid.NewGuid());
                        newUser.NumberOfBids = 0;
                        newUser.NumberOfSales = 0;
                        newUser.Rating = 0;
                        newUser.UserBadge = 0;
                        newUser.FeedbackRating = 0;
                        entities.Users.Add(newUser);
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, newUser.login_token);
                    }
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.Ambiguous, "Email already Registered.");

                }
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage AllUsers(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            int offSet = 0;

            if (list.Any())
            {
                offSet = Convert.ToInt32(list["Offset"]);

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<User> user = entities.Users.OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                    if(user.Any())
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User Found");
                }
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex); }
        }

        [HttpPost]
        public HttpResponseMessage AllUsersFromAdminSearch(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            int offSet = 0;
            String userString = String.Empty;

            if (list.Any())
            {
                offSet = Convert.ToInt32(list["Offset"]);
                userString = list["userString"];

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<User> user = entities.Users.Where(x => x.FullName.Contains(userString) || x.Email.Contains(userString) || x.City.Contains(userString)).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                    if (user.Any())
                        return Request.CreateResponse(HttpStatusCode.OK, user);
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User Found");
                }
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex); }
        }

        [HttpPost]
        public HttpResponseMessage UserDetailsForProfile(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult.ToString());

            String[] st = new String[1];
            int i = 0;

            foreach (KeyValuePair<String, String> s in list)
            {
                st[i] = s.Value;
                i++;
            }
            String token = st[0];
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    var entity = entities.Users.FirstOrDefault(k => k.login_token == token);
                    return Request.CreateResponse(HttpStatusCode.OK, new { CurrentUser = entity });
                }
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex); }
        }

        [HttpPost]
        public HttpResponseMessage GetUsersForCityForSearch(JObject jsonObject)
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
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        List<User> searchUsersList = entities.Users.Where(i => i.City == currentUser.City && i.FullName.Contains(searchQuery) && i.IsActive == true).OrderByDescending(i => i.CreatedOn).Skip(offSet).Take(10).ToList();
                        if (searchUsersList.Any())
                            return Request.CreateResponse(HttpStatusCode.OK, searchUsersList);
                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User Found");
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UserLogin(JObject jsonResult)
        {
            if (jsonResult == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such user exist");

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult.ToString());

            String[] st = new String[3];
            int i = 0;

            foreach (KeyValuePair<String, String> s in list)
            {
                st[i] = s.Value;
                i++;
            }
            String token = st[0];
            String email = st[1];
            String password = st[2];

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {

                    User user = entities.Users.FirstOrDefault(j => j.login_token == token);
                    if (user != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, user.login_token);
                    }
                    else
                    {
                        User existingUser = entities.Users.FirstOrDefault(k => k.Email == email && k.Password == password);
                        if (existingUser != null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, existingUser.login_token);
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such user exist");
                        }

                    }
                }
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateUserProfile(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult.ToString());

            String token = string.Empty;
            String city = string.Empty;
            String phoneNumber = string.Empty;
            String name = string.Empty;
            String email = string.Empty;
            String password = string.Empty;

            if (list.Any())
            {
                token = list["Token"];
                city = list["City"];
                phoneNumber = list["PhoneNumber"];
                name = list["Name"];
                password = list["Password"];
            }

            else return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please enter the values");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {

                    User user = entities.Users.FirstOrDefault(j => j.login_token == token);
                    if (user != null)
                    {
                        if (!user.FullName.Equals(name.Trim()))
                        {
                            List<Ad> userAds = entities.Ads.Where(i => i.UserToken == token).ToList();

                            if (userAds.Any())
                            {
                                foreach (Ad ad in userAds)
                                    ad.VendorName = name;
                            }

                            List<WishList> wishList = entities.WishLists.Where(i => i.UserToken == token).ToList();

                            if (wishList.Any())
                            {
                                foreach (WishList wishes in wishList)
                                    wishes.UserName = name;
                            }
                        }

                        user.City = city.Trim();
                        user.PhoneNumber = long.Parse(phoneNumber.Trim());
                        user.FullName = name.Trim();
                        user.Password = String.IsNullOrEmpty(password) ? user.Password : password.Trim();
                        entities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, new { User = user });
                    }
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error processing request");

                }
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateUserProfilePicture(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult.ToString());

            String token = string.Empty;
            String profileImage = string.Empty;


            if (list.Any())
            {
                token = list["Token"];
                profileImage = list["ProfileImage"];

            }

            else return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please enter the values");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {

                    User user = entities.Users.FirstOrDefault(j => j.login_token == token);
                    if (user != null)
                    {
                        if (profileImage.Equals(" "))
                            user.UserProfileImage = null;
                        else
                        user.UserProfileImage = profileImage;

                        entities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, new { User = user });
                    }
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error processing request");

                }
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage ForgotPassword(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResult.ToString());

            String email = string.Empty;

            if (list.Any())
            {
                email = list["Email"];
            }
            else return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    var entity = entities.Users.FirstOrDefault(k => k.Email == email);
                    if(entity!=null)
                    return Request.CreateResponse(HttpStatusCode.OK, entity.Password);
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found");
                }
            }
            catch (Exception ex) { return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex); }
        }

        [HttpPost]
        public HttpResponseMessage PostFeedbackForUser(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            String CurrentUserToken = string.Empty;
            String CurrentUserName = string.Empty;
            String ReviewerUserToken = string.Empty;
            int UserFeedbackRating = 0;
            String UserFeedBack = string.Empty;

            if (list.Any())
            {
                CurrentUserToken = list["CurrentUserToken"];
                CurrentUserName = list["CurrentUserName"];
                ReviewerUserToken = list["ReviewerUserToken"];
                UserFeedbackRating = Convert.ToInt32(list["UserFeedbackRating"]);
                UserFeedBack = list["UserFeedback"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please add a feedback");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())

                {
                    User currentUser = entities.Users.FirstOrDefault(x => x.login_token == CurrentUserToken);

                    User reviewerUser = entities.Users.FirstOrDefault(x => x.login_token == ReviewerUserToken);

                    UserFeedback existingUserFeedback = entities.UserFeedbacks.FirstOrDefault(i => i.ReviewerUserToken == ReviewerUserToken && i.CurrentUserToken == CurrentUserToken);

                    UserFeedback userFeedback = new UserFeedback();

                    if (existingUserFeedback != null)
                    {
                        existingUserFeedback.FeedbackText = UserFeedBack;
                        existingUserFeedback.FeedbackRating = UserFeedbackRating;
                        existingUserFeedback.ReviewedDate = DateTime.Now;
                    }
                    else
                    {
                        
                         userFeedback.CurrentUserToken = CurrentUserToken;
                        userFeedback.CurrentUserName = CurrentUserName;
                        userFeedback.ReviewerUserToken = ReviewerUserToken;
                        userFeedback.ReviewerUserName = reviewerUser.FullName;
                        userFeedback.FeedbackText = UserFeedBack;
                        userFeedback.FeedbackRating = UserFeedbackRating;
                        userFeedback.ReviewedDate = DateTime.Now;
                        userFeedback.IsActive = true;


                        entities.UserFeedbacks.Add(userFeedback);
                    }



                    entities.SaveChanges();

                    List<UserFeedback> allFeedbacksForCurrentUser = entities.UserFeedbacks.Where(j => j.CurrentUserToken == CurrentUserToken).ToList();

                    decimal totalRating = 0;

                    foreach (UserFeedback feedback in allFeedbacksForCurrentUser)
                    {
                        totalRating += feedback.FeedbackRating;
                    }

                    decimal userFeedbackRating = totalRating / allFeedbacksForCurrentUser.Count();
                    currentUser.FeedbackRating = userFeedbackRating;
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, existingUserFeedback!=null ? existingUserFeedback : userFeedback);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetFeedbackForUsers(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            String userToken = string.Empty;
            if (list.Any())
            {
                userToken = list["Token"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Feedbacks");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<UserFeedback> userFeedbacks = entities.UserFeedbacks.Where(i => i.CurrentUserToken == userToken).OrderBy(i => i.ReviewedDate).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, userFeedbacks);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }
    }

    




}

