namespace PCS.Front.Shared.Models;
public sealed record MenuItem
{
    public string? Icon { get; init; }
    public required string Ref { get; init; }
    public required string Label { get; init; }
}