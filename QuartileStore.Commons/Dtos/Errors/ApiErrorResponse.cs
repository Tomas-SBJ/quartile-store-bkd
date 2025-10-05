namespace QuartileStore.Commons.Dtos.Errors;

public class ApiErrorResponse
{
    public int StatusCode { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }
}