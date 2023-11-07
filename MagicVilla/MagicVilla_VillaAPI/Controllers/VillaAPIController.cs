using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]      //alternative [Route("api/[controller]")]   
    [ApiController]
    public class VillaAPIController  : ControllerBase  //base class provided by ASP.NET Core for building controllers in a Web API
    {
        [HttpGet] //notifies the swagger documentation that this endpoint is a GET endpoint
        public IEnumerable<Villa> GetVillas()
        {
            return new List<Villa> {
            new Villa{Id=1,Name="Pool View"},
            new Villa{Id=2,Name="Beach View" }
            };
        }
    }
}
