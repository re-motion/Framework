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
using System.Collections;
using Remotion.Collections;
using Remotion.Mixins;
using Remotion.Utilities;
using TypeExtensions = Remotion.Reflection.TypeExtensions;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The implementation for the <see cref="IBusinessObjectProvider"/> interface for the reflection based business object layer.
  /// </summary>
  public class BindableObjectProvider : BusinessObjectProvider
  {
    #region Obsolete

    [Obsolete ("Use BusinessObjectProvider.GetProvider instead. (Version 1.9.1.0)", true)]
    public static BindableObjectProvider Current
    {
      get { throw new NotImplementedException ("Obsolete. Use BusinessObjectProvider.GetProvider instead. (Version 1.9.1.0)"); }
    }

    [Obsolete ("Use BusinessObjectProvider.SetProvider instead. (Version 1.9.1.0)", true)]
    public static void SetCurrent (BindableObjectProvider provider)
    {
      throw new NotImplementedException ("Obsolete. Use BusinessObjectProvider.GetProvider instead. (Version 1.9.1.0)");
    }

    #endregion

    private static readonly LockingCacheDecorator<Type, Type> s_providerAttributeTypeCache = CacheFactory.CreateWithLocking<Type, Type>();

    /// <summary>
    /// Use this method as a shortcut to retrieve the <see cref="BindableObjectProvider"/> for a <see cref="Type"/> 
    /// that has the <see cref="BindableObjectMixinBase{T}"/> applied or is derived from a bindable object base class without first retrieving the 
    /// matching provider.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to retrieve the <see cref="BindableObjectProvider"/> for.</param>
    /// <returns>Returns the <see cref="BindableObjectProvider"/> for the <paramref name="type"/>.</returns>
    public static BindableObjectProvider GetProviderForBindableObjectType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var providerAttributeType = s_providerAttributeTypeCache.GetOrCreateValue (type, FindProviderAttributeType);

      var provider = (BindableObjectProvider) GetProvider (providerAttributeType);
      Assertion.IsNotNull (provider, "GetProvider cannot return null (type '{0}').", type.FullName);
      return provider;
    }

    /// <summary>
    /// Determines whether the specified type is a bindable object implementation.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>
    ///   <see langword="true"/> if the specified <paramref name="type"/> has a mixin derived from <see cref="BindableObjectMixinBase{TBindableObject}"/> 
    ///   or the <see cref="BindableObjectBaseClassAttribute"/> applied; otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <note type="caution">This check is not cached by the implemention. The result should be cached.</note>
    /// </remarks>
    public static bool IsBindableObjectImplementation (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (!IsSupportedAsBindableObjectImplementation (type))
        return false;

      if (HasBindableObjectMixin (type))
        return true;

      return IsBindableObjectBaseClass (type);
    }

    internal static Type GetConcreteTypeForBindableObjectImplementation (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var hasSupportForMixins = !type.IsSealed;
      if (hasSupportForMixins)
        return MixinTypeUtility.GetConcreteMixedType (type);

      return type;
    }

    private static bool IsSupportedAsBindableObjectImplementation (Type type)
    {
      return !type.ContainsGenericParameters;
    }

    private static bool IsBindableObjectBaseClass (Type type)
    {
      return typeof (IBusinessObject).IsAssignableFrom (type) && AttributeUtility.IsDefined (type, typeof (BindableObjectBaseClassAttribute), true);
    }

    private static bool HasBindableObjectMixin (Type type)
    {
      return MixinTypeUtility.HasAscribableMixin (type, typeof (BindableObjectMixinBase<>));
    }

    private static Type FindProviderAttributeType (Type type)
    {
      var concreteType = MixinTypeUtility.GetConcreteMixedType (type);
      var attribute = AttributeUtility.GetCustomAttribute<BusinessObjectProviderAttribute> (concreteType, true);

      if (attribute == null)
      {
        var message = string.Format (
            "The type '{0}' does not have the '{1}' applied.",
            type.FullName,
            typeof (BusinessObjectProviderAttribute).FullName);
        throw new ArgumentException (message, "type");
      }

      if (!TypeExtensions.CanAscribeTo (attribute.BusinessObjectProviderType, typeof (BindableObjectProvider)))
      {
        var message = string.Format (
            "The business object provider associated with the type '{0}' is not of type '{1}'.",
            type.FullName,
            typeof (BindableObjectProvider).FullName);
        throw new ArgumentException (message, "type");
      }

      return attribute.GetType ();
    }

    private readonly IDataStore<Type, BindableObjectClass> _businessObjectClassStore = DataStoreFactory.CreateWithLocking<Type, BindableObjectClass>();
    private readonly IDataStore<Type, IBusinessObjectService> _serviceStore = DataStoreFactory.CreateWithLocking<Type, IBusinessObjectService>();
    private readonly IMetadataFactory _metadataFactory;

    public BindableObjectProvider ()
        : this (BindableObjectMetadataFactory.Create(), BindableObjectServiceFactory.Create())
    {
    }

    protected BindableObjectProvider (IMetadataFactory metadataFactory)
        : this (metadataFactory, BindableObjectServiceFactory.Create())
    {
    }

    public BindableObjectProvider (IMetadataFactory metadataFactory, IBusinessObjectServiceFactory serviceFactory)
        : base (serviceFactory)
    {
      ArgumentUtility.CheckNotNull ("metadataFactory", metadataFactory);
      ArgumentUtility.CheckNotNull ("serviceFactory", serviceFactory);

      _metadataFactory = metadataFactory;
    }

    /// <summary>
    /// Gets the <see cref="IMetadataFactory"/> for this <see cref="BindableObjectProvider"/>. The <see cref="MetadataFactory"/> determines
    /// how properties are found for a specific <see cref="BindableObjectClass"/>.
    /// </summary>
    public IMetadataFactory MetadataFactory
    {
      get { return _metadataFactory; }
    }

    /// <summary> The <see cref="IDictionary"/> used to store the references to the registered servies. </summary>
    protected override IDataStore<Type, IBusinessObjectService> ServiceStore
    {
      get { return _serviceStore; }
    }

    /// <summary>
    /// Use this method to retrieve the <see cref="BindableObjectClass"/> for a <see cref="Type"/> 
    /// that has the <see cref="BindableObjectMixinBase{T}"/> applied or is derived from a bindable object base class.
    /// </summary>
    /// <param name="type">The type to get a <see cref="BindableObjectClass"/> for. This type must have a mixin derived from
    /// <see cref="BindableObjectMixinBase{TBindableObject}"/> or the <see cref="BindableObjectBaseClassAttribute"/> applied,
    /// and it is recommended to specify the simple target type rather then the generated mixed type.</param>
    /// <returns>Returns the <see cref="BindableObjectClass"/> for the <paramref name="type"/>.</returns>
    public BindableObjectClass GetBindableObjectClass (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _businessObjectClassStore.GetOrCreateValue (type, CreateBindableObjectClass);
    }

    private BindableObjectClass CreateBindableObjectClass (Type type)
    {
      if (!IsBindableObjectImplementation (type))
      {
        if (type.ContainsGenericParameters)
        {
          throw new ArgumentException (
              string.Format (
                  "The type '{0}' is not a bindable object implementation. Open generic types are not supported.",
                  type.FullName),
              "type");
        }

        throw new ArgumentException (
            string.Format (
                "The type '{0}' is not a bindable object implementation. It must either have a mixin derived from BindableObjectMixinBase<T> applied "
                + "or implement the IBusinessObject interface and apply the BindableObjectBaseClassAttribute.",
                type.FullName),
            "type");
      }

      IClassReflector classReflector = _metadataFactory.CreateClassReflector (type, this);
      Assertion.IsNotNull (classReflector, "The IMetadataFactory.CreateClassReflector method evaluated and returned null.");

      return classReflector.GetMetadata();
    }
  }
}