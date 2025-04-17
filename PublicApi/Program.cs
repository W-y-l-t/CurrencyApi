using Fuse8.BackendInternship.PublicApi;
using Fuse8.BackendInternship.PublicApi.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;

var app = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder
        .UseStartup<Startup>())
    .UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.WithMachineName()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers())
    )
    .Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FavoriteCurrencyContext>();
    await db.Database.MigrateAsync();
}

app.Run();