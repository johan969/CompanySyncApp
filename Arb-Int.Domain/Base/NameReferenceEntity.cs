using Arb_Int.Domain.Base;

/// <summary>
/// Adds Name, NormalizedName, Description, Code and IsActive to a datamodel
/// </summary>
public abstract class NamedReferenceEntity : INormalizedName, IDescribable, IIsActive, ICode
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}