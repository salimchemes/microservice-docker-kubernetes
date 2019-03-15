using System;
using Serilog;

namespace VillaSport.MicroService.Repositories
{
    public class VillaSportRepository : IVillaSportRepository
    { 
      public string GetWelcomeMessage()
        {
            Log.Information("log info");
            Log.Warning("log warning");
            Log.Error("log error");
            Log.Fatal("log fatal");
            return $"VillaSport MS response :) from {Environment.MachineName}";
        }
    }
}
