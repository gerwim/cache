namespace Cache.Tests.Models;

public record ComplexObject : IComplexObject
{
    public string? StringValue { get; set; }
    public Guid GuidValue { get; set; }
    public DateTime DateTimeValue { get; set; }
}

public interface IComplexObject
{
    public string? StringValue { get; set; }
}