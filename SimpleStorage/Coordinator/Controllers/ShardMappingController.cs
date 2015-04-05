using System.Web.Http;

namespace Coordinator.Controllers
{
    public class ShardMappingController : ApiController
    {
        public ShardMappingController(IConfiguration configuration)
        {
        }

        public int Get(string id)
        {
            return 0;
        }
    }
}