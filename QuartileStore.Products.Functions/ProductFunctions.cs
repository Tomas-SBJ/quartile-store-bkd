using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using QuartileStore.Commons.Dtos.Products;
using QuartileStore.Commons.Services.Contracts;
using QuartileStore.Products.Functions.Extensions;

namespace QuartileStore.Products.Functions;

public class ProductFunctions(
    IProductService productService,
    ILogger<ProductFunctions> logger
)
{
    [Function("CreateProduct")]
    public async Task<HttpResponseData> Create(
        [HttpTrigger(AuthorizationLevel.Function, "post",
            Route = "companies/{companyCode}/stores/{storeCode}/products")]
        HttpRequestData request,
        int companyCode,
        int storeCode)
    {
        logger.LogInformation("Request received to create a product");

        var (createDto, errorResponse) = await request.ReadAndValidateJsonAsync<ProductCreateDto>();

        if (errorResponse is not null)
            return errorResponse;
        
        var newProductDto = await productService.CreateAsync(storeCode, companyCode, createDto!);

        var response = request.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(newProductDto);

        response.Headers.Add("Location",
            $"api/companies/{companyCode}/stores/{storeCode}/products/{newProductDto.Code}");

        return response;
    }

    [Function("UpdateProduct")]
    public async Task<HttpResponseData> Update(
        [HttpTrigger(AuthorizationLevel.Function, "put",
            Route = "companies/{companyCode}/stores/{storeCode}/products/{code}")]
        HttpRequestData request,
        int companyCode,
        int storeCode,
        int code)
    {
        logger.LogInformation("Request received to update a product with code {Code}", code);

        var (updateDto, errorResponse) = await request.ReadAndValidateJsonAsync<ProductUpdateDto>();

        if (errorResponse is not null)
            return errorResponse;
        
        var updatedProduct = await productService.UpdateAsync(code, storeCode, companyCode, updateDto!);

        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(updatedProduct);

        return response;
    }

    [Function("GetProduct")]
    public async Task<HttpResponseData> GetProduct(
        [HttpTrigger(AuthorizationLevel.Function, "get",
            Route = "companies/{companyCode}/stores/{storeCode}/products/{code}")]
        HttpRequestData request,
        int companyCode,
        int storeCode,
        int code)
    {
        logger.LogInformation("Request received to get a product with code {Code}", code);
        
        var product = await productService.GetAsync(code, storeCode, companyCode);
        
        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(product);
        
        return response;
    }
    
    [Function("GetProducts")]
    public async Task<HttpResponseData> GetProducts(
        [HttpTrigger(AuthorizationLevel.Function, "get",
            Route = "companies/{companyCode}/stores/{storeCode}/products")]
        HttpRequestData request,
        int companyCode,
        int storeCode)
    {
        logger.LogInformation("Request received to get all products from store {Code}", storeCode);
        
        var products = await productService.GetAllByStoreAsync(storeCode, companyCode);
        
        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(products);
        
        return response;
    }
    
    [Function("DeleteProduct")]
    public async Task<HttpResponseData> Delete(
        [HttpTrigger(AuthorizationLevel.Function, "delete",
            Route = "companies/{companyCode}/stores/{storeCode}/products/{code}")]
        HttpRequestData request,
        int companyCode,
        int storeCode,
        int code)
    {
        logger.LogInformation("Request received to delete product with code {Code}", code);
        
        await productService.DeleteAsync(code,  storeCode, companyCode);
        
        return request.CreateResponse(HttpStatusCode.NoContent);
    }
}