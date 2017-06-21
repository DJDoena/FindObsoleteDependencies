using System;

namespace FindObsoleteDependencies
{
    internal sealed class CsProjsProcessor : ProjsProcessorBase
    {
        protected override String Extension
            => ("csproj");

        protected override void Process(String fullName)
        {
            var processor = new CsProjProcessor(fullName);

            processor.Process();
        }
    }
}