namespace FindObsoleteDependencies
{
    using System;

    internal sealed class VcxProjsProcessor : ProjsProcessorBase
    {
        protected override String Extension
            => ("vcxproj");

        protected override void Process(String fullName)
        {
            var processor = new VcxProjProcessor(fullName);

            processor.Process();
        }
    }
}