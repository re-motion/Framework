using System;
using System.Configuration;
using Remotion.Mixins;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.Persistence
{
  public interface IStorageObjectFactoryFactory
  {
    IStorageObjectFactory Create (Type storageObjectFactoryType);
    T Create<T> () where T : IStorageObjectFactory;
  }

  class StorageObjectFactoryFactory : IStorageObjectFactoryFactory
  {
    public IStorageObjectFactory Create (Type storageObjectFactoryType)
    {
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

    public T Create<T> ()
        where T : IStorageObjectFactory
    {
      return (T)Create(typeof(T));
    }
  }
}
