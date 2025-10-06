using QuartileStore.Commons.Domain.Entities.Abstracts;
using QuartileStore.Commons.Domain.Entities.Companies;
using QuartileStore.Commons.Domain.Entities.Products;

namespace QuartileStore.Commons.Domain.Entities.Stores;

public class Store : BaseEntity
{
    public int Code { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }

    public Guid CompanyId { get; set; }
    public virtual Company Company { get; set; }

    public virtual ICollection<Product> Products { get; set; } = [];

    public void Update(string name, string address)
    {
        Name = name;
        Address = address;
    }
}