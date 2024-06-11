public sealed record ResponseDto<TData>
{
    public TData? Data { get; init; }
    public List<string> Errors { get; init; } = [];
    public bool HasErrors => Errors?.Any() ?? false;
    public uint TotalCount { get; init; } = 1;
    public uint Page => (uint)(HasErrors ? Errors!.Count : 1);
}