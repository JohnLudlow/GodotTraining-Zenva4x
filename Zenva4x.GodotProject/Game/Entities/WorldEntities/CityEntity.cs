namespace Zenva4x.GodotProject.Game.Entities.WorldEntities;

public class CityEntity : GameEntity
{
  public required string CityName {get; init;}
  public required CivilizationEntity OwnerCivilization {get; init;}
}
