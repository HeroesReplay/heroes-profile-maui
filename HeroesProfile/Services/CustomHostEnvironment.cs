
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

using System.IO;
using System.Reflection;

namespace MauiApp2.Services
{
    public class CustomHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "HeroesProfile.UI";
        public string ContentRootPath { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}