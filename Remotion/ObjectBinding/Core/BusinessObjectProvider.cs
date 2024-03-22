// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Concurrent;
using Remotion.Collections.DataStore;
using Remotion.Mixins;
using Remotion.TypePipe;
using Remotion.Utilities;
using TypeExtensions = Remotion.Reflection.TypeExtensions;

namespace Remotion.ObjectBinding
{
  /// <summary>The <see langword="abstract"/> default implementation of the <see cref="IBusinessObjectProvider"/> interface.</summary>
  public abstract class BusinessObjectProvider : IBusinessObjectProviderWithIdentity
  {
    private static readonly ConcurrentDictionary<Type, IBusinessObjectProvider> s_businessObjectProviderStore =
        new ConcurrentDictionary<Type, IBusinessObjectProvider>();

    ///<remarks>Optimized for memory allocations</remarks>
    private static readonly Func<Type, IBusinessObjectProvider> s_createBusinessObjectProviderFromAttribute = CreateBusinessObjectProviderFromAttribute;

    /// <summary>
    /// Gets the <see cref="IBusinessObjectProvider"/> associated with the <see cref="BusinessObjectProviderAttribute"/> type specified.
    /// </summary>
    /// <param name="businessObjectProviderAttributeType">
    /// A <see cref="Type"/> derived from <see cref="BusinessObjectProviderAttribute"/>. Must not be <see langword="null" />.
    /// </param>
    /// <remarks>If no provider has been registered, a default isntance will be created using the <see cref="ObjectFactory"/>.</remarks>
    public static IBusinessObjectProvider GetProvider (Type businessObjectProviderAttributeType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom(
          "businessObjectProviderAttributeType", businessObjectProviderAttributeType, typeof(BusinessObjectProviderAttribute));

      return s_businessObjectProviderStore.GetOrAdd(businessObjectProviderAttributeType, s_createBusinessObjectProviderFromAttribute);
    }

    /// <summary>
    /// Gets the <see cref="IBusinessObjectProvider"/> associated with the <see cref="BusinessObjectProviderAttribute"/> type specified.
    /// </summary>
    /// <typeParam name="TBusinessObjectProviderAttribute">
    /// A <see cref="Type"/> derived from <see cref="BusinessObjectProviderAttribute"/>. Must not be <see langword="null" />.
    /// </typeParam>
    /// <remarks>If no provider has been registered, a default isntance will be created using the <see cref="ObjectFactory"/>.</remarks>
    public static IBusinessObjectProvider GetProvider<TBusinessObjectProviderAttribute> ()
        where TBusinessObjectProviderAttribute: BusinessObjectProviderAttribute
    {
      return GetProvider(typeof(TBusinessObjectProviderAttribute));
    }

    /// <summary>
    /// Sets the <see cref="IBusinessObjectProvider"/> to be associated with the <see cref="BusinessObjectProviderAttribute"/> type specified.
    /// </summary>
    /// <param name="businessObjectProviderAttributeType">
    /// A <see cref="Type"/> derived from <see cref="BusinessObjectProviderAttribute"/>. Must not be <see langword="null" />.
    /// </param>
    /// <param name="provider">
    /// The <see cref="IBusinessObjectProvider"/> instance to be associated with the <paramref name="businessObjectProviderAttributeType"/>. 
    /// Pass <see langword="null"/> to remove the association.
    /// </param>
    public static void SetProvider (Type businessObjectProviderAttributeType, IBusinessObjectProvider provider)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom(
          "businessObjectProviderAttributeType", businessObjectProviderAttributeType, typeof(BusinessObjectProviderAttribute));

      if (provider != null)
      {
        BusinessObjectProviderAttribute attribute = CreateBusinessObjectProviderAttribute(businessObjectProviderAttributeType);
        if (!TypeExtensions.CanAscribeTo(provider.GetType(), attribute.BusinessObjectProviderType))
        {
          throw new ArgumentException(
              "The provider is not compatible with the provider-type required by the businessObjectProviderAttributeType's instantiation.", "provider");
        }

        if (provider is BusinessObjectProvider)
          ((BusinessObjectProvider)provider)._providerAttribute = attribute;
      }

      if (provider == null)
        s_businessObjectProviderStore.TryRemove(businessObjectProviderAttributeType, out _);
      else
        s_businessObjectProviderStore.AddOrUpdate(businessObjectProviderAttributeType, provider, (key, oldProvider) => provider);
    }

    /// <summary>
    /// Sets the <see cref="IBusinessObjectProvider"/> to be associated with the <see cref="BusinessObjectProviderAttribute"/> type specified.
    /// </summary>
    /// <typeParam name="TBusinessObjectProviderAttribute">
    /// A <see cref="Type"/> derived from <see cref="BusinessObjectProviderAttribute"/>. Must not be <see langword="null" />.
    /// </typeParam>
    /// <param name="provider">
    /// The <see cref="IBusinessObjectProvider"/> instance to be associated with the <typeParamref name="TBusinessObjectProviderAttribute"/>. 
    /// Pass <see langword="null"/> to remove the association.
    /// </param>
    public static void SetProvider<TBusinessObjectProviderAttribute> (IBusinessObjectProvider provider)
        where TBusinessObjectProviderAttribute: BusinessObjectProviderAttribute
    {
      SetProvider(typeof(TBusinessObjectProviderAttribute), provider);
    }

