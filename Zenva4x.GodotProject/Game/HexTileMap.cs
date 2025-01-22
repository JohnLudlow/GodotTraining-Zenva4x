using System;
using System.Collections.Generic;
using System.Linq;

using Godot;

namespace Zenva4x.GodotProject.Game;

public enum TerrainTypes
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

public partial class HexTileMap : Node2D
{
  [Export]
  public int Width { get; set; } = 100;

  [Export]
  public int Height { get; set; } = 60;

  [Export]
  public int NumberOfAIPlayers { get; set; } = 8;

  [Export]
  public Color PlayerCivilizationColor { get; set; } = new Color(255, 255, 255);

  [Signal]
  public delegate void ClickOffMapEventHandler();

  private TileSetAtlasSource _tileSetAtlasSource;

  private PackedScene _cityScene = ResourceLoader.Load<PackedScene>("Game/City.tscn");

  private Dictionary<Vector2I, Hex> _mapData;

  private Dictionary<TerrainTypes, Vector2I> _terrainTextures;

  private TileMapLayer _baseLayer, _borderLayer, _overlayLayer, _civColorLayer;

  private Vector2I _selectedCell = new(-1, -1);

  private UIManager _uiManager;

  private readonly Dictionary<Vector2I, City> _cities = [];
  private readonly List<Civilization> _civs = [];

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
  public delegate void SendHexDataEventHandler(Hex hex);
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix

  [Signal]
  public delegate void SendCityUIInfoEventHandler(City city);

  public event SendHexDataEventHandler SendHexData;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    _uiManager = GetNode<UIManager>("/root/Game/CanvasLayer/UIManager");
    _mapData = [];
    _terrainTextures = new()
    {
      [TerrainTypes.Plains] = new Vector2I(0, 0),
      [TerrainTypes.Water] = new Vector2I(1, 0),

      [TerrainTypes.Desert] = new Vector2I(0, 1),
      [TerrainTypes.Mountain] = new Vector2I(1, 1),

      [TerrainTypes.Beach] = new Vector2I(0, 2),
      [TerrainTypes.ShallowWater] = new Vector2I(1, 2),

      [TerrainTypes.Ice] = new Vector2I(0, 3),
      [TerrainTypes.Forest] = new Vector2I(1, 3),

      [TerrainTypes.CivColorBase] = new Vector2I(0, 3),

    };

    _baseLayer = GetNode<TileMapLayer>("BaseLayer");
    _borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
    _overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");
    _civColorLayer = GetNode<TileMapLayer>("CivColorsLayer");
    _tileSetAtlasSource = (TileSetAtlasSource)_civColorLayer.TileSet.GetSource(0);

    GenerateTerrain();

    GenerateResources();

    var starts = GenerateStartingLocations(NumberOfAIPlayers + 1);
    var player = GeneratePlayerCiv(starts[0]);

    starts.RemoveAt(0);

    GenerateAIPlayerCivs(starts);

