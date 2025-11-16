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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Combines one or more <see cref="IMemberInformationGlobalizationService"/>-instances and 
  /// delegates to them to retrieve localized name for a specified member.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor(typeof(IMemberInformationGlobalizationService), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public sealed class CompoundMemberInformationGlobalizationService : IMemberInformationGlobalizationService
  {
    private readonly IReadOnlyCollection<IMemberInformationGlobalizationService> _memberInformationGlobalizationServices;

    /// <summary>
    ///   Combines several <see cref="IGlobalizationService"/>-instances to a single <see cref="CompoundMemberInformationGlobalizationService"/>.
    /// </summary>
    /// <param name="memberInformationGlobalizationServices"> The <see cref="IMemberInformationGlobalizationService"/>s, starting with the least specific.</param>
    public CompoundMemberInformationGlobalizationService (IEnumerable<IMemberInformationGlobalizationService> memberInformationGlobalizationServices)
    {
      ArgumentNullException.ThrowIfNull(memberInformationGlobalizationServices);

      _memberInformationGlobalizationServices = memberInformationGlobalizationServices.ToArray();
    }

    public IEnumerable<IMemberInformationGlobalizationService> MemberInformationGlobalizationServices
    {
      get { return _memberInformationGlobalizationServices; }
    }

    public bool TryGetTypeDisplayName (
        ITypeInformation typeInformation,
        ITypeInformation typeInformationForResourceResolution,
        [MaybeNullWhen(false)] out string result)
    {
      ArgumentNullException.ThrowIfNull(typeInformation);
      ArgumentNullException.ThrowIfNull(typeInformationForResourceResolution);

      foreach (var service in _memberInformationGlobalizationServices)
      {
        if (service.TryGetTypeDisplayName(typeInformation, typeInformationForResourceResolution, out result))
          return true;
      }

      result = null;
      return false;
    }

    public bool TryGetPropertyDisplayName (
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution,
        [MaybeNullWhen(false)] out string result)
    {
      ArgumentNullException.ThrowIfNull(propertyInformation);
      ArgumentNullException.ThrowIfNull(typeInformationForResourceResolution);

      foreach (var service in _memberInformationGlobalizationServices)
      {
        if (service.TryGetPropertyDisplayName(propertyInformation, typeInformationForResourceResolution, out result))
          return true;
      }

      result = null;
      return false;
    }

    public IReadOnlyDictionary<CultureInfo, string> GetAvailablePropertyDisplayNames (
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentNullException.ThrowIfNull(propertyInformation);
      ArgumentNullException.ThrowIfNull(typeInformationForResourceResolution);

      var result = new Dictionary<CultureInfo, string>();
      foreach (var service in _memberInformationGlobalizationServices)
      {
        foreach (var localization in service.GetAvailablePropertyDisplayNames(propertyInformation, typeInformationForResourceResolution))
        {
          if (!result.ContainsKey(localization.Key))
            result.Add(localization.Key, localization.Value);
        }
      }

      return result;
    }

    public IReadOnlyDictionary<CultureInfo, string> GetAvailableTypeDisplayNames (
        ITypeInformation typeInformation,
        ITypeInformation typeInformationForResourceResolution)
    {
      ArgumentNullException.ThrowIfNull(typeInformation);
      ArgumentNullException.ThrowIfNull(typeInformationForResourceResolution);

      var result = new Dictionary<CultureInfo, string>();
      foreach (var service in _memberInformationGlobalizationServices)
      {
        foreach (var localization in service.GetAvailableTypeDisplayNames(typeInformation, typeInformationForResourceResolution))
        {
          if (!result.ContainsKey(localization.Key))
            result.Add(localization.Key, localization.Value);
        }
      }

      return result;
    }
  }
}
