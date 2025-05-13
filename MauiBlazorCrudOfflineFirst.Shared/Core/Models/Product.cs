using SQLite;

namespace MauiBlazorCrudOfflineFirst.Shared.Core.Models;

public class Product
{
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public bool IsSynced { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }
} 