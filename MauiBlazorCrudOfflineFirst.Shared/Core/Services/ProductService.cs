using MauiBlazorCrudOfflineFirst.Shared.Core.Models;
using SQLite;

namespace MauiBlazorCrudOfflineFirst.Shared.Core.Services;

public class ProductService
{
    private readonly SQLiteAsyncConnection _connection;

    public ProductService(string dbPath)
    {
        _connection = new SQLiteAsyncConnection(dbPath);
        _connection.CreateTableAsync<Product>().Wait();
    }

    public Task<List<Product>> GetProductsAsync()
    {
        return _connection.Table<Product>().Where(p=>!p.IsDeleted).ToListAsync();
    }

    public Task<List<Product>> GetUnSyncedAsync()
    {
        var allUnSynced = _connection.Table<Product>().Where(p=>!p.IsSynced).ToListAsync();
        return allUnSynced;
    }

    public Task<Product> GetByIdAsync(int id)
    {
        return _connection.Table<Product>().Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public Task<int> AddOrUpdateAsync(Product product)
    {
        product.LastModified = DateTime.UtcNow;
        return _connection.InsertOrReplaceAsync(product);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product != null)
        {
            product.IsDeleted = true;
            product.LastModified = DateTime.UtcNow;
            product.IsSynced = true;
            return await _connection.UpdateAsync(product);
        }

        return 0;
    }

    public Task<int> UpsertFromServerAsync(Product product)
    {
        return _connection.InsertOrReplaceAsync(product);
    }
}