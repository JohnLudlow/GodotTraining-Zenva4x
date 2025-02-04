using System.Diagnostics;

using Godot;

using Zenva4x.Core.Game.Entities.WorldEntities;

namespace Zenva4x.Core.Game.Entities.MapEntities;

[DebuggerDisplay("{TerrainType} at {Coordinates} => Owner [{OwnerCity?.CityName} ({OwnerCity?.OwnerCivilization?.CivilizationName})] Food {Food}, Production {Production}")]
public class MapHexEntity : GameEntity
{
  public required Vector2I Coordinates { get; init; }
  public CityEntity? OwnerCity { get; set; }
  
  #region Hex stats

  public required TerrainType TerrainType { get; init; }
  public required int Food { get; init; }
  public required int Production { get; init; }

  #endregion
}