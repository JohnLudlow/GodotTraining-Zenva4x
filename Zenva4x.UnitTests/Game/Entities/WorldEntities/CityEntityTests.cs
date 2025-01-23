using Zenva4x.GodotProject.Game.Entities.WorldEntities;

namespace Zenva4x.UnitTests.Game.Entities.WorldEntities;

public static class CityEntityTests
{

  public class Constructor
  {
    [Fact]
    public void CreatesCity()
    {
      var city = new CityEntity { 
        CityName = "A City",
        OwnerCivilization = new CivilizationEntity{
          CivilizationName = "A Civilization"
        }
      };
      Assert.IsType<CityEntity>(city);
    }
  }
}