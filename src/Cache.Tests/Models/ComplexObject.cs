namespace Cache.Tests.Models;

public record ComplexObject
{
    public string? StringValue { get; set; }
    public Guid GuidValue { get; set; }
    public DateTime DateTimeValue { get; set; }
}