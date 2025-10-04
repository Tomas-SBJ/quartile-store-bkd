using System.Security.Principal;
using QuartileStore.Commons.Domain.Entities.Stores;

namespace QuartileStore.Commons.Domain.Entities.Products;

public class Product
{
    public int Id { get; set; }
    public int Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public int StoreId { get; set; }
    public Store Store { get; set; }
}