using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<Unit> Get()
        {
            return _repository.Get();
        }
    }
}
