﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cadmean.RPC.ASP
{
    public class FunctionController : ControllerBase
    {
        protected FunctionCall Call;

        [HttpPost]
        public async Task<FunctionOutput> Post()
        {
            Call = await GetFunctionCall();
            
            var callMethod = GetCallMethod();
            if (!CallMethodIsValid(callMethod))
                return FunctionOutput.WithError(1);

            var args = GetArguments(callMethod);

            object result;
            try
            {
                dynamic task = (Task) callMethod.Invoke(this, args);
                await task;
                result = task.GetAwaiter().GetResult();
            }
            catch (FunctionException ex)
            {
                return FunctionOutput.WithError(ex.Code);
            }
            catch 
            {
                return FunctionOutput.WithError(3);
            }
            
            var output = FunctionOutput.WithResult(result);
            
            
            
            return output;
        }

        private bool CallMethodIsValid(MethodInfo methodInfo)
        {
            return methodInfo != null && !methodInfo.IsAbstract;
        }

        private async Task<FunctionCall> GetFunctionCall()
        {
            using var r = new StreamReader(Request.Body);
            var str = await r.ReadToEndAsync();
            return (FunctionCall) JsonConvert.DeserializeObject(str, typeof(FunctionCall));
        }

        private MethodInfo GetCallMethod()
        {
            var rpcService = HttpContext.Items["rpcService"] as RpcService ?? throw new RpcServerException("RPC service not found");
            var functionName = HttpContext.Items["functionName"] as string;
            var cached = rpcService.GetCachedFunction(functionName);

            if (cached != null)
                return cached;
            
            var t = GetType();
            var methodInfo = t.GetMethod("OnCall", 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            rpcService.CacheFunction(functionName, methodInfo);
            return methodInfo;
        }

        private object[] GetArguments(MethodInfo callMethod)
        {
            var parameters = callMethod.GetParameters();
            object[] args;

            if (Call.Arguments != null)
            {
                args= new object[parameters.Length];
                for (int i = 0; i < Math.Min(parameters.Length, Call.Arguments.Length); i++)
                {
                    var arg = Call.Arguments[i];
                    if (arg is JObject json)
                    {
                        args[i] = json.ToObject(parameters[i].ParameterType);
                    }
                    else
                    {
                        args[i] = arg;
                    }
                }
            }
            else
            {
                args = new object[0];
            }

            return args;
        }


        private object ExecuteFunctionAsync(MethodInfo callMethod, object[] args)
        {
            
        }

        private void AddAuthenticationMetaData(FunctionOutput result)
        {
            
        }
    }
}