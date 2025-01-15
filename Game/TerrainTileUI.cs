using System.Collections.Generic;

using Godot;

namespace ZenvaHexMap.Game;

public partial class TerrainTileUI : Panel
{
  TextureRect _terrainImage;
  Label _terrainLabel, _foodLabel, _productionLabel;
  private Hex _hex;

  private static readonly Dictionary<TerrainTypes, string> _terrainTypeStrings = new()
  {
    [TerrainTypes.Beach] = "Beach",
    [TerrainTypes.Desert] = "Ice",
    [TerrainTypes.Forest] = "Forest",
    [TerrainTypes.Ice] = "Ice",
    [TerrainTypes.Mountain] = "Mountain",
    [TerrainTypes.Plains] = "Plains",
    [TerrainTypes.ShallowWater] = "Shallow Water",
    [TerrainTypes.Water] = "Water",
  };

  private static readonly Dictionary<TerrainTypes, Texture2D> _terrainTypeImages = new()
  {
    [TerrainTypes.Beach] = ResourceLoader.Load<Texture2D>("res://Assets/UI/beach.jpg"),
    [TerrainTypes.Desert] = ResourceLoader.Load<Texture2D>("res://Assets/UI/desert.jpg"),
    [TerrainTypes.Forest] = ResourceLoader.Load<Texture2D>("res://Assets/UI/forest.jpg"),
    [TerrainTypes.Ice] = ResourceLoader.Load<Texture2D>("res://Assets/UI/ice.jpg"),
    [TerrainTypes.Mountain] = ResourceLoader.Load<Texture2D>("res://Assets/UI/mountain.jpg"),
    [TerrainTypes.Plains] = ResourceLoader.Load<Texture2D>("res://Assets/UI/plains.jpg"),
    [TerrainTypes.ShallowWater] = ResourceLoader.Load<Texture2D>("res://Assets/UI/shallow.jpg"),
    [TerrainTypes.Water] = ResourceLoader.Load<Texture2D>("res://Assets/UI/ocean.jpg"),
  };

  public Hex Hex
  {
    get => _hex;
    set
    {
      _hex = value;

      _terrainImage.Texture = _terrainTypeImages[_hex.TerrainType];
      _terrainLabel.Text = _terrainTypeStrings[_hex.TerrainType];
      _foodLabel.Text = $"Food: {value.Food}";
      _productionLabel.Text = $"Prodution : {value.Production}";
    }
  }

  public override void _Ready()
  {
    _terrainImage = GetNode<TextureRect>("TerrainImage");
    _terrainLabel = GetNode<Label>("TerrainLabel");
    _foodLabel = GetNode<Label>("FoodLabel");
    _productionLabel = GetNode<Label>("ProductionLabel");
  }
}
