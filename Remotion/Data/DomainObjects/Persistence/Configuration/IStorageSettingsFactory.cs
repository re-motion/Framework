using System;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public interface IStorageSettingsFactory
  {
    IStorageSettings Create (IStorageObjectFactoryFactory storageObjectFactoryFactory);
  }
}
