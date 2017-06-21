using System;
using System.Reflection;

namespace FindObsoleteDependencies
{
    internal interface IAssemblyReflectionManager : IDisposable
    {
        Boolean LoadAssembly(String assemblyPath, String domainName);

        TResult Reflect<TResult>(String assemblyPath, Func<Assembly, TResult> func);

        Boolean UnloadDomain(String domainName);
    }
}