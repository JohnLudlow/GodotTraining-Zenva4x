using Godot;
using System;

namespace ZenvaHexMap.Game;

public partial class Settler : Unit
{
  public Settler()
  {
    UnitName = "Settler";
    ProductionRequired = 100;
  }
}
