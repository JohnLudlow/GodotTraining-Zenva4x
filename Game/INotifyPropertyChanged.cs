using System;

namespace ZenvaHexMap.Game;

public class EntityUpdatedEventArgs<TEntity>(TEntity city) : EventArgs
{
  public TEntity City { get; } = city;
}


public interface INotifyEntityPropertyChanged<TEntityArgsType>
{
  event EventHandler<TEntityArgsType> PropertyChanged;
}
