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
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieves the human-readable localized representation of reflection objects based on their names.
  /// </summary>
  /// <remarks>
  ///   <list type="bullet">
  ///     <item><see cref="TryGetTypeDisplayName"/> performs the lookup based on the long and the short name of the type, prefixed with <c>type:</c>.</item>
  ///     <item><see cref="TryGetPropertyDisplayName"/> performs the lookup based on the long and the short name of the property, prefixed with <c>property:</c>.</item>
  ///   </list>
  /// The long name is resolved using <see cref="IMemberInformationNameResolver"/>.
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IMemberInformationGlobalizationService), Lifetime = LifetimeKind.Singleton,
      Position = Position, RegistrationType = RegistrationType.Multiple)]
  public sealed class ResourceManagerBasedMemberInformationGlobalizationService : IMemberInformationGlobalizationService
  {
    public const int Position = MultiLingualNameBasedMemberInformationGlobalizationService.Position - 1;

    private readonly IGlobalizationService _globalizationService;
    private readonly IMemberInformationNameResolver _memberInformationNameResolver;

    public ResourceManagerBasedMemberInformationGlobalizationService (
        IGlobalizationService globalizationService,
        IMemberInformationNameResolver memberInformationNameResolver)
    {
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);
      ArgumentUtility.CheckNotNull ("memberInformationNameResolver", memberInformationNameResolver);

      _globalizationService = globalizationService;
      _memberInformationNameResolver = memberInformationNameResolver;
    }

    public bool TryGetTypeDisplayName (
        ITypeInformation typeInformation,
        ITypeInformation typeInformationForResourceResolution,
        out string result)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      result = GetStringOrDefault (
          typeInformationForResourceResolution,
          typeInformation.Name,
          _memberInformationNameResolver.GetTypeName (typeInformation),
          "type:");

      return result != null;
    }

    public bool TryGetPropertyDisplayName (
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution,
        out string result)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      result = GetStringOrDefault (
          typeInformationForResourceResolution,
          propertyInformation.Name,
          _memberInformationNameResolver.GetPropertyName (propertyInformation),
          "property:");

      return result != null;
    }

    private string GetStringOrDefault (ITypeInformation typeInformation, string shortMemberName, string longMemberName, string resourcePrefix)
    {
      var resourceManager = _globalizationService.GetResourceManager (typeInformation);

      return resourceManager.GetStringOrDefault (resourcePrefix + longMemberName)
             ?? resourceManager.GetStringOrDefault (resourcePrefix + shortMemberName);
    }
  }
}