using Godot;

namespace Zenva4x.GodotProject.Game;

public partial class UIManager : Node2D
{
  private TerrainTileUI _terrainTileUI;
  private CityUI _cityUI;
  private GeneralUI _generalUI;

  [Signal]
  public delegate void EndTurnEventHandler();

  private PackedScene TerrainUIScene { get; } = ResourceLoader.Load<PackedScene>("Game/TerrainTileUI.tscn");
  private PackedScene CityUIScene { get; } = ResourceLoader.Load<PackedScene>("Game/CityUI.tscn");


  private CityUI CityUI
  {

    get => _cityUI;
    set
    {
      HideAllPopups();

      _cityUI?.QueueFree();
      _cityUI = value;
    }
  }

  private TerrainTileUI TerrainTileUI
  {

    get => _terrainTileUI;
    set
    {
      HideAllPopups();

      _terrainTileUI = value;
    }
  }

  public override void _Ready()
  {
    base._Ready();

    _generalUI = GetNode<GeneralUI>("GeneralUI");

    var button = _generalUI.GetNode<Button>("EndTurnButton");
    button.Pressed += SignalEndTurn;
  }

  public void SignalEndTurn()
  {
    EmitSignal(SignalName.EndTurn);
    _generalUI.EndTurn();
  }

  public void HideAllPopups()
  {
    _terrainTileUI?.QueueFree();
    _terrainTileUI = null;

    _cityUI?.QueueFree();
    _cityUI = null;
  }

  public void UpdateTerrainInfoUI(Hex hex)
  {
    TerrainTileUI = (TerrainTileUI)TerrainUIScene.Instantiate();
    AddChild(TerrainTileUI);
    TerrainTileUI.Hex = hex;
  }

  public void UpdateCityInfoUI(City city)
  {
    CityUI = (CityUI)CityUIScene.Instantiate();
    AddChild(CityUI);
    CityUI.City = city;
  }
}
