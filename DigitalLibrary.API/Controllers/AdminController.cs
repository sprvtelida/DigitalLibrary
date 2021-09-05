using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibrary.API.Controllers
{
    //[ApiController]
    [Route("api/admin")]
    public class AdminController : Controller//ControllerBase
    {
        [Route("admin")]
        [HttpGet]
        [Authorize(Policy = DigitalLibraryConstants.Policies.Administration)]
        public string Admin()
        {
            return "admin";
        }

        [Route("moder")]
        [HttpGet]
        [Authorize(Policy = DigitalLibraryConstants.Policies.Moderation)]
        public string Moder()
        {
            return "moder";
        }

        [Route("user")]
        [HttpGet]
        public string User()
        {
            return "user";
        }
    }
}
