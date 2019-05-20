using Remotion.Data.DomainObjects.Persistence.Model;

namespace Remotion.Data.DomainObjects.Persistence.NonPersistent.Model
{
  /// <summary>
  /// Defines that a property does not have persistence information available.
  /// </summary>
  public class NonPersistentStorageProperty : IStoragePropertyDefinition
  {
    public static readonly IStoragePropertyDefinition Instance = new NonPersistentStorageProperty();

    private NonPersistentStorageProperty ()
    {
    }
  }
}