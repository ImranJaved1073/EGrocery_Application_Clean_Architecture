using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public string getBrandName(int id)
        {
            return _brandRepository.Get(id).BrandName;
        }

        public List<Brand> getAllBrands()
        {
            return _brandRepository.Get();
        }
    }
}
