using System.Net.Http.Json;
using MauiBlazorCrudOfflineFirst.Shared.Core.Models;

namespace MauiBlazorCrudOfflineFirst.Shared.Core.Services;

public class SyncService
{
    private readonly ProductService _productService;
    private readonly HttpClient _httpClient;

    public SyncService(ProductService productService, HttpClient httpClient)
    {
        _productService = productService;
        _httpClient = httpClient;
    }

    public async Task SyncAsync()
    {
        var unsynced = await _productService.GetUnSyncedAsync();

        foreach (var product in unsynced)
        {
            HttpResponseMessage response;
            if (product.IsDeleted)
            {
                response = await _httpClient.DeleteAsync($"api/products/{product.Id}");
            }
            else
            {
                response = await _httpClient.PostAsJsonAsync($"api/products", product);
            }

            if (response.IsSuccessStatusCode)
            {
                product.IsSynced = true;
                await _productService.AddOrUpdateAsync(product);
            }
        }
        
        var serverProducts = await _httpClient.GetFromJsonAsync<List<Product>>($"api/products");
        if (serverProducts is null) return;

        foreach (var remote in serverProducts)
        {
            var local = await _productService.GetByIdAsync(remote.Id);
            if (local ==null)
            {
                await _productService.UpsertFromServerAsync(remote);
            }
            else
            {
                if (local.LastModified > remote.LastModified)
                {
                    await _httpClient.PostAsJsonAsync($"api/products", remote);
                }
                else if (remote.LastModified > local.LastModified)
                {
                    await _productService.UpsertFromServerAsync(remote);
                }
            }
        }
    }
}