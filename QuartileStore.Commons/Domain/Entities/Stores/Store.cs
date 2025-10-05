using System.ComponentModel.DataAnnotations.Schema;
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
    public Company Company { get; set; }
    
    [NotMapped]
    public List<Product> Products { get; set; }
    
    public void Update(string name, string address)
    {
        Name = name;
        Address = address;
    }
}