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
    public class AdController : ApiController
    {

       int[] ratingArray = new int[240] {7,7,7,7,7,7,7,7,7,7,7,7,7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            5,5,5,5,5,5,5,5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            3,3,3,3,3,3,3,3,3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            1,1,1,1,1,1,1,1,1,1,1,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, };


        [HttpPost]
        public HttpResponseMessage GetAllAds(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            int offSet = 0;

            if (list.Any())
            {
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
                    List<Ad> AdList = entities.Ads.Where(x => x.IsDeleted == false && x.IsSold == false && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                    if (AdList.Any())
                        return Request.CreateResponse(HttpStatusCode.OK, AdList);
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
        public HttpResponseMessage GetAllAdsForSearchQueryinAdmin(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            int offSet = 0;
            String adString = String.Empty;

            if (list.Any())
            {
                offSet = Convert.ToInt32(list["Offset"]);
                adString = list["adString"];

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");
            }

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<Ad> AdList = entities.Ads.Where(x => x.IsDeleted == false && x.IsSold == false && x.IsActive == true && (x.AdDescription.Contains(adString) || x.AdTitle.Contains(adString) || x.AdCategory.Contains(adString) || x.AdCity.Contains(adString) || x.AdLocality.Contains(adString) || x.VendorName.Contains(adString)) ).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                    if (AdList.Any())
                        return Request.CreateResponse(HttpStatusCode.OK, AdList);
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
        public HttpResponseMessage PostAd(JObject jsonObject)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    String token = list["Token"];
                    String image = String.Empty;

                    User user = entities.Users.FirstOrDefault(i => i.login_token == token);

                    Ad ad = new Ad();

                    ad.AdTitle = list["AdTitle"].Trim();
                    ad.AdCategory = list["AdCategory"].Trim();
                    ad.AdLocality = list["AdLocality"].Trim();
                    ad.AdImageOne = (list["AdImage1"]);

                    if (list.TryGetValue("AdImage2", out image))
                        ad.AdImageTwo = list["AdImage2"];



                    if (list.TryGetValue("AdImage3", out image))
                        ad.AdImageThree = list["AdImage3"];

                    if (list.TryGetValue("AdImage4", out image))
                        ad.AdImageFour = list["AdImage4"];

                    if (list.TryGetValue("AdImage5", out image))
                        ad.AdImageFive = list["AdImage5"];

                    ad.AdDescription = list["AdDescription"].Trim();
                    ad.AdSellingPrice = Convert.ToInt32(Convert.ToDouble(list["AdSellingPrice"]));
                    ad.AdHighestBid = 0;
                    ad.AdCity = user.City.Trim();
                    ad.UserToken = list["Token"];
                    ad.Filter = list["Filter"].Trim();
                    ad.VendorName = user.FullName.Trim();
                    ad.VendorPhoneNumber = user.PhoneNumber.ToString().Trim();
                    ad.CreatedOn = DateTime.Now;
                    ad.IsActive = true;
                    ad.IsSold = false;
                    ad.IsDeleted = false;
                    ad.IsEligibleForRepost = false;
                    ad.NoOfDemands = 0;
                    ad.isDisplayPhoneNumber = Convert.ToBoolean(list["DisplayPhoneNumber"]);

                    Locality localityDivision = entities.Localities.FirstOrDefault(i => i.LocalityName == ad.AdLocality);
                    if (localityDivision != null)
                    {
                        ad.AdDivision = localityDivision.LocalityDivision;
                    }

                    entities.Ads.Add(ad);

                    user.Rating = user.Rating + 5;

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
        public HttpResponseMessage GetAdsForCity(JObject jsonObject)
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
                    List<AdWithDemands> adWithDemands = new List<AdWithDemands>();
                    List<AdsDemand> userAdDemands = entities.AdsDemands.Where(i => i.UserToken == token).ToList();

                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        var AdList = entities.Ads.Where(x => x.AdCity == currentUser.City && x.IsDeleted == false && x.IsSold == false && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        if (AdList.Any())
                        {
                            foreach (Ad ad in AdList)
                            {
                                AdWithDemands newAdWithDemand = new AdWithDemands();

                                newAdWithDemand.AdId = ad.AdId;
                                newAdWithDemand.AdTitle = ad.AdTitle;
                                newAdWithDemand.AdCategory = ad.AdCategory;
                                newAdWithDemand.AdLocality = ad.AdLocality;
                                newAdWithDemand.AdCity = ad.AdCity;
                                newAdWithDemand.UserToken = ad.UserToken;
                                newAdWithDemand.VendorName = ad.VendorName;
                                newAdWithDemand.VendorPhoneNumber = ad.VendorPhoneNumber;
                                newAdWithDemand.AdImageOne = ad.AdImageOne;
                                newAdWithDemand.AdSellingPrice = ad.AdSellingPrice;
                                newAdWithDemand.AdHighestBid = ad.AdHighestBid;
                                newAdWithDemand.Filter = ad.Filter;
                                newAdWithDemand.CreatedOn = ad.CreatedOn;
                                newAdWithDemand.isDisplayPhoneNumber = ad.isDisplayPhoneNumber;
                                newAdWithDemand.NoOfDemands = ad.NoOfDemands;

                                User vendor = entities.Users.FirstOrDefault(i => i.login_token == ad.UserToken);
                                newAdWithDemand.UserBadge = vendor.UserBadge;

                                newAdWithDemand.isDemanded = false;

                                if (userAdDemands.Any())
                                {
                                    foreach (AdsDemand a in userAdDemands)
                                    {
                                        if (ad.AdId == a.AdId)
                                            newAdWithDemand.isDemanded = true;
                                    }
                                }
                                adWithDemands.Add(newAdWithDemand);
                            }

                            

                            return Request.CreateResponse(HttpStatusCode.OK, adWithDemands);
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
        public HttpResponseMessage GetAdsForUser(JObject jsonObject)
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
                    List<AdWithDemands> adWithDemands = new List<AdWithDemands>();
                    List<AdsDemand> userAdDemands = entities.AdsDemands.Where(i => i.UserToken == token).ToList();
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        var AdList = entities.Ads.Where(x => x.UserToken == token && x.IsSold == false && x.IsDeleted == false && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        if (AdList.Any())
                        {

                            return Request.CreateResponse(HttpStatusCode.OK, AdList);
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
        public HttpResponseMessage GetAdsForCurrentUser(JObject jsonObject)
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

                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {

                        List<Ad> ads = entities.Ads.Where(i => i.UserToken == token && i.IsSold == false && i.IsDeleted == false && i.IsActive == true).ToList();

                        DateTime currentDate = DateTime.Now;

                        foreach (Ad ad in ads)
                        {
                            DateTime adDate = ad.CreatedOn;
                            double totalDays = (currentDate - adDate).TotalDays;

                            if (totalDays >= 3)
                                ad.IsEligibleForRepost = true;
                            else
                                ad.IsEligibleForRepost = false;
                        }

                        entities.SaveChanges();


                        var AdList = entities.Ads.Where(x => x.UserToken == token && x.IsSold == false && x.IsDeleted == false && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        if (AdList.Any())
                        {

                            return Request.CreateResponse(HttpStatusCode.OK, AdList);
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
        public HttpResponseMessage GetAdsForCityWithCategory(JObject jsonObject)
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
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {

                    List<AdWithDemands> adWithDemands = new List<AdWithDemands>();
                    List<AdsDemand> userAdDemands = entities.AdsDemands.Where(i => i.UserToken == token).ToList();
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        var AdList = entities.Ads.Where(x => x.AdCity == currentUser.City && x.AdCategory == Category.Trim() && x.IsActive == true && x.IsDeleted == false && x.IsSold == false).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        if (AdList.Any())
                        {
                            foreach (Ad ad in AdList)
                            {
                                AdWithDemands newAdWithDemand = new AdWithDemands();

                                newAdWithDemand.AdId = ad.AdId;
                                newAdWithDemand.AdTitle = ad.AdTitle;
                                newAdWithDemand.AdCategory = ad.AdCategory;
                                newAdWithDemand.AdLocality = ad.AdLocality;
                                newAdWithDemand.AdCity = ad.AdCity;
                                newAdWithDemand.UserToken = ad.UserToken;
                                newAdWithDemand.VendorName = ad.VendorName;
                                newAdWithDemand.VendorPhoneNumber = ad.VendorPhoneNumber;
                                newAdWithDemand.AdImageOne = ad.AdImageOne;
                                newAdWithDemand.AdSellingPrice = ad.AdSellingPrice;
                                newAdWithDemand.AdHighestBid = ad.AdHighestBid;
                                newAdWithDemand.Filter = ad.Filter;
                                newAdWithDemand.CreatedOn = ad.CreatedOn;
                                newAdWithDemand.NoOfDemands = ad.NoOfDemands;
                                newAdWithDemand.isDisplayPhoneNumber = ad.isDisplayPhoneNumber;
                                User vendor = entities.Users.FirstOrDefault(i => i.login_token == ad.UserToken);
                                newAdWithDemand.UserBadge = vendor.UserBadge;
                                newAdWithDemand.isDemanded = false;

                                if (userAdDemands.Any())
                                {
                                    foreach (AdsDemand a in userAdDemands)
                                    {
                                        if (ad.AdId == a.AdId)
                                            newAdWithDemand.isDemanded = true;
                                    }
                                }
                                adWithDemands.Add(newAdWithDemand);
                            }

                                return Request.CreateResponse(HttpStatusCode.OK, adWithDemands);
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
        public HttpResponseMessage GetAdsForCityWithFilter(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            String token = string.Empty;
            int offSet = 0;
            String Filter = string.Empty;

            if (list.Any())
            {
                token = list["Token"];
                offSet = Convert.ToInt32(list["Offset"]);
                Filter = list["Filter"].Trim();

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<AdWithDemands> adWithDemands = new List<AdWithDemands>();
                    List<AdsDemand> userAdDemands = entities.AdsDemands.Where(i => i.UserToken == token).ToList();
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        List<Ad> AdList = new List<Ad>();
                        if (!Filter.Equals("Most demanded"))
                        {
                            AdList = entities.Ads.Where(x => x.AdCity == currentUser.City && x.Filter == Filter && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        }
                        else
                        {
                            AdList = entities.Ads.Where(x => x.AdCity == currentUser.City && x.NoOfDemands > 0 && x.IsActive == true).OrderByDescending(x => x.NoOfDemands).Skip(offSet).Take(7).ToList();
                        }
                        if (AdList.Any())
                        {
                            foreach (Ad ad in AdList)
                            {
                                AdWithDemands newAdWithDemand = new AdWithDemands();

                                newAdWithDemand.AdId = ad.AdId;
                                newAdWithDemand.AdTitle = ad.AdTitle;
                                newAdWithDemand.AdCategory = ad.AdCategory;
                                newAdWithDemand.AdLocality = ad.AdLocality;
                                newAdWithDemand.AdCity = ad.AdCity;
                                newAdWithDemand.UserToken = ad.UserToken;
                                newAdWithDemand.VendorName = ad.VendorName;
                                newAdWithDemand.VendorPhoneNumber = ad.VendorPhoneNumber;
                                newAdWithDemand.AdImageOne = ad.AdImageOne;
                                newAdWithDemand.AdSellingPrice = ad.AdSellingPrice;
                                newAdWithDemand.AdHighestBid = ad.AdHighestBid;
                                newAdWithDemand.Filter = ad.Filter;
                                newAdWithDemand.CreatedOn = ad.CreatedOn;
                                newAdWithDemand.NoOfDemands = ad.NoOfDemands;
                                newAdWithDemand.isDisplayPhoneNumber = ad.isDisplayPhoneNumber;
                                User vendor = entities.Users.FirstOrDefault(i => i.login_token == ad.UserToken);
                                newAdWithDemand.UserBadge = vendor.UserBadge;
                                newAdWithDemand.isDemanded = false;

                                if (userAdDemands.Any())
                                {
                                    foreach (AdsDemand a in userAdDemands)
                                    {
                                        if (ad.AdId == a.AdId)
                                            newAdWithDemand.isDemanded = true;
                                    }
                                }
                                adWithDemands.Add(newAdWithDemand);
                            }

                                return Request.CreateResponse(HttpStatusCode.OK, adWithDemands);
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
        public HttpResponseMessage GetAdsForCityWithCategoryAndFilter(JObject jsonObject)
        {

            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject.ToString());
            String token = string.Empty;
            int offSet = 0;
            String Category = string.Empty;
            String Filter = string.Empty;

            if (list.Any())
            {
                token = list["Token"];
                offSet = Convert.ToInt32(list["Offset"]);
                Category = list["Category"].Trim();
                Filter = list["Filter"].Trim();

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Ad Found");
            }
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<AdWithDemands> adWithDemands = new List<AdWithDemands>();
                    List<AdsDemand> userAdDemands = entities.AdsDemands.Where(i => i.UserToken == token).ToList();
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        List<Ad> AdList = new List<Ad>();
                        if (!Filter.Equals("Most demanded"))
                        {
                            AdList = entities.Ads.Where(x => x.AdCity == currentUser.City && x.AdCategory == Category && x.Filter == Filter && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        }
                        else
                        {
                            AdList = entities.Ads.Where(x => x.AdCity == currentUser.City && x.AdCategory == Category && x.NoOfDemands > 0 && x.IsActive == true).OrderByDescending(x => x.NoOfDemands).Skip(offSet).Take(7).ToList();
                        }
                           

                        if (AdList.Any())
                        {
                            foreach (Ad ad in AdList)
                            {
                                AdWithDemands newAdWithDemand = new AdWithDemands();

                                newAdWithDemand.AdId = ad.AdId;
                                newAdWithDemand.AdTitle = ad.AdTitle;
                                newAdWithDemand.AdCategory = ad.AdCategory;
                                newAdWithDemand.AdLocality = ad.AdLocality;
                                newAdWithDemand.AdCity = ad.AdCity;
                                newAdWithDemand.UserToken = ad.UserToken;
                                newAdWithDemand.VendorName = ad.VendorName;
                                newAdWithDemand.VendorPhoneNumber = ad.VendorPhoneNumber;
                                newAdWithDemand.AdImageOne = ad.AdImageOne;
                                newAdWithDemand.AdSellingPrice = ad.AdSellingPrice;
                                newAdWithDemand.AdHighestBid = ad.AdHighestBid;
                                newAdWithDemand.Filter = ad.Filter;
                                newAdWithDemand.CreatedOn = ad.CreatedOn;
                                newAdWithDemand.NoOfDemands = ad.NoOfDemands;
                                newAdWithDemand.isDisplayPhoneNumber = ad.isDisplayPhoneNumber;
                                User vendor = entities.Users.FirstOrDefault(i => i.login_token == ad.UserToken);
                                newAdWithDemand.UserBadge = vendor.UserBadge;
                                newAdWithDemand.isDemanded = false;

                                if (userAdDemands.Any())
                                {
                                    foreach (AdsDemand a in userAdDemands)
                                    {
                                        if (ad.AdId == a.AdId)
                                            newAdWithDemand.isDemanded = true;
                                    }
                                }
                                adWithDemands.Add(newAdWithDemand);
                            }

                                return Request.CreateResponse(HttpStatusCode.OK, adWithDemands);
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
        public HttpResponseMessage SubmitReportForAds(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            String reportReason = string.Empty;
            int adId = 0;
            String token = string.Empty;

            if (list.Any())
            {
                token = list["Token"];
                reportReason = list["ReportReason"];
                adId = Convert.ToInt32(list["AdId"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please enter a reason.");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);

                    AdReport adReport = new AdReport();
                    adReport.AdId = adId;
                    adReport.ReportReason = reportReason;
                    adReport.ReportDate = DateTime.Now;
                    adReport.IsActive = true;
                    adReport.UserName = currentUser.FullName;
                    adReport.UserToken = token;
                    entities.AdReports.Add(adReport);
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
        public HttpResponseMessage MakeBidForAds(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int bidAmount = 0;
            int adId = 0;
            String token = string.Empty;
            bool isDisplayPhoneNumber = true;

            if (list.Any())
            {
                bidAmount = Convert.ToInt32(Convert.ToDouble(list["BidAmount"]));
                adId = Convert.ToInt32(list["AdId"]);
                token = list["Token"];
                isDisplayPhoneNumber = Convert.ToBoolean(list["isDisplayPhoneNumber"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please enter a reason.");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(x => x.login_token == token);

                    Ad currentAd = entities.Ads.FirstOrDefault(x => x.AdId == adId);

                    User vendorUser = entities.Users.FirstOrDefault(x => x.login_token == currentAd.UserToken);

                    AdBid currentHighestBid = entities.AdBids.FirstOrDefault(i => i.AdId == adId && i.IsHighestBid == true);

                    AdBid newBid = new AdBid();

                    if (currentHighestBid == null)
                    {
                        newBid.IsHighestBid = true;

                        UserNotification newNotification = new UserNotification();
                        newNotification.AdOrWishId = adId;
                        newNotification.isCommentForAd = false;
                        newNotification.isCommentForWish = false;
                        newNotification.isDemand = false;
                        newNotification.isFirstBid = true;
                        newNotification.isFulfill = false;
                        newNotification.isHigherBid = false;
                        newNotification.UserToken = token;
                        newNotification.isNew = true;
                        if (isDisplayPhoneNumber)
                            newNotification.isDisplayPhoneNumber = true;
                        else
                            newNotification.isDisplayPhoneNumber = false;
                        newNotification.VendorName = vendorUser.FullName;
                        newNotification.VendorToken = vendorUser.login_token;
                        newNotification.UserName = currentUser.FullName;
                        newNotification.UserPhoneNumber = Convert.ToString(currentUser.PhoneNumber);
                        newNotification.NotificationDate = DateTime.Now;
                        newNotification.isRead = false;
                        entities.UserNotifications.Add(newNotification);
                    }
                        

                    else if (bidAmount >= currentHighestBid.BidAmount)
                    {
                        newBid.IsHighestBid = true;
                        currentHighestBid.IsHighestBid = false;

                        UserNotification newNotification = new UserNotification();
                        newNotification.AdOrWishId = adId;
                        newNotification.isCommentForAd = false;
                        newNotification.isCommentForWish = false;
                        newNotification.isDemand = false;
                        newNotification.isFirstBid = false;
                        newNotification.isFulfill = false;
                        newNotification.isHigherBid = true;
                        newNotification.UserToken = token;
                        newNotification.isNew = true;
                        if (isDisplayPhoneNumber)
                            newNotification.isDisplayPhoneNumber = true;
                        else
                            newNotification.isDisplayPhoneNumber = false;

                        newNotification.VendorName = vendorUser.FullName;
                        newNotification.VendorToken = vendorUser.login_token;
                        newNotification.UserName = currentUser.FullName;
                        newNotification.UserPhoneNumber = Convert.ToString(currentUser.PhoneNumber);
                        newNotification.NotificationDate = DateTime.Now;
                        newNotification.isRead = false;
                        entities.UserNotifications.Add(newNotification);
                    }


                    else
                        newBid.IsHighestBid = false;

                    newBid.AdId = adId;
                    newBid.BidAmount = bidAmount;
                    newBid.BidDate = DateTime.Now;
                    newBid.IsActive = true;
                    newBid.UserName = currentUser.FullName;
                    newBid.UserToken = currentUser.login_token;

                    if (currentHighestBid != null)
                    {
                        if (bidAmount >= currentHighestBid.BidAmount)
                        {
                            currentAd.AdHighestBid = bidAmount;
                        }

                    }
                    else
                    {
                        currentAd.AdHighestBid = bidAmount;
                    }

                    entities.AdBids.Add(newBid);
                    currentUser.NumberOfBids = currentUser.NumberOfBids + 1;


                    currentAd.NoOfBids = currentAd.NoOfBids + 1;
                    entities.SaveChanges();


                    return Request.CreateResponse(HttpStatusCode.OK, currentAd.AdHighestBid);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }



        [HttpPost]
        public HttpResponseMessage GetCommentsForAds(JObject jsonResult)
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

                    AdComment newComment = new AdComment();

                    newComment.AdId = adId;
                    newComment.Comment = AdComment.Trim();
                    newComment.CommentDate = DateTime.Now;
                    newComment.IsActive = true;
                    newComment.UserName = currentUser.FullName;
                    newComment.UserToken = token;

                    entities.AdComments.Add(newComment);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK, AdComment);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetSingleAdDetails(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Please add a comment");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    List<AdBid> threeHighestBids = entities.AdBids.Where(x => x.AdId == adId).OrderByDescending(x => x.BidAmount).Take(3).ToList();

                    int i = 0;
                    int numberOfBids = threeHighestBids.Count();
                    int[] bids = new int[numberOfBids];

                    if (threeHighestBids.Any())
                    {
                        foreach (AdBid a in threeHighestBids)
                        {
                            bids[i] = a.BidAmount;
                            i++;
                        }
                    }

                    int firstBidAmount = 0;
                    int secondBidAmount = 0;
                    int thirdBidAmount = 0;

                    if (numberOfBids == 1)
                    {
                        firstBidAmount = bids[0];
                    }
                    else if (numberOfBids == 2)
                    {
                        firstBidAmount = bids[0];
                        secondBidAmount = bids[1];
                    }
                    else if (numberOfBids == 3)
                    {
                        firstBidAmount = bids[0];
                        secondBidAmount = bids[1];
                        thirdBidAmount = bids[2];
                    }



                    Ad ads = entities.Ads.FirstOrDefault(z => z.AdId == adId);


                    List<AdWithBids> adWithBids = (from ad in entities.Ads
                                                   where ad.AdId == adId
                                                   select new
                                                   {
                                                       AdTitle = ad.AdTitle,
                                                       SellerName = ad.VendorName,
                                                       SellerLocality = ad.AdLocality,
                                                       SellingPrice = ad.AdSellingPrice,
                                                       AdDescription = ad.AdDescription
                                                   })

                                                       .Select(x => new AdWithBids
                                                       {
                                                           AdTitle = x.AdTitle,
                                                           AdImageOne = ads.AdImageOne,
                                                           AdImageTwo = ads.AdImageTwo,
                                                           AdImageThree = ads.AdImageThree,
                                                           AdImageFour = ads.AdImageFour,
                                                           AdImageFive = ads.AdImageFive,
                                                           SellerName = x.SellerName,
                                                           SellerLocality = x.SellerLocality,
                                                           SellingPrice = x.SellingPrice,
                                                           FirstBidAmount = firstBidAmount,
                                                           SecondBidAmount = secondBidAmount,
                                                           ThirdBidAmount = thirdBidAmount,
                                                           AdDescription = x.AdDescription,
                                                           isDisplayPhoneNumber = ads.isDisplayPhoneNumber,
                                                           NoOfBids = ads.NoOfBids,
                                                           Date = ads.CreatedOn

                                                       }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, adWithBids);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage DeleteSelectedAd(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    Ad adToBeDeleted = entities.Ads.FirstOrDefault(i => i.AdId == adId);

                    if (adToBeDeleted != null)
                    {
                        adToBeDeleted.IsDeleted = true;
                        adToBeDeleted.DeletedDate = DateTime.Now;
                        entities.SaveChanges();
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");


                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage DeleteSelectedAdPermanently(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    Ad adToBeDeleted = entities.Ads.FirstOrDefault(i => i.AdId == adId);

                    if (adToBeDeleted != null)
                    {
                        entities.Ads.Remove(adToBeDeleted);
                        entities.SaveChanges();
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");


                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage MarkSoldSelectedAd(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            String token = String.Empty;
            int soldPrice = 0;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
                token = list["Token"];
                soldPrice = Convert.ToInt32(list["SoldPrice"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);

                    Ad adToBeSold = entities.Ads.FirstOrDefault(i => i.AdId == adId);

                    int noOfHours = Convert.ToInt32((DateTime.Now - adToBeSold.CreatedOn).TotalHours);

                    int rating = noOfHours< 240 ?ratingArray[noOfHours] : 0;

                    currentUser.Rating = currentUser.Rating + rating;
                        

                    List<AdBid> adBids = entities.AdBids.Where(i => i.AdId == adId).OrderByDescending(i => i.BidAmount).Take(5).ToList();

                    if (adBids.Any())
                    {
                        if (adBids.Count() <= 3)
                        {
                            foreach (AdBid adBid in adBids)
                            {
                                User user = entities.Users.FirstOrDefault(i => i.login_token == adBid.UserToken);
                                user.Rating = user.Rating + 3;
                            }
                        }

                        else
                        {
                            int counter = 0;
                            while (counter < 3)
                            {
                                String userToken = adBids[counter].UserToken;
                                User user = entities.Users.FirstOrDefault(i => i.login_token == userToken);
                                user.Rating = user.Rating + 3;
                                counter++;
                            }

                            if (adBids.Count() == 4)
                            {
                                String userToken = adBids[3].UserToken;
                                User user = entities.Users.FirstOrDefault(i => i.login_token == userToken);
                                user.Rating = user.Rating + 2;
                                
                            }

                            if (adBids.Count() == 5)
                            {
                                String userToken = adBids[3].UserToken;
                                User user = entities.Users.FirstOrDefault(i => i.login_token == userToken);
                                user.Rating = user.Rating + 2;
                                String userToken2 = adBids[4].UserToken;
                                User user2 = entities.Users.FirstOrDefault(i => i.login_token == userToken2);
                                user2.Rating = user.Rating + 2;

                            }

                        }
                            
                    }


                    if (adToBeSold != null)
                    {
                        adToBeSold.IsSold = true;
                        adToBeSold.SoldDate = DateTime.Now;
                        adToBeSold.SoldPrice = soldPrice;
                        currentUser.NumberOfSales = currentUser.NumberOfSales + 1;
                        entities.SaveChanges();
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");


                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage RepostSelectedAd(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    Ad adToBeSold = entities.Ads.FirstOrDefault(i => i.AdId == adId);

                    if (adToBeSold != null)
                    {
                        adToBeSold.CreatedOn = DateTime.Now;
                        adToBeSold.IsEligibleForRepost = false;
                        entities.SaveChanges();
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");


                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage AddOrRemoveAdDemand(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            string token = String.Empty;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
                token = list["Token"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);

                    Ad adInDemand = entities.Ads.FirstOrDefault(i => i.AdId == adId);

                    User vendorUser = entities.Users.FirstOrDefault(x => x.login_token == adInDemand.UserToken);

                    

                    AdsDemand adDemand = entities.AdsDemands.FirstOrDefault(i => i.UserToken == token && i.AdId == adId);

                    if (adDemand != null)
                    {
                        entities.AdsDemands.Remove(adDemand);
                        adInDemand.NoOfDemands = adInDemand.NoOfDemands - 1;
                        vendorUser.Rating = vendorUser.Rating - 4;

                        UserNotification existingNotification = entities.UserNotifications.FirstOrDefault(i => i.isDemand == true && i.UserToken == token && i.AdOrWishId == adId);
                        if (existingNotification != null)
                            entities.UserNotifications.Remove(existingNotification);

                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.Moved, "Demand Removed");
                    }
                    else
                    {
                        AdsDemand newDemand = new AdsDemand()
                        {

                            AdId = adId,
                            UserToken = token,
                            DemandDate = DateTime.Now
                        };
                        entities.AdsDemands.Add(newDemand);
                        adInDemand.NoOfDemands = adInDemand.NoOfDemands + 1;
                        vendorUser.Rating = vendorUser.Rating + 4;

                        UserNotification newNotification = new UserNotification();
                        newNotification.AdOrWishId = adId;
                        newNotification.isCommentForAd = false;
                        newNotification.isCommentForWish = false;
                        newNotification.isDemand = true;
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
                        return Request.CreateResponse(HttpStatusCode.OK, "Demand Added");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetAdsForCityForSearch(JObject jsonObject)
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

                    List<AdWithDemands> adWithDemands = new List<AdWithDemands>();
                    List<AdsDemand> userAdDemands = entities.AdsDemands.Where(i => i.UserToken == token).ToList();

                    User currentUser = entities.Users.FirstOrDefault(i => i.login_token == token);
                    if (currentUser != null)
                    {
                        var AdList = entities.Ads.Where(x => x.AdCity == currentUser.City && x.IsDeleted == false && x.IsSold == false && (x.AdTitle.Contains(searchQuery) || x.AdDescription.Contains(searchQuery) || x.AdLocality.Contains(searchQuery) || x.AdDivision.Contains(searchQuery)) && x.IsActive == true).OrderByDescending(x => x.CreatedOn).Skip(offSet).Take(7).ToList();
                        if (AdList.Any())
                        {
                            foreach (Ad ad in AdList)
                            {
                                AdWithDemands newAdWithDemand = new AdWithDemands();

                                newAdWithDemand.AdId = ad.AdId;
                                newAdWithDemand.AdTitle = ad.AdTitle;
                                newAdWithDemand.AdCategory = ad.AdCategory;
                                newAdWithDemand.AdLocality = ad.AdLocality;
                                newAdWithDemand.AdCity = ad.AdCity;
                                newAdWithDemand.UserToken = ad.UserToken;
                                newAdWithDemand.VendorName = ad.VendorName;
                                newAdWithDemand.VendorPhoneNumber = ad.VendorPhoneNumber;
                                newAdWithDemand.AdImageOne = ad.AdImageOne;
                                newAdWithDemand.AdSellingPrice = ad.AdSellingPrice;
                                newAdWithDemand.AdHighestBid = ad.AdHighestBid;
                                newAdWithDemand.Filter = ad.Filter;
                                newAdWithDemand.CreatedOn = ad.CreatedOn;
                                newAdWithDemand.NoOfDemands = ad.NoOfDemands;
                                newAdWithDemand.isDisplayPhoneNumber = ad.isDisplayPhoneNumber;
                                User vendor = entities.Users.FirstOrDefault(i => i.login_token == ad.UserToken);
                                newAdWithDemand.UserBadge = vendor.UserBadge;
                                newAdWithDemand.isDemanded = false;

                                if (userAdDemands.Any())
                                {
                                    foreach (AdsDemand a in userAdDemands)
                                    {
                                        if (ad.AdId == a.AdId)
                                            newAdWithDemand.isDemanded = true;
                                    }
                                }
                                adWithDemands.Add(newAdWithDemand);
                            }



                            return Request.CreateResponse(HttpStatusCode.OK, adWithDemands);
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
        public HttpResponseMessage RepostInDifferentCity(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            String city = String.Empty;
            String locality = String.Empty;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
                city = list["City"];
                locality = list["Locality"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    Ad adToBeUpdated = entities.Ads.FirstOrDefault(i => i.AdId == adId);

                    if (adToBeUpdated != null)
                    {
                        Ad newAd = new Ad();
                        newAd = adToBeUpdated;



                        newAd.CreatedOn = DateTime.Now;
                        newAd.IsEligibleForRepost = null;
                        newAd.AdCity = city;
                        newAd.AdLocality = locality;
                        entities.Ads.Add(newAd);
                        entities.SaveChanges();
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");


                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateAdCategory(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            String category = String.Empty;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
                category = list["Category"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    Ad adToBeUpdated = entities.Ads.FirstOrDefault(i => i.AdId == adId);

                    if (adToBeUpdated != null)
                    {
                        adToBeUpdated.AdCategory = category;
                        entities.SaveChanges();
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");


                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateAdInfo(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            int adId = 0;
            String AdTitle = String.Empty;
            String AdDesc = String.Empty;
            if (list.Any())
            {
                adId = Convert.ToInt32(list["AdId"]);
                AdTitle = list["AdTitle"];
                AdDesc = list["AdDesc"];
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    Ad adToBeUpdated = entities.Ads.FirstOrDefault(i => i.AdId == adId);

                    if (adToBeUpdated != null)
                    {
                        adToBeUpdated.AdTitle = AdTitle;
                        adToBeUpdated.AdDescription = AdDesc;
                        entities.SaveChanges();
                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Add not found");


                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }
    }
}
