using challenge.Models;
using System;

namespace challenge.Services
{
    public interface ICompensationService
    {
        CompensationDb GetById(String id);
        CompensationDb Create(CompensationDb compensationDb);
    }
}
