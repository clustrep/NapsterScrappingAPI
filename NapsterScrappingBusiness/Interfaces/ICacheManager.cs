using NapsterScrappingBusiness.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NapsterScrappingBusiness.Interfaces
{
    public interface ICacheManager
    {
        Task<List<GenderSubGenreDto>> GenderSubGenreList();
    }
}
