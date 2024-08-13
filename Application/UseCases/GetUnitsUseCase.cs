using Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class GetUnitsUseCase
    {
        private readonly IRepository<Unit> _repository;

        public GetUnitsUseCase(IRepository<Unit> repository)
        {
            _repository = repository;
        }

        public async Task<List<Unit>> GetAsync()
        {
            return await _repository.GetAsync();
        }
    }
}
