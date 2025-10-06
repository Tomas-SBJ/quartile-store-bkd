using QuartileStore.Commons.Domain.Entities.Abstracts;
using QuartileStore.Commons.Domain.Entities.Stores;

namespace QuartileStore.Commons.Domain.Entities.Products;

public class Product : BaseEntity
{
    public int Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public Guid StoreId { get; set; }
    public virtual Store Store { get; set; }
    
    public void Update(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;
    }
}