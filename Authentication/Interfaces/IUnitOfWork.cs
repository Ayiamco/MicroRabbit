using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Interfaces
{
    public interface IUnitOfWork
    {
        public Task<int> SaveAsync();
        public int Save();
    }
}
