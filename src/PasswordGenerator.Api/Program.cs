using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Api.Data;
using PasswordGenerator.Api.Endpoints;
using PasswordGenerator.Api.Services;
using PasswordGeneratorImpl = PasswordGenerator.Api.Services.PasswordGenerator;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PasswordStore")
	?? "Data Source=data/passwords.db";
var sqliteConnection = new SqliteConnectionStringBuilder(connectionString);

if (!string.Equals(sqliteConnection.DataSource, ":memory:", StringComparison.OrdinalIgnoreCase))
{
	if (!Path.IsPathRooted(sqliteConnection.DataSource))
	{
		sqliteConnection.DataSource = Path.GetFullPath(
			sqliteConnection.DataSource,
			builder.Environment.ContentRootPath);
	}

	var databaseDirectory = Path.GetDirectoryName(sqliteConnection.DataSource);
	if (!string.IsNullOrEmpty(databaseDirectory))
	{
		Directory.CreateDirectory(databaseDirectory);
	}
}

builder.Services.AddDbContext<PasswordDbContext>(options =>
	options.UseSqlite(sqliteConnection.ToString()));
builder.Services.AddScoped<IPasswordStore, EfPasswordStore>();
builder.Services.AddSingleton<IPasswordRandomSource, CryptographicPasswordRandomSource>();
builder.Services.AddSingleton<IPasswordGenerator, PasswordGeneratorImpl>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

var app = builder.Build();

app.MapPasswordEndpoints();

app.Run();

public partial class Program;