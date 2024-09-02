using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;
        public CitiesController(ICityInfoRepository cityInfoRepository,
            IMapper mapper) { 
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _cityInfoRepository = cityInfoRepository ?? 
                throw new ArgumentNullException(nameof(cityInfoRepository));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
            string? name, string? searchQuery,int pageNumber=1, int pageSize=10)
        {
            if(pageSize > maxCitiesPageSize)
            {
                pageSize=maxCitiesPageSize;
            }

            var (cityEntities,paginationMetadata) = await _cityInfoRepository
                .GetCitiesAsync(name, searchQuery,pageNumber,pageSize);

            Response.Headers.Add("X-pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
            //return Ok(_citiesDataStore.Cities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(
            int id, 
            bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id,includePointsOfInterest);
            if(city == null)
            {
                return NotFound();
            
            }

            if(includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }

    }
}
