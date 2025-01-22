using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ZenvaHexMap.Game;

[DebuggerDisplay("{UnitName}")]
public partial class Unit : Node2D
{
  private Civilization? _ownerCivilization;

  public static ReadOnlyDictionary<Type, PackedScene> UnitScneResoures {get;} = new (
    new Dictionary<Type, PackedScene>() {
      [typeof(Settler)] = ResourceLoader.Load<PackedScene>("res://Game/Settler.tscn"),
      [typeof(Warrior)] = ResourceLoader.Load<PackedScene>("res://Game/Warrior.tscn"),
    }
  );

  public Vector2I UnitCoordinates { get; set; }

  public Civilization? OwnerCivilization
  {
    get => _ownerCivilization;
    set
    {
      _ownerCivilization = value;
      _ownerCivilization?.Units.Add(this);
    }
  }

  public string UnitName { get; protected set; } = "DEFAULT";
  public int ProductionRequired { get; protected set; }
}
