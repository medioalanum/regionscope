namespace RegionScope.Models;

public sealed class Indicator
{
    public required string Code { get; set; }

    public required string Name { get; set; }

    public required string Unit { get; set; }

    public ICollection<Observation> Observations { get; set; } = [];
}
