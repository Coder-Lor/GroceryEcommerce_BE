using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using Microsoft.Extensions.Configuration;

namespace GroceryEcommerce.Infrastructure.Services;

public class DataAccessAdapterFactory(IConfiguration configuration) : IDataAccessAdapterFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("GroceryEcommerce")
        ?? throw new ArgumentNullException(nameof(configuration));
    
    public IDisposable CreateAdapter()
    {
        return new DataAccessAdapter(_connectionString);
    }
}