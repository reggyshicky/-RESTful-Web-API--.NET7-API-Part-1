using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //alternative [Route("api/[controller]")]
    [Route("api/VillaAPI")]      //you can decide to use controller as a placeholder for the controller's name or use explicitly the name of the controller in this case it is VillaAPI 
    [ApiController]
    public class VillaAPIController  : ControllerBase  //base class provided by ASP.NET Core for building controllers in a Web API
    {
        [HttpGet] //notifies the swagger documentation that this endpoint is a GET endpoint
        public IEnumerable<VillaDTO> GetVillas()
        {
            return new List<VillaDTO> {
            new VillaDTO{Id=1,Name="Pool View"},
            new VillaDTO{Id=2,Name="Beach View" }
            };
        }
    }
}
