using System;
using System.Collections.Generic;

using Godot;

namespace ZenvaHexMap.Game;

public partial class Civilization
{
  public Civilization()
  {
    var rand = new Random();
    CivilizationTerritoryColor = new Color(rand.Next(255) / 255f, rand.Next(255) / 255f, rand.Next(255) / 255f);
  }


  public required int CivilizationID { get; init; }
  public required string CivilizationName { get; init; }
  public bool IsPlayerCivilization { get; init; }

  public Color CivilizationTerritoryColor { get; init; }
  public required int AltTileId { get; set; }
  public List<City> Cities { get; set; } = [];

  public void ProcessTurn() {foreach (var city in Cities) { city.ProcessTurn(); } }
}
