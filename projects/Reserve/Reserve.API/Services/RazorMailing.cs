using System.IO;
using System.Reflection;
using RazorLight;

namespace Reserve.API.Services
{
    public class RazorMailing
    {
        public RazorLightEngine Engine;
        
        public RazorMailing()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Path.Combine(location ?? string.Empty, "Assets"))
                .Build();
        }
    }
}