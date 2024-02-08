namespace Cache.Tests.Models;

public record NonPublic
{
    public NonPublic(string privateSet, string init)
    {
        Private = privateSet;
        Init = init;
    }
    
    public string Private { get; private set; }
    public string Init { get; init; }
}