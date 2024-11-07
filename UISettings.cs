namespace FRSP6498;
/// <summary>
/// Data class to describe a Control for the input form
/// </summary>
public class UISettings
{
    /// <summary>
    /// Every Setting needs to have a name
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Leave as null if the control is ui only (does not accept input)
    /// </summary>
    public string? DataType { get; set; }
    public string? Position { get; set; }
    public string? Value { get; set; }
}
