using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Repositories
{
    public interface IUnitOfWork
    {
        public Task<int> SaveAsync();
        public int Save();
    }
}
