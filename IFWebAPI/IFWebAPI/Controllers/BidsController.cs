using IFWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IFWebAPI.Controllers
{
    public class BidsController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetThreeHighestBidForAd(JObject jsonResult)
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
                    List<AdBid> threeHighestBids = entities.AdBids.Where(i => i.AdId == adId).OrderByDescending(i => i.BidAmount).Take(3).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, threeHighestBids);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }

        }

        [HttpPost]
        public HttpResponseMessage GetAdsWithUserBids(JObject jsonResult)
        {
            Dictionary<String, String> list = JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonResult.ToString());
            String token = string.Empty;
            int offset = 0;

            if (list.Any())
            {
                token = list["Token"];
                offset = Convert.ToInt32(list["Offset"]);
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request");

            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    //Joining Ads table and AdsBids table to get the data for the highest bids per ad placed by user
                    var adWithBids = entities.AdBids.Where(x => x.UserToken == token).GroupBy(x => x.AdId, (key, g) => g.OrderByDescending(e => e.BidAmount).FirstOrDefault())
                        .Join(entities.Ads, x => x.AdId, y => y.AdId, (x, y) => new
                        {
                            BidAmount = x.BidAmount,
                            BidDate = x.BidDate,
                            AdId = y.AdId,
                            AdTitle = y.AdTitle,
                            AdImageOne = y.AdImageOne,
                            AdLocality = y.AdLocality,
                            CreatedOn = y.CreatedOn,
                            AdSellingPrice = y.AdSellingPrice,
                            AdHighestBid = y.AdHighestBid,
                            UserToken = y.UserToken,
                            VendorPhoneNumber = y.VendorPhoneNumber
                        })
                            .OrderByDescending(x => x.BidDate).Skip(offset).Take(10).ToList();


                    List<UserAdsWithHisHighestBid> userAdsWithHisHighestBidList = new List<UserAdsWithHisHighestBid>();

                    foreach (var v in adWithBids)
                    {
                        UserAdsWithHisHighestBid newUser = new UserAdsWithHisHighestBid()
                        {
                            AdId = v.AdId,
                            AdTitle = v.AdTitle,
                            AdImageOne = v.AdImageOne,
                            AdLocality = v.AdLocality,
                            CreatedOn = v.CreatedOn,
                            AdSellingPrice = v.AdSellingPrice,
                            AdHighestBid = v.AdHighestBid,
                            BidAmount = v.BidAmount,
                            BidDate = v.BidDate,
                            VendorPhoneNumber = v.VendorPhoneNumber,
                            UserToken = v.UserToken
                        };
                        userAdsWithHisHighestBidList.Add(newUser);
                    }

                    if (userAdsWithHisHighestBidList.Any())
                        return Request.CreateResponse(HttpStatusCode.OK, userAdsWithHisHighestBidList);
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Bids Found");


                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }

    }
}
