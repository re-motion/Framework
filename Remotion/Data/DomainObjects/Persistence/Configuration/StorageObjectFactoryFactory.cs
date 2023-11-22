using System;
using System.Configuration;
using JetBrains.Annotations;
using Remotion.Mixins;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// Resolves or creates an <see cref="IStorageObjectFactory"/>.
  /// </summary>
  [UsedImplicitly]
  [ImplementationFor(typeof(IStorageObjectFactoryFactory), Lifetime = LifetimeKind.Singleton)]
  public class StorageObjectFactoryFactory : IStorageObjectFactoryFactory
  {
    /// <summary>
    /// Resolves or creates an <see cref="IStorageObjectFactory"/> object based on the supplied <see cref="Type"/>.
    /// If the <see cref="IStorageObjectFactory"/> of the requested type cannot be resolved by the service locator, a
    /// new instance is created.
    /// </summary>
    public IStorageObjectFactory Create (Type storageObjectFactoryType)
    {

      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("storageObjectFactoryType", storageObjectFactoryType, typeof(IStorageObjectFactory));
      try
      {
        var registeredService = (IStorageObjectFactory?)SafeServiceLocator.Current.GetService(storageObjectFactoryType);
        if (registeredService != null)
          return registeredService;
      }
      catch (ActivationException ex)
      {
        var message = string.Format(
            "The factory type '{0}' cannot be resolved: {1}",
            storageObjectFactoryType,
            ex.Message);
        throw new ConfigurationErrorsException(message, ex);
      }

      if (storageObjectFactoryType.IsAbstract)
      {
        var message = string.Format(
            "The factory type '{0}' cannot be instantiated because it is "
            + "abstract. Either register an implementation of '{1}' in the configured service locator, or specify a non-abstract type.",
            storageObjectFactoryType,
            storageObjectFactoryType.Name);
        throw new ConfigurationErrorsException(message);
      }

      try
      {
        return (IStorageObjectFactory)ObjectFactory.Create(storageObjectFactoryType);
      }
      catch (Exception ex)
      {
        var message = string.Format(
            "The factory type '{0}' cannot be instantiated: {1}",
            storageObjectFactoryType,
            ex.Message);
        throw new ConfigurationErrorsException(message, ex);
      }
    }
  }
}
