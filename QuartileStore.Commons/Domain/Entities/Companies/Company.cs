using QuartileStore.Commons.Domain.Entities.Abstracts;
using QuartileStore.Commons.Domain.Entities.Stores;

namespace QuartileStore.Commons.Domain.Entities.Companies;

public class Company : BaseEntity
{
    public int Code { get; set; }
    public string Name { get; set; }
    public string CountryCode { get; set; }

    public virtual ICollection<Store> Stores { get; set; } = [];

    public void Update(string name, string countryCode)
    {
        Name = name;
        CountryCode = countryCode;
    }
}