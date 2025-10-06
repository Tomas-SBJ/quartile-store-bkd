namespace QuartileStore.Commons.Application.Dtos.Errors;

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }
    
    public IDictionary<string, string[]>? Errors { get; set; }
}