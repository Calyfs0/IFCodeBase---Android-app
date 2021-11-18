using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IFWebAPI.Models;

namespace IFWebAPI.Controllers
{
    public class CategoryController : ApiController
    {   
       
        public HttpResponseMessage GetCategories()
        {
            try
            {
                using (IFdbEntities entities = new IFdbEntities())
                {
                    var entity = entities.CityMasterTables.ToList();

                    if (entity != null)
                    {
                        List<String> categoryList = entities.Categories.Select(x => x.CategoryName).ToList();
                        if (categoryList.Any())
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, categoryList);
                        }
                        else
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No categories");


                    }

                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,"No categories");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadGateway, ex);
            }
        }
    }
}
