using System.Web;
using System.Web.Http;
using DataAccess.Import;

namespace Api.Controllers
{
    //[OAuth2Authentication]
    //[Authorize(Roles = "Admin")]
    [Route("api/import/csv")]
    public class ImportController : ApiController
    {
        private const string CsvFileName = "Team DB  Profile - Profiles.csv";

        private UserImport importer;

        public ImportController(UserImport importer)
        {
            this.importer = importer;
        }

        [HttpGet]
        public string Get()
        {
            importer.ImportAllUsers(string.Format("{0}{1}", HttpRuntime.AppDomainAppPath, CsvFileName));
            return "OK";
        }
    }
}
