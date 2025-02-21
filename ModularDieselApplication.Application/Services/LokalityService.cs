using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;

namespace ModularDieselApplication.Application.Services
{
    public class LokalityService : ILokalityService
    {
        private readonly ILokalityRepository _lokalityRepository;
        public LokalityService(ILokalityRepository lokalityRepository)
        {
            _lokalityRepository = lokalityRepository;
        }
        public async Task<List<Lokalita>> GetAllLokalityAsync()
        {
            return await _lokalityRepository.GetAllLokalityAsync();
        }
    }
}