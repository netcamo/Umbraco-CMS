using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Umbraco.Cms.Tests.Integration.Testing;

public class SqliteTestDatabase : ITestDatabase
{
    private readonly IOptionsMonitor<ConnectionStrings> _connectionStrings;
    private readonly IUmbracoDatabaseFactory _databaseFactory;
    private Guid? _key;

    public SqliteTestDatabase(IOptionsMonitor<ConnectionStrings> connectionStrings, IUmbracoDatabaseFactory databaseFactory)
    {
        _connectionStrings = connectionStrings;
        _databaseFactory = databaseFactory;
        _key = Guid.NewGuid();
    }

    public ConnectionStrings Initialize()
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = $"{GetAbsolutePath()}",
            Mode = SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true,
            Pooling = false, // When pooling true, files kept open after connections closed, bad for cleanup.
            Cache = SqliteCacheMode.Shared,
        };

        var connectionStrings = new ConnectionStrings
        {
            ConnectionString = builder.ConnectionString,
            ProviderName = "Microsoft.Data.Sqlite",
        };

        _connectionStrings.CurrentValue.ConnectionString = connectionStrings.ConnectionString;
        _connectionStrings.CurrentValue.ProviderName = connectionStrings.ProviderName;

        _databaseFactory.Configure(connectionStrings);
        return connectionStrings;
    }

    public void Teardown() => TryDeleteFile(GetAbsolutePath());

    private string GetAbsolutePath()
    {
        string? projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
        string tempFolder = @"TEMP\databases";
        var tempFolderPath = Path.Combine(projectDirectory!, tempFolder);
        return Path.Combine(tempFolderPath, _key.ToString());
    }

    private void TryDeleteFile(string filePath)
    {
        const int maxRetries = 5;
        var retries = 0;
        var retry = true;
        do
        {
            try
            {
                File.Delete(filePath);
                retry = false;
            }
            catch (IOException)
            {
                retries++;
                if (retries >= maxRetries)
                {
                    throw;
                }

                Thread.Sleep(500);
            }
        }
        while (retry);
    }
}
