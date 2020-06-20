using System.Runtime.InteropServices;
using Xunit;

namespace JosephGuadagno.AzureHelpers.Storage.Tests
{
    public sealed class OnlyRunOnWindowsFact: FactAttribute
    {
        public OnlyRunOnWindowsFact()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Skip = "This tests only works on Windows";
            }
        }
    }
}