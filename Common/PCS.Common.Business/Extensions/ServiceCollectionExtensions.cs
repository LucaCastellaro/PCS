using MongoDB.Driver;

namespace PCS.Common.Business.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices<T>(this IServiceCollection services)
    {
        var types = typeof(T).Assembly.GetTypes();

        var interfaces = types.Where(xx => xx.Name.StartsWith("I") && xx.Name.EndsWith("Service") && xx.IsInterface);

        foreach (var abstraction in interfaces)
        {
            var implementation = types.FirstOrDefault(xx => xx.Name == abstraction.Name[1..] && xx.IsClass && !xx.IsAbstract);

            if (implementation is null) continue;

            services.AddTransient(abstraction, implementation);
        }

        return services;
    }

    public static IServiceCollection AddMongoDb(this IServiceCollection services, string connectionString, bool logQueryOnConsole = false)
    {
        string connectionString2 = connectionString;
        services.AddSingleton((Func<IServiceProvider, IMongoClient>)delegate
        {
            var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrlBuilder(connectionString2).ToMongoUrl());

            return new MongoClient(mongoClientSettings);
        });
        services.AddScoped((IServiceProvider sp) => sp.GetRequiredService<IMongoClient>().StartSession());
        services.AddSingleton(delegate (IServiceProvider sp)
        {
            IMongoClient requiredService = sp.GetRequiredService<IMongoClient>();
            string databaseName = MongoUrl.Create(connectionString2).DatabaseName;
            return requiredService.GetDatabase(databaseName);
        });
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
}