    private static IBusinessObjectProvider CreateBusinessObjectProviderFromAttribute (Type businessObjectProviderAttributeType)
    {
      BusinessObjectProviderAttribute? attribute = CreateBusinessObjectProviderAttribute(businessObjectProviderAttributeType);
      IBusinessObjectProvider provider = CreateBusinessObjectProvider(attribute.BusinessObjectProviderType);

      if (provider is BusinessObjectProvider)
        ((BusinessObjectProvider)provider)._providerAttribute = attribute;

      return provider;
    }

    private static BusinessObjectProviderAttribute CreateBusinessObjectProviderAttribute (Type businessObjectProviderAttributeType)
    {
      return (BusinessObjectProviderAttribute)Activator.CreateInstance(businessObjectProviderAttributeType)!; // TODO: Not null assertion
    }

    private static IBusinessObjectProvider CreateBusinessObjectProvider (Type businessObjectProviderType)
    {
      return (IBusinessObjectProvider)ObjectFactory.Create(businessObjectProviderType, ParamList.Empty);
    }

    private readonly IBusinessObjectServiceFactory _serviceFactory;
    private BusinessObjectProviderAttribute? _providerAttribute;
    private Func<Type, IBusinessObjectService?>? _getServiceStoreValueFactory;

    protected BusinessObjectProvider (IBusinessObjectServiceFactory serviceFactory)
    {
      ArgumentUtility.CheckNotNull("serviceFactory", serviceFactory);

      _serviceFactory = serviceFactory;
    }

    /// <summary> Gets the <see cref="IDataStore{TKey,TValue}"/> used to store the references to the registered servies. </summary>
    /// <value>An object implementing <see cref="IDataStore{TKey,TValue}"/>. Must not retun <see langword="null" />.</value>
    /// <remarks>
    ///   <note type="inotes">
    ///    If your object model does not support services, this property should return an instance of type <see cref="NullDataStore{TKey,TValue}"/>.
    ///   </note>
    /// </remarks>
    protected abstract IDataStore<Type, IBusinessObjectService?> ServiceStore { get; }

    /// <summary>Gets the <see cref="IBusinessObjectServiceFactory"/> passed during construction.</summary>
    public IBusinessObjectServiceFactory ServiceFactory
    {
      get { return _serviceFactory; }
    }

    public BusinessObjectProviderAttribute? ProviderAttribute
    {
      get { return _providerAttribute; }
    }

    /// <summary> Retrieves the requested <see cref="IBusinessObjectService"/>. Must not be <see langword="null" />.</summary>
    public IBusinessObjectService? GetService (Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("serviceType", serviceType, typeof(IBusinessObjectService));

      IDataStore<Type, IBusinessObjectService?> serviceStore = ServiceStore;
      Assertion.IsNotNull(serviceStore, "The ServiceStore evaluated and returned null. It should return a null object instead.");

      // Optimized for memory allocations
      if (_getServiceStoreValueFactory == null)
      {
        _getServiceStoreValueFactory = type => _serviceFactory.CreateService(this, type);
      }

      return serviceStore.GetOrCreateValue(serviceType, _getServiceStoreValueFactory);
    }

    /// <summary> Retrieves the requested <see cref="IBusinessObjectService"/>. </summary>
    public T? GetService<T> () where T: IBusinessObjectService
    {
      return (T?)GetService(typeof(T));
    }

    /// <summary> Registers a new <see cref="IBusinessObjectService"/> with this <see cref="BusinessObjectProvider"/>. </summary>
    /// <param name="serviceType"> The <see cref="Type"/> of the <paramref name="service"/> to be registered. Must not be <see langword="null" />.</param>
    /// <param name="service"> The <see cref="IBusinessObjectService"/> to register. Must not be <see langword="null" />.</param>
    public void AddService (Type serviceType, IBusinessObjectService service)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("serviceType", serviceType, typeof(IBusinessObjectService));
      ArgumentUtility.CheckNotNull("service", service);

      IDataStore<Type, IBusinessObjectService?> serviceStore = ServiceStore;
      Assertion.IsNotNull(serviceStore, "The ServiceStore evaluated and returned null. It should return a non-null object instead.");

      serviceStore[serviceType] = service;
    }

    /// <summary> Registers a new <see cref="IBusinessObjectService"/> with this <see cref="BusinessObjectProvider"/>. </summary>
    /// <param name="service"> The <see cref="IBusinessObjectService"/> to register. Must not be <see langword="null" />.</param>
    /// <typeparam name="T">The <see cref="Type"/> of the <paramref name="service"/> to be registered.</typeparam>
    public void AddService<T> (T service) where T : IBusinessObjectService
    {
      ArgumentUtility.CheckNotNull("service", service);

      AddService(typeof(T), service);
    }

    /// <summary>Returns the <see cref="Char"/> to be used as a serparator when formatting the property path's identifier.</summary>
    public virtual char GetPropertyPathSeparator ()
    {
      return '.';
    }

    /// <summary> Returns a <see cref="String"/> to be used instead of the actual value if the property is not accessible. </summary>
    /// <returns> A <see cref="String"/> that can be easily distinguished from typical property values. </returns>
    public virtual string GetNotAccessiblePropertyStringPlaceHolder ()
    {
      return "×";
    }
  }
}
