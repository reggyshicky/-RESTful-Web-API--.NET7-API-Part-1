using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MagicVilla_VillaAPI.Controllers
{
    //alternative [Route("api/[controller]")]
    [Route("api/VillaAPI")]      //you can decide to use controller as a placeholder for the controller's name or use explicitly the name of the controller in this case it is VillaAPI 
    [ApiController]
    public class VillaAPIController  : ControllerBase  //base class provided by ASP.NET Core for building controllers in a Web API
    {

        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        [HttpGet] //notifies the swagger documentation that this endpoint is a GET endpoint
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas() //ActionResult is a type that can be returned from action methods to represent different kinds of HTTP responses
        {
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }




        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(200, Tpye = typeof(VillaDTO))] // This attributes are used by Swagger to generate API documentation that includes information about the possible responsnes from an API endpoint
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<VillaDTO>(villa));
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO createDTO)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (await _db.Villas.FirstOrDefaultAsync(u=>u.Name.ToLower()==createDTO.Name.ToLower())!= null)
            {
                ModelState.AddModelError("CustomErrors", "Villa Name already exists");
                return BadRequest(ModelState);
            }
            if (createDTO == null)   
            {
                return BadRequest(createDTO);
            }
            //if (villaDTOd.Id > 0)  //when creating the default id is zero
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}


            Villa model = _mapper.Map<Villa>(createDTO);


            //Villa model = new()
            //{
            //    Amenity = createDTO.Amenity,
            //    Details = createDTO.Details,
            //    ImageUrl = createDTO.ImageUrl,
            //    Name = createDTO.Name,
            //    Occupancy = createDTO.Occupancy,
            //    Rate = createDTO.Rate,
            //    Sqft = createDTO.Sqft,
            //};
            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync(); 
            
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);

            //CreatedAtRoute is a helpe method provided by ASP.Net Core for creating an HTTP response with a 201 Created status code. it is used when you want to indicate that a new resource has been created as  a result of a POST request ad you want to provide a way for the client to access or reference this newly created resource

            


        }




        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0 )
            {
                return BadRequest();
            }

            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }




        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }
            
            Villa model = _mapper.Map<Villa>(updateDTO); //short and alternative, Converting villaUpdateDTO to Villa


            //Villa model = new()
            //{
            //    Amenity = updateDTO.Amenity,
            //    Details = updateDTO.Details,
            //    Id = updateDTO.Id,
            //    ImageUrl = updateDTO.ImageUrl,
            //    Name = updateDTO.Name,
            //    Occupancy = updateDTO.Occupancy,
            //    Rate = updateDTO.Rate,
            //    Sqft = updateDTO.Sqft,
            //};

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            return NoContent();
        }




        //Patch. add some nuget packages, 
        //https://jsonpatch.com/
        //Microsoft.AspNetCore.JsonPatch
        //Microsoft.AspNetCore.MVC.NewtonSoftJson
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u=> u.Id == id);
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            //in the above code, inside Map<VillaUpdateDTO>(villa), the output is VillaUpdateDTO and the source is villa
     
            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = _mapper.Map<Villa>(villaDTO);
            
            _db.Villas.Update(model);
            await _db.SaveChangesAsync();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
        //by passing ModelState  to the ApplyTo method, you allow the method to populate the ModelState dictionary with any validation errors that may arise during the patch application

    }
}
