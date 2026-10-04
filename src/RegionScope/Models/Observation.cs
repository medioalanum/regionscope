namespace RegionScope.Models;

public sealed class Observation
{
    public required string CountryCode { get; set; }
    public required string IndicatorCode { get; set; }
    public int Year { get; set; }
    public decimal Value { get; set; }
    public required string Unit { get; set; }
    public required string Source { get; set; }
    public DateTime RetrievedAtUtc { get; set; }
    public Country? Country { get; set; }
    public Indicator? Indicator { get; set; }
}
