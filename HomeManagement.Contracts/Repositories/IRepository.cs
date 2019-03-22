﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HomeManagement.Contracts.Repositories
{
    public interface IRepository : IUnitOfWork
    {
        int Count();

        void Remove(int id);

        IDbTransaction CreateTransaction();
    }

    public interface IRepository<T> : IRepository
    {
        IQueryable<T> All { get; }

        void Add(T entity);

        Task AddAsync(T entity);

        void Remove(T entity);

        void Update(T entity);

        T GetById(int id);

        T FirstOrDefault();

        T FirstOrDefault(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetAll();

        bool Exists(T entity);

        IEnumerable<T> Where(Expression<Func<T, bool>> predicate);

        decimal Sum(Expression<Func<T, int>> selector, Expression<Func<T, bool>> predicate = null);

        decimal Sum(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate = null);

        int Count(Expression<Func<T, bool>> predicate);
    }
}
