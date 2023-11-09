using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MagicVilla_VillaAPI.Controllers
{
    //alternative [Route("api/[controller]")]
    [Route("api/VillaAPI")]      //you can decide to use controller as a placeholder for the controller's name or use explicitly the name of the controller in this case it is VillaAPI 
    [ApiController]
    public class VillaAPIController  : ControllerBase  //base class provided by ASP.NET Core for building controllers in a Web API
    {
        [HttpGet] //notifies the swagger documentation that this endpoint is a GET endpoint
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas() //ActionResult is a type that can be returned from action methods to represent different kinds of HTTP responses
        {
            return Ok(VillaStore.villaList);
        }
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(200, Tpye = typeof(VillaDTO))] // This attributes are used by Swagger to generate API documentation that includes information about the possible responsnes from an API endpoint
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<VillaDTO> Create([FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id > 0)  //when creating the default id is zero
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);
            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);

            //CreatedAtRoute is a helpe method provided by ASP.Net Core for creating an HTTP response with a 201 Created status code. it is used when you want to indicate that a new resource has been created as  a result of a POST request ad you want to provide a way for the client to access or reference this newly created resource
        }
    }
}
