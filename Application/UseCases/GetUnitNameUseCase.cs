using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public string? GetName(int id)
        {
            return _repository.Get(id).Name;
        }
    }
}
