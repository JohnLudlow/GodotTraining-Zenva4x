using System;

namespace Zenva4x.GodotProject.Game.Entities;

public interface INotifyGameEntityPropertyChanged
{
  public event EventHandler<GameEntityChangedEventArgs> EntityChanged;
}

public class GameEntityChangedEventArgs(GameEntity entity) : EventArgs
{
  public required GameEntity Entity { get; init; } = entity;
}
