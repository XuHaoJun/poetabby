using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RCB.JavaScript.Models
{
  public class DbConfig
  {
    public string Server { get; set; }
    public string User { get; set; }
    public string Pass { get; set; }
    public string Port { get; set; }
    public string Database { get; set; }
    private string _databaseUrl;

    public DbConfig(string databaseUrl)
    {
      _databaseUrl = databaseUrl;
      _databaseUrl.Replace("//", "");
      char[] delimiterChars = { '/', ':', '@', '?' };
      string[] strConn = _databaseUrl.Split(delimiterChars);
      strConn = strConn.Where(x => !string.IsNullOrEmpty(x)).ToArray();
      User = strConn[1];
      Pass = strConn[2];
      Server = strConn[3];
      Database = strConn[5];
      Port = strConn[4];
    }

    public string GetDatabaseUrl()
    {
      return _databaseUrl;
    }

    public string GetDefaultConnectionString()
    {
      return "host=" + Server + ";port=" + Port + ";database=" + Database + ";uid=" + User + ";pwd=" + Pass + ";sslmode=Require;Trust Server Certificate=true;Timeout=1000";
    }

    static public string GetDefaultConnectionString(string databaseUrl)
    {
      return new DbConfig(databaseUrl).GetDefaultConnectionString();
    }
  }

  public class PoeDbContext : DbContext
  {
    public DbSet<PoeCharacterModel> Characters { get; set; }

    public DbSet<PoeLeagueModel> Leagues { get; set; }

    public static readonly ILoggerFactory MyLoggerFactory
        = LoggerFactory.Create(builder => { builder.AddConsole(); });

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseLoggerFactory(MyLoggerFactory);
      //Get Database Connection 
      //Environment.SetEnvironmentVariable("DATABASE_URL", "postgres://ojunflcdtkendq:be88fc41989efe90fda30380a6dae8ec9259cc19f237f11135b68a52371a6ce5@ec2-54-235-146-51.compute-1.amazonaws.com:5432/d8lhbkcpmedcej");
      string databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
      string connectionString;
      if (String.IsNullOrWhiteSpace(databaseUrl))
      {
        string hostname;
        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
        {
          hostname = "host.docker.internal";
        }
        else
        {
          hostname = "localhost";
        }
        connectionString = $"Host={hostname};Database=poetabby;Username=postgres;Password=";
      }
      else
      {
        connectionString = DbConfig.GetDefaultConnectionString(databaseUrl);
      }
      optionsBuilder.UseNpgsql(connectionString);
      optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<PoeCharacterModel>()
          .HasIndex(c => c.CharacterId)
          .IsUnique();
      modelBuilder.Entity<PoeCharacterModel>()
          .HasIndex(c => new { c.CharacterName, c.AccountName })
          .IsUnique();
      modelBuilder.Entity<PoeCharacterModel>()
          .HasIndex(c => c.Pob)
          .HasMethod("gin");
      modelBuilder.Entity<PoeCharacterModel>()
          .HasIndex(c => c.Items)
          .HasMethod("gin");
      modelBuilder.Entity<PoeCharacterModel>()
          .HasIndex(c => c.CountAnalysis)
          .HasMethod("gin");
      // modelBuilder.Entity<PoeCharacterModel>()
      //     .Property(c => c.Depth)
      //     .HasDefaultValue(new PoeDepth() { Solo = 0, Default = 0 });
      int ladderSize = 15000;
      modelBuilder.Entity<PoeCharacterModel>()
          .Property(c => c.Rank)
          .HasDefaultValue(ladderSize + 1);


      modelBuilder.Entity<PoeLeagueModel>()
          .HasIndex(l => l.LeagueId)
          .IsUnique();
    }
  }
}