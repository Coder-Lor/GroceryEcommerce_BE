namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IDataAccessAdapterFactory
{
    IDisposable CreateAdapter();
}