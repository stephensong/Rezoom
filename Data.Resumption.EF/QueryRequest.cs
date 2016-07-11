﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Data.Resumption.Services;
using EntityFramework.Extensions;

namespace Data.Resumption.EF
{
    public class QueryRequest<TContext, T> : ContextRequest<TContext, List<T>>
        where TContext : DbContext
        where T : class
    {
        private readonly Func<TContext, IQueryable<T>> _query;

        public QueryRequest(Func<TContext, IQueryable<T>> query)
        {
            _query = query;
        }

        public override bool Mutation => false;
        public override bool Idempotent => true;

        protected override Func<Task<List<T>>> Prepare(TContext db)
        {
            var future = _query(db).AsNoTracking().Future();
            return () => Task.FromResult(future.ToList()); // unfortunately, futures don't support ToListAsync
        }
    }
}