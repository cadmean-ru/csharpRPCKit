﻿namespace Cadmean.RPC
{
    public struct FunctionOutput<TResult>
    {
        public int Error { get; set; }
        public TResult Result { get; set; }

        public static FunctionOutput WithError(int error)
        {
            return new FunctionOutput
            {
                Error = error,
                Result = null,
            };
        }
        
        public static FunctionOutput WithResult(TResult result)
        {
            return new FunctionOutput
            {
                Error = 0,
                Result = result,
            };
        }
    }
    
    public struct FunctionOutput
    {
        public int Error { get; set; }
        public object Result { get; set; }
        
        public static FunctionOutput WithError(int error)
        {
            return new FunctionOutput
            {
                Error = error,
                Result = null,
            };
        }
        
        public static FunctionOutput WithResult(object result)
        {
            return new FunctionOutput
            {
                Error = 0,
                Result = result,
            };
        }
    }
}