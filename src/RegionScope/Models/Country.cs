namespace RegionScope.Models;

public sealed class Country
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public bool IsEuropeanUnionMember { get; set; }
}