    SendHexData += _uiManager.UpdateTerrainInfoUI;
    _uiManager.EndTurn += ProcessTurn;
  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseButton mouseButton)
    {
      var mapCoords = _baseLayer.LocalToMap(GetLocalMousePosition());

      if (_mapData.TryGetValue(mapCoords, out var hex))
      {
        if (mouseButton.ButtonMask == MouseButtonMask.Left)
        {
          GD.Print(hex);

          if (_cities.TryGetValue(mapCoords, out var city))
          {
            EmitSignal(SignalName.SendCityUIInfo, city);
          }
          else
          {
            SendHexData?.Invoke(hex);
          }

          if (mapCoords != _selectedCell)
          {
            _overlayLayer.SetCell(_selectedCell, -1);
          }

          _overlayLayer.SetCell(mapCoords, 0, new(0, 1));
          _selectedCell = mapCoords;
        }
      }
      else
      {
        _overlayLayer.SetCell(_selectedCell, -1);
        EmitSignal(SignalName.ClickOffMap);
      }
    }
  }

  public void ProcessTurn()
  {
    foreach (var civilization in _civs)
    {
      civilization.ProcessTurn();
    }
  }

  public Civilization GeneratePlayerCiv(Vector2I start)
  {
    var id = _tileSetAtlasSource.CreateAlternativeTile(_terrainTextures[TerrainTypes.CivColorBase]);

    var player = new Civilization()
    {
      CivilizationID = 0,
      CivilizationName = "Player",
      CivilizationTerritoryColor = PlayerCivilizationColor,
      IsPlayerCivilization = true,

      AltTileId = id,
    };

    _tileSetAtlasSource.GetTileData(_terrainTextures[TerrainTypes.CivColorBase], id).Modulate = player.CivilizationTerritoryColor;
    _civs.Add(player);

    CreateCity(player, start, "Player Capital");

    return player;
  }

  private void GenerateAIPlayerCivs(List<Vector2I> startingLocations)
  {
    for (var i = 0; i < startingLocations.Count; i++)
    {
      var civ = new Civilization
      {
        CivilizationID = i + 1,
        CivilizationName = $"Civ {i + 1}",
        IsPlayerCivilization = false,
        AltTileId = 0,
      };

      var id = _tileSetAtlasSource.CreateAlternativeTile(_terrainTextures[TerrainTypes.CivColorBase]);
      GD.Print($"Created alternate tile for civ {civ.CivilizationName} {civ.CivilizationTerritoryColor} ({id}, {_tileSetAtlasSource.GetAlternativeTilesCount(_terrainTextures[TerrainTypes.CivColorBase])})");
      _tileSetAtlasSource.GetTileData(_terrainTextures[TerrainTypes.CivColorBase], id).Modulate = civ.CivilizationTerritoryColor;

      civ.AltTileId = id;

      CreateCity(civ, startingLocations[i], $"{civ.CivilizationName} City at {startingLocations[i]}");
      _civs.Add(civ);
    }
  }

  public void CreateCity(Civilization civilization, Vector2I coordinates, string cityName)
  {
    var city = _cityScene.Instantiate<City>();
    city.Map = this;
    city.OwnerCivilization = civilization;
    city.CityName = cityName;

    _mapData[coordinates].IsCityCenter = true;

    city.CityCentreCoordinates = coordinates;
    city.Position = _baseLayer.MapToLocal(coordinates);

    city.AddTerritory([_mapData[coordinates]]);
    city.AddTerritory(
        GetAdjacentCells(coordinates)
            .Select(c => _mapData[c])
            .Where(hex => hex.OwnerCity is null)
        );

    civilization.Cities.Add(city);

    AddChild(city);

    _cities[coordinates] = city;

    UpdateCivTerritoryMap(civilization);
  }

  private List<Vector2I> GenerateStartingLocations(int count)
  {
    var locations = new List<Vector2I>();
    var plainsLocations = _mapData.Values
        .Where(h => h.TerrainType == TerrainTypes.Plains)
        .Select(h => h.Coordinates).ToList();

    var rand = new Random();
    for (var i = 0; i < count; i++)
    {
      var coord = new Vector2I();
      var valid = false;
      var counter = 0;

      while (!valid && counter < 10000)
      {
        coord = plainsLocations.ElementAt(rand.Next(plainsLocations.Count));
        valid = IsValidStartingLocation(coord, locations);
        counter++;
      }

      if (valid)
      {
        plainsLocations.Remove(coord);

        foreach (var cell in GetAdjacentCells(coord, 3))
        {
          plainsLocations.Remove(cell);
        }

        locations.Add(coord);
      }
    }

    return locations;
  }

  private bool IsValidStartingLocation(Vector2I coordinates, IEnumerable<Vector2I> locations)
  {
    if (
        !_mapData.ContainsKey(coordinates) ||

        coordinates.X < 3 || coordinates.X > Width - 3 ||
        coordinates.Y < 3 || coordinates.Y > Height - 3
    )
    {
      return false;
    }

    if (locations.Any(l => Math.Abs(coordinates.X - l.X) < 20 && Math.Abs(coordinates.Y - l.Y) < 20))
    {
      return false;
    }

    return true;
  }

  public void UpdateCivTerritoryMap(Civilization civilization)
  {
    foreach (var city in civilization.Cities)
    {
      foreach (var hex in city.CityTerritory)
      {
        GD.Print($"Updating cell tile for civ {civilization.CivilizationName} {civilization.CivilizationTerritoryColor} ({civilization.AltTileId}) ({hex.Coordinates})");
        _civColorLayer.SetCell(hex.Coordinates, 0, _terrainTextures[TerrainTypes.CivColorBase], civilization.AltTileId);
      }
    }
  }

  private readonly Dictionary<Vector2I, WeakReference<IEnumerable<Hex>>> _adjacencyCache = [];
  private readonly object _locker = new();


  public IEnumerable<Hex> GetAdjacentHexes(Vector2I coordinates, int distance = 1)
  {
    lock (_locker)
    {
      if (_adjacencyCache.TryGetValue(coordinates, out var adj) && adj.TryGetTarget(out var hexes))
      {
        return hexes;
      }
      else
      {
        var newHexes = GetAdjacentCells(coordinates, distance).Select(c => _mapData[c]);
        _adjacencyCache[coordinates] = new WeakReference<IEnumerable<Hex>>(newHexes);      

        return newHexes;
      }

    }
  }

  private List<Vector2I> GetAdjacentCells(Vector2I coordinates, int distance = 1) 
    => GetAdjacentCells(coordinates, distance, []);

  private List<Vector2I> GetAdjacentCells(Vector2I coordinates, int distance, List<Vector2I> cells)
  {
    foreach (var adj in _baseLayer.GetSurroundingCells(coordinates))
    {
      if (distance > 1 && !cells.Contains(adj))
      {
        cells.AddRange(GetAdjacentCells(adj, distance - 1, cells));
      }

      if (_mapData.ContainsKey(adj))
      {
        cells.Add(adj);
      }
    }

    return cells.Distinct().ToList();
  }

  public bool HexInBounds(Vector2I coordinates)
    => _mapData.ContainsKey(coordinates);

  public void GenerateResources()
  {
    var r = new Random();
    for (var x = 0; x < Width; x++)
    {
      for (var y = 0; y < Height; y++)
      {
        var h = _mapData[new(x, y)];

        var (food, production) = h.TerrainType switch
        {
          TerrainTypes.Plains => (r.Next(2, 6), r.Next(0, 3)),
          TerrainTypes.Desert => (r.Next(0, 2), r.Next(0, 2)),
          TerrainTypes.Forest => (r.Next(1, 4), r.Next(2, 6)),
          TerrainTypes.Beach => (r.Next(0, 4), r.Next(0, 2)),
          _ => (0, 0),
        };

        h.Food = food;
        h.Production = production;
      }
    }
  }

  public void GenerateTerrain()
  {
    var rand = new Random();
    var seed = rand.Next(100000);

    var (baseNoiseMax, baseNoiseMap) = MakeSomeNoise(
        new FastNoiseLite
        {
          NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
          Seed = seed,
          Frequency = 0.008f,
          FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
          FractalOctaves = 4,
          FractalLacunarity = 2.25f
        }
    );

    var baseTerrainRanges = new[] {
            (0                        , baseNoiseMax / 10 * 2.5f , TerrainTypes.Water) ,
            (baseNoiseMax / 10 * 2.5f , baseNoiseMax / 10 * 4f   , TerrainTypes.ShallowWater) ,
            (baseNoiseMax / 10 * 4f   , baseNoiseMax / 10 * 4.5f , TerrainTypes.Beach) ,
            (baseNoiseMax / 10 * 4.5f , baseNoiseMax + .05f      , TerrainTypes.Plains) ,
        };

    DrawTerrain(baseNoiseMap, baseTerrainRanges);

    var (forestNoiseMax, forestNoiseMap) = MakeSomeNoise(
        new FastNoiseLite
        {
          NoiseType = FastNoiseLite.NoiseTypeEnum.Cellular,
          Seed = seed,
          Frequency = 0.04f,
          FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
          FractalLacunarity = 2f
        }
    );

    DrawOverrideTerrain(baseNoiseMap, baseTerrainRanges, forestNoiseMap, forestNoiseMax / 10 * 7, forestNoiseMax + .05f, TerrainTypes.Forest);

    var (desertNoiseMax, desertNoiseMap) = MakeSomeNoise(
        new FastNoiseLite
        {
          NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth,
          Seed = seed,
          Frequency = 0.06f,
          FractalType = FastNoiseLite.FractalTypeEnum.Fbm,
          FractalLacunarity = 2f
        }
    );

    DrawOverrideTerrain(baseNoiseMap, baseTerrainRanges, desertNoiseMap, desertNoiseMax / 10 * 5, desertNoiseMax + .05f, TerrainTypes.Desert);

    var (mountainNoiseMax, mountainNoiseMap) = MakeSomeNoise(
        new FastNoiseLite
        {
          NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex,
          Seed = seed,
          Frequency = 0.02f,
          FractalType = FastNoiseLite.FractalTypeEnum.Ridged,
          FractalLacunarity = 2f
        }
    );

    DrawOverrideTerrain(baseNoiseMap, baseTerrainRanges, mountainNoiseMap, mountainNoiseMax / 10 * 5, mountainNoiseMax + .05f, TerrainTypes.Mountain);

    DrawIceCap(rand);
  }

  private void DrawIceCap(Random random, int maxIce = 5)
  {
    for (var x = 0; x < Width; x++)
    {
      for (var y = 0; y < random.Next(1, maxIce) + 1; y++)
      {
        var hex = new Hex(TerrainTypes.Ice, new Vector2I(x, y));
        _mapData[hex.Coordinates] = hex;

        _baseLayer.SetCell(new Vector2I(x, y), 0, _terrainTextures[TerrainTypes.Ice]);
      }

      for (var y = Height - 1; y > Height - random.Next(1, maxIce) - 1; y--)
      {
        var hex = new Hex(TerrainTypes.Ice, new Vector2I(x, y));
        _mapData[hex.Coordinates] = hex;

        _baseLayer.SetCell(new Vector2I(x, y), 0, _terrainTextures[TerrainTypes.Ice]);
      }
    }
  }

  private void DrawOverrideTerrain(
      float[,] baseNoiseMap,
      IEnumerable<(float min, float max, TerrainTypes terrainType)> baseTerrainRanges,
      float[,] overrideNoiseMap,
      float overrideMin,
      float overrideMax,
      TerrainTypes overrideTerrainType)
  {
    for (var x = 0; x < Width; x++)
    {
      for (var y = 0; y < Height; y++)
      {
        var terrainType = baseTerrainRanges.First(range =>
            baseNoiseMap[x, y] >= range.min &&
            baseNoiseMap[x, y] < range.max
        ).terrainType;

        if (
            overrideNoiseMap[x, y] >= overrideMin &&
            overrideNoiseMap[x, y] < overrideMax &&
            terrainType == TerrainTypes.Plains
        )
        {
          var hex = new Hex(overrideTerrainType, new Vector2I(x, y));
          _mapData[hex.Coordinates] = hex;

          _baseLayer.SetCell(hex.Coordinates, 0, _terrainTextures[hex.TerrainType]);
          _borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
        }
      }
    }
  }

  private void DrawTerrain(float[,] noiseMap, IEnumerable<(float min, float max, TerrainTypes terrainType)> terrainRanges)
  {
    for (var x = 0; x < Width; x++)
    {
      for (var y = 0; y < Height; y++)
      {
        var terrainType = terrainRanges.First(range =>
            noiseMap[x, y] >= range.min &&
            noiseMap[x, y] < range.max
        ).terrainType;

        var hex = new Hex(terrainType, new Vector2I(x, y));
        _mapData[hex.Coordinates] = hex;

        _baseLayer.SetCell(hex.Coordinates, 0, _terrainTextures[terrainType]);
        _borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
      }
    }
  }

  private (float noiseMax, float[,] noiseMap) MakeSomeNoise(FastNoiseLite noise)
  {
    var noiseMap = new float[Width, Height];
    var noiseMax = 0f;

    for (var x = 0; x < Width; x++)
    {
      for (var y = 0; y < Height; y++)
      {
        noiseMap[x, y] = Math.Abs(noise.GetNoise2D(x, y));
        if (noiseMap[x, y] > noiseMax)
        {
          noiseMax = noiseMap[x, y];
        }
      }
    }

    return (noiseMax, noiseMap);
  }

  public Vector2 MapToLocal(Vector2I coords)
    => _baseLayer.MapToLocal(coords);
}