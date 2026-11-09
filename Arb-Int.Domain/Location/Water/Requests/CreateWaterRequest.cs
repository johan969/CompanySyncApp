

using Arb_Int.Domain.Location;

public class CreateWaterRequest
{
    public string Name { get; set; } = null!;
    public WaterType Type { get; set; }
    public WaterFeatureType FeatureType { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? ParentWaterId { get; set; }
}