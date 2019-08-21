using AspNetCore.AsyncInitialization;

using System.Threading.Tasks;

namespace Simple.Models
{
    public class MyAppInitializer : IAsyncInitializer
    {
        public MyAppInitializer(IFoo foo, IBar bar)
        {
        }

        public async Task InitializeAsync()
        {
            // Initialization code here
        }
    }

    public interface IFoo
    {
    }
    public interface IBar
    {
    }
}
