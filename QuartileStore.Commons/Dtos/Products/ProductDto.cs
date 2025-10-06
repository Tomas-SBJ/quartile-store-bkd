namespace QuartileStore.Commons.Dtos.Products;

public class ProductDto
{
    public int Code { get; set; }
    public int StoreCode { get; set; }
    public int CompanyCode { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}