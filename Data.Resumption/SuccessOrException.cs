﻿using System;
using System.Runtime.ExceptionServices;

namespace Data.Resumption
{
    public struct SuccessOrException
    {
        private readonly object _success;
        private readonly Exception _exception;
        public SuccessOrException(object success)
        {
            _success = success;
            _exception = null;
        }

        public SuccessOrException(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            _success = null;
            _exception = exception;
        }

        public object Success
        {
            get
            {
                if (_exception != null) ExceptionDispatchInfo.Capture(_exception).Throw(); // rethrow with original stack trace
                return _success;
            }
        }
        public Exception Exception
        {
            get
            {
                if (_exception == null) throw new InvalidOperationException("Request operation succeeded unexpectedly");
                return _exception;
            }
        }
        public bool HasSuccess => _exception != null;
    }
}