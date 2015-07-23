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
using System.Threading;
using Remotion.ExtensibleEnums;
using Remotion.Globalization;
using Remotion.Globalization.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Defines a facade for localizing the information exposed by the <see cref="IBusinessObject"/> interfaces via various globalization services.
  /// </summary>
  /// <remarks>
  /// Implementations of the <see cref="BindableObjectGlobalizationService"/> can be directly registered with <see cref="BusinessObjectProvider"/> 
  /// using the <see cref="BusinessObjectProvider.AddService"/> method or indirectly by providing a custom implementation of the 
  /// <see cref="IBusinessObjectServiceFactory"/>.
  /// </remarks>
  [ImplementationFor (typeof (BindableObjectGlobalizationService), Lifetime = LifetimeKind.Singleton)]
  public sealed class BindableObjectGlobalizationService
  {
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Globalization.BindableObjectGlobalizationService")]
    private enum ResourceIdentifier
    {
      True,
      False
    }

    private readonly IMemberInformationGlobalizationService _memberInformationGlobalizationService;
    private readonly Lazy<IResourceManager> _resourceManager;
    private readonly IEnumerationGlobalizationService _enumerationGlobalizationService;
    private readonly IExtensibleEnumGlobalizationService _extensibleEnumGlobalizationService;

    public BindableObjectGlobalizationService (
        IGlobalizationService globalizationServices,
        IMemberInformationGlobalizationService memberInformationGlobalizationService,
        IEnumerationGlobalizationService enumerationGlobalizationService,
        IExtensibleEnumGlobalizationService extensibleEnumGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("globalizationServices", globalizationServices);
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("enumerationGlobalizationService", enumerationGlobalizationService);
      ArgumentUtility.CheckNotNull ("extensibleEnumGlobalizationService", extensibleEnumGlobalizationService);

      _resourceManager = new Lazy<IResourceManager> (
          () => globalizationServices.GetResourceManager (typeof (ResourceIdentifier)),
          LazyThreadSafetyMode.ExecutionAndPublication);
      _memberInformationGlobalizationService = memberInformationGlobalizationService;
      _enumerationGlobalizationService = enumerationGlobalizationService;
      _extensibleEnumGlobalizationService = extensibleEnumGlobalizationService;
    }

    /// <summary>
    /// Gets the localized display name of an enumeration value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The localized display name.</returns>
    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return _enumerationGlobalizationService.GetEnumerationValueDisplayName (value);
    }

    /// <summary>
    /// Gets the localized display name of the extensible enumeration value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The localized display name.</returns>
    public string GetExtensibleEnumerationValueDisplayName (IExtensibleEnum value) //move to member info globalization service
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return _extensibleEnumGlobalizationService.GetExtensibleEnumValueDisplayName (value);
    }

    /// <summary>
    /// Gets the localized display name of a boolean value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The localized display name.</returns>
    public string GetBooleanValueDisplayName (bool value)
    {
      return _resourceManager.Value.GetString (value ? ResourceIdentifier.True : ResourceIdentifier.False);
    }

    /// <summary>
    /// Gets the localized display name of a type.
    /// </summary>
    /// <param name="typeInformation">The <see cref="ITypeInformation"/> for which to lookup the resource.</param>
    /// <param name="typeInformationForResourceResolution">The <see cref="ITypeInformation"/> providing the resources.</param>
    /// <returns>The localized display name.</returns>
    public string GetTypeDisplayName (ITypeInformation typeInformation, ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return _memberInformationGlobalizationService.GetTypeDisplayName (typeInformation, typeInformationForResourceResolution);
    }

    /// <summary>
    /// Gets the localized display name of a property.
    /// </summary>
    /// <param name="propertyInformation">The <see cref="IPropertyInformation"/> for which to lookup the resource.</param>
    /// <param name="typeInformationForResourceResolution">The <see cref="ITypeInformation"/> providing the resources.</param>
    /// <returns>The localized display name.</returns>
    public string GetPropertyDisplayName (IPropertyInformation propertyInformation, ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      var mixinIntroducedPropertyInformation = propertyInformation as MixinIntroducedPropertyInformation;
      if (mixinIntroducedPropertyInformation != null)
      {
        // Note: this is only needed until there is support for interfaces in object binding, 
        // or at least, until PropertyInfoAdapter can perform the FindIntefaceDeclaration lookup fast.
        // Then, the MultiLingualResourcesBasedMemberInformationGlobalizationService will be able to implement the look of the localization 
        // from the class-qualified name, then the interfaces-qualified names, and finally, the unqualified name.
        var interfaceImplementationPropertyInfo = mixinIntroducedPropertyInformation.InterfaceImplementationPropertyInfo;
        string displayNameFromInterface;
        if (_memberInformationGlobalizationService.TryGetPropertyDisplayName (
            interfaceImplementationPropertyInfo.DeclarationPropertyInfo,
            typeInformationForResourceResolution,
            out displayNameFromInterface))
        {
          return displayNameFromInterface;
        }

        string displayNameFromImplementation;
        if (_memberInformationGlobalizationService.TryGetPropertyDisplayName (
            interfaceImplementationPropertyInfo.ImplementationPropertyInfo,
            typeInformationForResourceResolution,
            out displayNameFromImplementation))
        {
          return displayNameFromImplementation;
        }
      }

      return _memberInformationGlobalizationService.GetPropertyDisplayName (
          propertyInformation.GetOriginalDeclaration(),
          typeInformationForResourceResolution);
    }
  }
}