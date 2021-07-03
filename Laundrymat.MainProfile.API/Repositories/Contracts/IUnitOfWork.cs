using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Laundromat.MainProfile.API.Repositories
{
    public interface IUnitOfWork
    {
        public ILaundryRepo LaundryRepo { get;  }
        public Task<int> SaveAsync();
        public int Save();
    }
}
