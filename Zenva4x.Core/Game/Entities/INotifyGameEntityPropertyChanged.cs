using System;

namespace Zenva4x.Core.Game.Entities;

public interface INotifyGameEntityPropertyChanged
{
  public event EventHandler<GameEntityChangedEventArgs> EntityChanged;
}

public class GameEntityChangedEventArgs(GameEntity entity) : EventArgs
{
  public required GameEntity Entity { get; init; } = entity;
}
