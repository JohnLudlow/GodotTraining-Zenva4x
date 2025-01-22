using Godot;
using System;

namespace Zenva4x.GodotProject.Game;

public partial class Warrior : Unit
{
  public Warrior()
  {
    UnitName = "Warrior";
    ProductionRequired = 50;
  }
}
