using Godot;

using Zenva4x.GodotProject.Game.Entities.WorldEntities;

namespace Zenva4x.GodotProject.Game.Entities.MapEntities;

public enum TerrainType
{
  Water,
  Plains,
  Desert,
  Mountain,
  Forest,
  Beach,
  ShallowWater,
  Ice,
  CivColorBase,
}

public class MapHexEntity : GameEntity
{
  public required Vector2I Coordinates { get; init; }
  public CityEntity? OwnerCity { get; set; }
  
  #region Hex stats

  public required TerrainType TerrainType { get; init; }
  public required int Food { get; init; }
  public required int Production { get; init; }

  #endregion

  public override string ToString()
  {
      return $"{TerrainType} at {Coordinates} => Owner [{OwnerCity?.CityName} ({OwnerCity?.OwnerCivilization?.CivilizationName})] Food {Food}, Production {Production}";
  }

}