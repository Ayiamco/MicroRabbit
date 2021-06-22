﻿using Authentication.Entities;
using Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Authentication.Infrastructure
{
    public abstract class GenericQueries<T1, T2> : IGenericQueries<T1, T2> where T1 : class
    {
        protected readonly ApplicationDbContext _context;

        public GenericQueries(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Create(T1 entity)
        {
            await _context.Set<T1>().AddAsync(entity);
            return;
        }
        public async Task<T1> Read(T2 id)
        {
            return await _context.Set<T1>().FindAsync(id);
        }

        public void Delete(T1 entity)
        {
            _context.Set<T1>().Remove(entity);
        }
        public void Update(T1 entity)
        {
            _context.Set<T1>().Update(entity);
        }


        public IEnumerable<T1> GetAll(int pageSize = 0, int currentPage = 0)
        {
            return _context.Set<T1>().Skip(currentPage * pageSize).Take(pageSize).AsQueryable().ToList();
        }

        public IEnumerable<T1> Find(Expression<Func<T1, bool>> func_predicate)
        {
            return _context.Set<T1>().Where(func_predicate);
        }
    }
}
