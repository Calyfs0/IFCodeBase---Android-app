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
    public class LocalityController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetLocalities(JObject jsonResult)
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
                    var entity = entities.CityMasterTables.ToList();

                    if (entity != null)
                    {
                        User user = entities.Users.FirstOrDefault(p => p.login_token == token);
                        if (user != null)
                        {
                            List<String> localitiesList = entities.Localities.Where(j => j.LocalityCity == user.City).Select(x => x.LocalityName).ToList();

                            if (localitiesList.Any())
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, localitiesList);
                            }
                            else
                                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Localities found");


                        }
                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound,"No Localities found");

                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Localities found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }
    }
}
