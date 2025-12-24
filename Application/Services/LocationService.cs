using Application.DTOs;
using Application.Interfaces;
using Dominio.Interfaces;

namespace Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllAsync();
            return locations.Select(l => new LocationDto
            {
                Id = l.Id,
                Name = l.Name
            });
        }
    }
}