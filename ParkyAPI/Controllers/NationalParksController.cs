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
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : Controller
    {
        private INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks();
            var objDto = new List<NationalParkDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }
            return Ok(objDto);
        }
        [HttpGet("{nationalParkId:int}", Name = "GetNationlPark")]
        public IActionResult GetNationlPark(int nationalParkId)
        {
            NationalPark nationalPark = _npRepo.GetNationalPark(nationalParkId);
            if(nationalPark == null)
            {
                return NotFound();
            }
            NationalParkDto nationalParkDto = _mapper.Map<NationalParkDto>(nationalPark);
            return Ok(nationalParkDto);
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExist(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exsist");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            NationalPark nationalPark = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.CreateNationalPark(nationalPark))
            {
                ModelState.AddModelError("", "Error with saving file");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetNationlPark", new {nationalParkId = nationalPark.Id}, nationalPark);

        }
        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkId,[FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || nationalParkId!=nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExist(nationalParkDto.Name))
            {
                ModelState.AddModelError("", $"National Park Exsist {nationalParkDto.Name}");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            NationalPark nationalPark = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.UpdateNationalPark(nationalPark))
            {
                ModelState.AddModelError("", $"Error with updating file {nationalPark.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_npRepo.NationalParkExist(nationalParkId))
            {
                return NotFound();
            }
            NationalPark nationalPark = _npRepo.GetNationalPark(nationalParkId);
            if (!_npRepo.DeleteNationalPark(nationalPark))
            {
                ModelState.AddModelError("", $"Error with deleting file {nationalPark.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }



    }
}
