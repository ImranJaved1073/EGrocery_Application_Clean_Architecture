using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BrandService
    {
        private readonly IRepository<Brand> _brandRepository;

        public BrandService(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<string> GetBrandNameAsync(int id)
        {
            var brand = await _brandRepository.GetAsync(id);
            return brand.BrandName;
        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            return await _brandRepository.GetAsync();
        }
    }
}
