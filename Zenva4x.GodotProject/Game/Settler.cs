using Godot;
using System;

namespace Zenva4x.GodotProject.Game;

public partial class Settler : Unit
{
  public Settler()
  {
    UnitName = "Settler";
    ProductionRequired = 100;
  }
}
