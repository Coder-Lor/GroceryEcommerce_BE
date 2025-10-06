namespace GroceryEcommerce.Application.Models.Catalog;

public class ProductQuestionDto
{
    public Guid ProductQuestionId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public Guid? AnsweredBy { get; set; }
    public string? AnsweredByName { get; set; }
    public short Status { get; set; } // 1: Pending, 2: Answered, 3: Rejected
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateProductQuestionRequest
{
    public Guid ProductId { get; set; }
    public string Question { get; set; } = string.Empty;
}

public class UpdateProductQuestionRequest
{
    public string Question { get; set; } = string.Empty;
    public short Status { get; set; }
}
