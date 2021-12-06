namespace Com.Apdcomms.StormPipeline.Tests
{
    using Com.Apdcomms.StormPipeline.Extensions;
    using Com.Apdcomms.StormPipeline.Parsing;
    using Com.Apdcomms.StormPipeline.Queue;
    using FakeItEasy;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class TestBootstrapper
    {
        public INotificationQueue FakeNotificationQueue { get; } = A.Fake<INotificationQueue>();

        public ILogger FakeLogger { get; } = A.Fake<ILogger>();

        public MappingConfig FakeMappingConfig { get; } = new MappingConfig();

        public void Bind(IServiceCollection serviceCollection)
        {
            SetupMappedIndexes();
            var mappingConfigOptions = Options.Create(this.FakeMappingConfig);
            serviceCollection.AddSingleton(mappingConfigOptions);

            serviceCollection.AddStormPipeline();
            serviceCollection.AddSingleton(FakeNotificationQueue);
            serviceCollection.AddSingleton(FakeLogger);            
        }

        public static string GenerateString()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 2050).Select(s => s[random.Next(s.Length)]).ToArray());
        }        

        private void SetupMappedIndexes()
        {
            StreamReader r = new StreamReader("mapping.json");
            string jsonData = r.ReadToEnd();            
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> dictornary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, int>>>>(jsonData);

            dictornary.TryGetValue("Mappings", out Dictionary<string, Dictionary<string, int>> messages);

            messages.TryGetValue("PRI", out var PRI_Message_Indexes);
            
            messages.TryGetValue("CI2", out var CI2_Message_Indexes);
            
            messages.TryGetValue("DI", out var DI_Message_Indexes);
            
            messages.TryGetValue("SDA", out var SDA_Message_Indexes);
           
            messages.TryGetValue("UI", out var UI_Message_Indexes);
            
            messages.TryGetValue("UR", out var UR_Message_Indexes);
            
            messages.TryGetValue("PSI", out var PSI_Message_Indexes);
            
            messages.TryGetValue("CI", out var CI_Message_Indexes);
            
            messages.TryGetValue("CR", out var CR_Message_Indexes);
            
            messages.TryGetValue("DCO", out var DCO_Message_Indexes);

            messages.TryGetValue("RRA", out var RRA_Message_Indexes);

            messages.TryGetValue("RILA", out var RILA_Message_Indexes);

            messages.TryGetValue("CR2", out var CR2_Message_Indexes);

            messages.TryGetValue("DDO", out var DDO_Message_Indexes);

            messages.TryGetValue("RSC", out var RSC_Message_Indexes);

            messages.TryGetValue("RFS", out var RFS_Message_Indexes);

            messages.TryGetValue("PII", out var PII_Message_Indexes);

            messages.TryGetValue("PILI", out var PILI_Message_Indexes);

            messages.TryGetValue("DIL", out var DIL_Message_Indexes);

            messages.TryGetValue("DR", out var DR_Message_Indexes);

            messages.TryGetValue("RRS", out var RRS_Message_Indexes);

            messages.TryGetValue("AL",  out var AL_Message_Indexes);

            messages.TryGetValue("CSU", out var CSU_Message_Indexes);

            messages.TryGetValue("CLI", out var CLI_Message_Indexes);

            messages.TryGetValue("FWA", out var FWA_Message_Indexes);

            messages.TryGetValue("CAI", out var CAI_Message_Indexes);

            messages.TryGetValue("ZAI", out var ZAI_Message_Indexes);

            messages.TryGetValue("DAI", out var DAI_Message_Indexes);

            FakeMappingConfig.Add("PRI", PRI_Message_Indexes);
            FakeMappingConfig.Add("CI2", CI2_Message_Indexes);
            FakeMappingConfig.Add("DI", DI_Message_Indexes);
            FakeMappingConfig.Add("SDA", SDA_Message_Indexes);
            FakeMappingConfig.Add("UI", UI_Message_Indexes);
            FakeMappingConfig.Add("UR", UR_Message_Indexes);
            FakeMappingConfig.Add("PSI", PSI_Message_Indexes);
            FakeMappingConfig.Add("CI", CI_Message_Indexes);
            FakeMappingConfig.Add("CR", CR_Message_Indexes);
            FakeMappingConfig.Add("DCO", DCO_Message_Indexes);
            FakeMappingConfig.Add("RRA", RRA_Message_Indexes);
            FakeMappingConfig.Add("RILA", RILA_Message_Indexes);
            FakeMappingConfig.Add("CR2", CR2_Message_Indexes);
            FakeMappingConfig.Add("DDO", DDO_Message_Indexes);
            FakeMappingConfig.Add("RSC", RSC_Message_Indexes);
            FakeMappingConfig.Add("RFS", RFS_Message_Indexes);
            FakeMappingConfig.Add("PII", PII_Message_Indexes);
            FakeMappingConfig.Add("PILI", PILI_Message_Indexes);
            FakeMappingConfig.Add("DIL", DIL_Message_Indexes);
            FakeMappingConfig.Add("DR", DR_Message_Indexes);
            FakeMappingConfig.Add("RRS", RRS_Message_Indexes);
            FakeMappingConfig.Add("AL", AL_Message_Indexes);
            FakeMappingConfig.Add("CSU", CSU_Message_Indexes);
            FakeMappingConfig.Add("CLI", CLI_Message_Indexes);
            FakeMappingConfig.Add("FWA", FWA_Message_Indexes);
            FakeMappingConfig.Add("CAI", CAI_Message_Indexes);
            FakeMappingConfig.Add("ZAI", ZAI_Message_Indexes);
            FakeMappingConfig.Add("DAI", DAI_Message_Indexes);
        }
    }
}
