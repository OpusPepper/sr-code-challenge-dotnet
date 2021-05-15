using challenge.Models;
using System;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface ICompensationRepository
    {
        CompensationDb GetById(String id);
        CompensationDb Add(CompensationDb compensationDb);
        Task SaveAsync();
    }
}