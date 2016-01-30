using System.Collections.Generic;
using System.Web.Http;

namespace SharpTsdb
{
    [RoutePrefix("api/testing")]
    public class RoutedController : ApiController
    {
        [Route("getall")]
        public IEnumerable<string> GetAllItems()
        {
            return new string[] { "value1", "value2" };
        }
    }
}