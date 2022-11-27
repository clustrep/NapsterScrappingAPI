using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NapsterScrappingBusiness.Dtos;
using NapsterScrappingBusiness.Gender;
using NapsterScrappingBusiness.Interfaces;
using NapsterScrappingBusiness.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NapsterScrappingInfrastructure.Cache
{
    public class CacheManager : ICacheManager
    {
        protected IMediator Mediator;
        private readonly IMemoryCache _cache;
        private readonly int _shortTime = 100; // segundos
        private readonly int _longTime = 20; // minutos
        public CacheManager(IMemoryCache cache, IMediator mediator)
        {
            _cache = cache;
            Mediator = mediator;
        }
        public async Task<List<GenderSubGenreDto>> GenderSubGenreList()
        {
            MemoryCacheEntryOptions opt = new MemoryCacheEntryOptions();
            opt.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_longTime);
            // Creamos o traemos la lista, buscando en el cache por Key
            var cacheEntry = _cache.GetOrCreateExclusiveAsync(ParameterKeys.GenderSubGenreList, async () =>
            {
                var genderSubGenreDto = await Mediator.Send(new GendersSubGenresList.Query());
                return genderSubGenreDto;
            }, opt);
            return await cacheEntry;
        }
    }
}
