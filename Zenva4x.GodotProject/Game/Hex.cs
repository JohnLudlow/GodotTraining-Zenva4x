using Godot;

namespace Zenva4x.GodotProject.Game;
public class Hex
{
    public TerrainTypes TerrainType { get; }
    public Vector2I Coordinates { get; }
    public int Food { get; set; }
    public int Production { get; set; }

    public City OwnerCity { get; set; }

    public bool IsCityCenter { get; set; }

    public Hex(TerrainTypes terrainType, Vector2I coordinates, int food = 0, int production = 0)
    {
        TerrainType = terrainType;
        Coordinates = coordinates;
        Food = food;
        Production = production;
    }

    public override string ToString()
    {
        return $"{TerrainType} at {Coordinates} => Owner [{OwnerCity?.CityName} ({OwnerCity?.OwnerCivilization?.CivilizationName})] Food {Food}, Production {Production}";
    }
}

