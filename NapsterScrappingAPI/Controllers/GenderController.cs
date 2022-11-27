using MediatR;
using Microsoft.AspNetCore.Mvc;
using NapsterScrappingBusiness.Dtos;
using NapsterScrappingBusiness.Gender;
using NapsterScrappingBusiness.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NapsterScrappingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController : ControllerBase
    {
        protected IMediator Mediator;
        private readonly ICacheManager _cache;
        public GenderController(IMediator mediator, ICacheManager cache) { Mediator = mediator; _cache = cache; }

        /// <summary>
        /// Detalle de géneros y subgeneros por género en Napster
        /// </summary>
        /// <returns>Lista con los detalles de género y subgénero</returns>
        [HttpGet]
        public async Task<IEnumerable<GenderSubGenreDto>> Get()
        {
            return await _cache.GenderSubGenreList();
        }

        [HttpGet]
        [Route("artist/{name}")]
        public async Task<IEnumerable<ArtistByGendersDto>> GetArtistByGenders(string name)
        {
            return await Mediator.Send(new ArtistByGender.Query { Name = name });
        }
    }
}
