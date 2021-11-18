using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IFWebAPI.Models;

namespace IFWebAPI.Controllers
{
    public class CityController : ApiController
    {
        
        public HttpResponseMessage GetCities()
        {
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    var entity = entities.CityMasterTables.ToList();

                    if (entity != null)
                    {
                        List<String> cityList = entities.CityMasterTables.Select(x => x.CityName).ToList();

                        if (cityList.Any())
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, cityList);
                        }
                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Cities");

                    }
                        
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,"No Cities");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway,ex);
            }
        }
    }
}
