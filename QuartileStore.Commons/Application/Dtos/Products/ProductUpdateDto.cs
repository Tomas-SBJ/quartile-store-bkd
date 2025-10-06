using System.ComponentModel.DataAnnotations;

namespace QuartileStore.Commons.Application.Dtos.Products;

public class ProductUpdateDto
{
    [Required(ErrorMessage = "The Name field is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "The Description field is required")]
    public string Description { get; set; }

    [Required(ErrorMessage = "The Price field is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "The Price field must be greater than 0")]
    public decimal Price { get; set; }
}