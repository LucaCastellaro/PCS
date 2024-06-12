namespace PCS.Front.Shared.Models;
public record MenuItem
{
    public string? Icon { get; init; }
    public required string Ref { get; init; }
    public required string Label { get; init; }
}

public record TabItem : MenuItem
{
    public required bool IsActive { get; set; }
}