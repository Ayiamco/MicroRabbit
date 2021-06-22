﻿using Authentication.Entities;
using Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext _context)
        {
            this._context = _context;
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
