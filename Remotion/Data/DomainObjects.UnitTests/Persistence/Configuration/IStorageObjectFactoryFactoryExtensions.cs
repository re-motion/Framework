using System;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  public static class IStorageObjectFactoryFactoryExtensions
  {
    /// <summary>
    /// Resolves or creates an <see cref="IStorageObjectFactory"/> object based on the <see cref="Type"/> of the generic parameter.
    /// </summary>
    public static T Create<T> (this IStorageObjectFactoryFactory storageObjectFactoryFactory)
        where T : IStorageObjectFactory
    {
      return (T)storageObjectFactoryFactory.Create(typeof(T));
    }
  }
}
