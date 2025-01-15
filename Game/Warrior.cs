using Godot;
using System;

namespace ZenvaHexMap.Game;

public partial class Warrior : Unit
{
  public Warrior()
  {
    UnitName = "Warrior";
    ProductionRequired = 50;
  }
}
