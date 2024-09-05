using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion(1)]
    [ApiVersion(2)]
    [Route("api/v{version:apiVersion}/cities")]
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


        /// <summary>
        /// Get a city by id
        /// </summary>
        /// <param name="cityId">The id of the city to get</param>
        /// <param name="includePointsOfInterest">Whether or not to include points of interest</param>
        /// <returns>A city with or without points of interest</returns>
        /// <response code ="200">Returns the requested city</response>
        [HttpGet("{cityId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCity(
            int cityId, 
            bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(cityId, includePointsOfInterest);
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
