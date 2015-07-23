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
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieves the human-readable localized representation of reflection objects based on the <see cref="MultiLingualNameAttribute"/> 
  /// applied to the respective reflection object.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IMemberInformationGlobalizationService), Lifetime = LifetimeKind.Singleton, 
      Position = Position, RegistrationType = RegistrationType.Multiple)]
  public sealed class MultiLingualNameBasedMemberInformationGlobalizationService : IMemberInformationGlobalizationService
  {
    public const int Position = 0;

    private class LocalizedNameForTypeInformationProvider : LocalizedNameProviderBase<ITypeInformation>
    {
      protected override IEnumerable<MultiLingualNameAttribute> GetCustomAttributes (ITypeInformation typeInformation)
      {
        ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);

        return typeInformation.GetCustomAttributes<MultiLingualNameAttribute> (false);
      }

      protected override Assembly GetAssembly (ITypeInformation typeInformation)
      {
        ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);

        return typeInformation.Assembly;
      }

      protected override string GetContextForExceptionMessage (ITypeInformation typeInformation)
      {
        ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);

        return string.Format ("The type '{0}'", typeInformation.FullName);
      }
    }

    private class LocalizedNameForPropertyInformationProvider : LocalizedNameProviderBase<IPropertyInformation>
    {
      protected override IEnumerable<MultiLingualNameAttribute> GetCustomAttributes (IPropertyInformation propertyInformation)
      {
        ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);

        var originalDeclaration = propertyInformation.GetOriginalDeclaration();
        var isOriginalDeclaration = propertyInformation.Equals (originalDeclaration);

        var originallyDeclaredAttributes = originalDeclaration.GetCustomAttributes<MultiLingualNameAttribute> (false);

        if (!isOriginalDeclaration
            && propertyInformation.GetCustomAttributes<MultiLingualNameAttribute> (true).Length != originallyDeclaredAttributes.Length)
        {
          throw new InvalidOperationException (
              string.Format (
                  "The property '{0}' overridden on type '{1}' has one or more MultiLingualNameAttributes applied via a property override. "
                  + "The MultiLingualNameAttributes maybe only be applied to the original declaration of a property.",
                  propertyInformation.Name,
                  GetDeclaringTypeName (propertyInformation)));
        }
        return originallyDeclaredAttributes;
      }

      protected override Assembly GetAssembly (IPropertyInformation propertyInformation)
      {
        ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);

        return Maybe.ForValue (propertyInformation.GetOriginalDeclaringType()).Select (t => t.Assembly).ValueOrDefault();
      }

      protected override string GetContextForExceptionMessage (IPropertyInformation propertyInformation)
      {
        ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);

        return string.Format (
            "The property '{0}' declared on type '{1}'",
            propertyInformation.Name,
            GetDeclaringTypeName (propertyInformation.GetOriginalDeclaration()));
      }

      private string GetDeclaringTypeName (IPropertyInformation propertyInformation)
      {
        return Maybe.ForValue (propertyInformation.DeclaringType).Select (t => t.FullName).ValueOrDefault ("<undefined>");
      }
    }


    private readonly LocalizedNameForTypeInformationProvider _localizedNameForTypeInformationProvider =
        new LocalizedNameForTypeInformationProvider();

    private readonly LocalizedNameForPropertyInformationProvider _localizedNameForPropertyInformationProvider =
        new LocalizedNameForPropertyInformationProvider();

    public MultiLingualNameBasedMemberInformationGlobalizationService ()
    {
    }

    public bool TryGetTypeDisplayName (ITypeInformation typeInformation, ITypeInformation typeInformationForResourceResolution, out string result)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return _localizedNameForTypeInformationProvider.TryGetLocalizedNameForCurrentUICulture (typeInformation, out result);
    }

    public bool TryGetPropertyDisplayName (
        IPropertyInformation propertyInformation,
        ITypeInformation typeInformationForResourceResolution,
        out string result)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);
      ArgumentUtility.CheckNotNull ("typeInformationForResourceResolution", typeInformationForResourceResolution);

      return _localizedNameForPropertyInformationProvider.TryGetLocalizedNameForCurrentUICulture (propertyInformation, out result);
    }
  }
}