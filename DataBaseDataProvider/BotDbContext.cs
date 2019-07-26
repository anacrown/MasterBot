using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using BotBase.BotInstance;
using Microsoft.EntityFrameworkCore;
using DataFrameModel = DataBaseDataProvider.Model.DataFrameModel;
using ExceptionModel = DataBaseDataProvider.Model.ExceptionModel;
using LaunchModel = DataBaseDataProvider.Model.LaunchModel;
using LaunchSettingsModel = DataBaseDataProvider.Model.LaunchSettingsModel;

namespace CodenjoyBot
{
    public sealed class BotDbContext : DbContext
    {
        public BotDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<LaunchModel> LaunchModels { get; set; }
        public DbSet<DataFrameModel> DataFrameModels { get; set; }
        public DbSet<ExceptionModel> ExceptionModels { get; set; }
        public DbSet<LaunchSettingsModel> LaunchSettingsModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = CodenjoyDataStore.db");
        }
    }

    internal static class BotInstanceExtention
    {
        public static LaunchSettingsModel GetSettings(this BotInstance botInstance)
        {
            return new LaunchSettingsModel
            {
                CreateTime = DateTime.Now,
                HashCode = botInstance.GetHashCode(),
                Data = GetData(botInstance),
                Title = $"{botInstance.Name} {botInstance.Title}"
            };
        }

        public static BotInstance FromSettings(LaunchSettingsModel settingsModel)
        {
            var botInstance = FromData(settingsModel.Data);
            botInstance.SettingsId = settingsModel.Id;
            return botInstance;
        }

        private static byte[] GetData(this BotInstance botInstance)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, botInstance);
                return stream.ToArray();
            }
        }
        private static BotInstance FromData(byte[] data)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream(data))
            {
                stream.Position = 0;
                return (BotInstance)formatter.Deserialize(stream);
            }
        }
    }
}