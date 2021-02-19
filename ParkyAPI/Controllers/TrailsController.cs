using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/Trails")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : Controller
    {
        private ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }


        /// <summary>
        /// Get list of the trails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetTrails()
        {
            var objList = _trailRepo.GetTrails();
            var objDto = new List<TrailDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }
            return Ok(objDto);
        }
        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="TrailId">The id of the trail</param>
        /// <returns></returns>
        [HttpGet("{TrailId:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int TrailId)
        {
            Trail Trail = _trailRepo.GetTrail(TrailId);
            if(Trail == null)
            {
                return NotFound();
            }
            TrailDto TrailDto = _mapper.Map<TrailDto>(Trail);
            return Ok(TrailDto);
        }

        [HttpGet("[action]/{nationalParkId:int}")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailInNationalPark(int nationalParkId)
        {
            var objList = _trailRepo.GetTrailsInNationalPark(nationalParkId);
            if (objList == null)
            {
                return NotFound();
            }
            var objDto = new List<TrailDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }


            return Ok(objDto);

        }



        /// <summary>
        /// Create a trail
        /// </summary>
        /// <param name="TrailDto">trail object</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesDefaultResponseType]
        public IActionResult CreateTrail([FromBody] TrailCreateDto TrailDto)
        {
            if (TrailDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_trailRepo.TrailExist(TrailDto.Name))
            {
                ModelState.AddModelError("", "trail Exsist");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Trail Trail = _mapper.Map<Trail>(TrailDto);
            if (!_trailRepo.CreateTrail(Trail))
            {
                ModelState.AddModelError("", "Error with saving file");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetTrail", new {TrailId = Trail.Id}, Trail);

        }
        /// <summary>
        /// Update trail
        /// </summary>
        /// <param name="TrailId">id of trail</param>
        /// <param name="TrailDto">trail object</param>
        /// <returns></returns>
        [HttpPatch("{TrailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(204, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateTrail(int TrailId,[FromBody] TrailUpdateDto TrailDto)
        {
            if (TrailDto == null || TrailId!=TrailDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (_trailRepo.TrailExist(TrailDto.Name))
            {
                ModelState.AddModelError("", $"trail Exsist {TrailDto.Name}");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            Trail Trail = _mapper.Map<Trail>(TrailDto);
            if (!_trailRepo.UpdateTrail(Trail))
            {
                ModelState.AddModelError("", $"Error with updating file {Trail.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        /// <summary>
        /// Delete trail
        /// </summary>
        /// <param name="TrailId"></param>
        /// <returns></returns>
        [HttpDelete("{TrailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int TrailId)
        {
            if (!_trailRepo.TrailExist(TrailId))
            {
                return NotFound();
            }
            Trail Trail = _trailRepo.GetTrail(TrailId);
            if (!_trailRepo.DeleteTrail(Trail))
            {
                ModelState.AddModelError("", $"Error with deleting file {Trail.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }



    }
}
