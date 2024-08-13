using Domain;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class GetUnitNameUseCase
    {
        private readonly IRepository<Unit> _repository;

        public GetUnitNameUseCase(IRepository<Unit> repository)
        {
            _repository = repository;
        }

        public async Task<string?> GetNameAsync(int id)
        {
            var unit = await _repository.GetAsync(id);
            return unit?.Name;
        }
    }
}
