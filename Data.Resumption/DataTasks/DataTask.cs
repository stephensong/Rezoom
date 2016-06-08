﻿using System;
using System.Collections.Generic;

namespace Data.Resumption.DataTasks
{
    public static class DataTask
    {
        public static IDataTask<TOut> Select<TIn, TOut>(this IDataTask<TIn> bound, Func<TIn, TOut> mapping)
            => new MapTask<TIn, TOut>(bound, mapping);
        public static IDataTask<TOut> SelectMany<TPending, TOut>(this IDataTask<TPending> bound, Func<TPending, IDataTask<TOut>> continuation)
            => new BindTask<TPending, TOut>(bound, continuation);
        public static IDataTask<T> Return<T>(T value) => new ReturnTask<T>(value);
        public static IDataTask<TOut> Apply<T, TOut>(this IDataTask<Func<T, TOut>> functionTask, IDataTask<T> inputTask)
            => new ApplyTask<T, TOut>(functionTask, inputTask);
        public static IDataTask<TSum> Sum<T, TSum>(this IEnumerable<IDataTask<T>> tasks, TSum initial, Func<TSum, T, TSum> add)
            => new SumTask<T, TSum>(tasks, initial, add);
    }
}