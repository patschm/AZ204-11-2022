using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Hosting;

namespace ACME.Tests.Integration
{
    public class TestWebApplicationFactory<T>: WebApplicationFactory<T> where T: class
    {
        private string settingsFile = @"..\..\..\Properties\launchSettings.json";

        private void InitSettings()
        {
            if (!File.Exists(settingsFile))
            {
                return;
            }
            using (var file = File.OpenText(settingsFile))
            {
                var reader = new JsonTextReader(file);
                var jObject = JObject.Load(reader);

                var variables = jObject
                    .GetValue("profiles")
                    .SelectMany(profiles => profiles.Children())
                    .SelectMany(profile => profile.Children<JProperty>())
                    .Where(prop => prop.Name == "environmentVariables")
                    .SelectMany(prop => prop.Value.Children<JProperty>())
                    .ToList();

                foreach (var variable in variables)
                {
                    Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
                }
            }
        }
        protected override IHostBuilder CreateHostBuilder()
        {
            InitSettings();
            return base.CreateHostBuilder()!;
        }
       
    }
}
