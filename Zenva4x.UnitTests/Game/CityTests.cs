using Zenva4x.GodotProject.Game;

namespace Zenva4x.UnitTests.Game;

public class CityTests
{
  [Fact]
  public void City_ctor_CreatesCity()
  {
    var city = new City();
    Assert.IsType<City>(city);
  }
}
