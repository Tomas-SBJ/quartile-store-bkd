namespace QuartileStore.Commons.Application.Dtos.Stores;

public record StoreDto
{
    public int Code { get; set; }
    public int CompanyCode { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}