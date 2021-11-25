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
using System.Globalization;
using JetBrains.Annotations;

#nullable disable

// ReSharper disable once CheckNamespace
namespace Remotion.Globalization
{
  /// <summary>
  /// Use this class to get the clear text representations of enumeration values.
  /// </summary>
  /// <remarks>
  /// Use the <see cref="MultiLingualResourcesAttribute"/> or the <see cref="MultiLingualNameAttribute"/> to provide globalization support for the enum type.
  /// </remarks>
  public static class EnumDescription
  {
    [NotNull]
    [Obsolete ("Use Enum.GetValues (enumType) to get the values, then use IEnumerationGlobalizationService.GetEnumerationValueDisplayName (...) for each value. (Version 1.13.223.0)", true)]
    public static EnumValue[] GetAllValues ([NotNull] Type enumType)
    {
      throw new NotSupportedException("Use Enum.GetValues (enumType) to get the values, then use IEnumerationGlobalizationService.GetEnumerationValueDisplayName (...) for each value. (Version 1.13.223.0)");
    }

    [NotNull]
    [Obsolete ("Use Enum.GetValues (enumType) to get the values, then use IEnumerationGlobalizationService.GetEnumerationValueDisplayName (...) for each value. Use a CultureScope set the CurrentUICulture. (Version 1.13.223.0)", true)]
    public static EnumValue[] GetAllValues ([NotNull] Type enumType, [CanBeNull] CultureInfo culture)
    {
      throw new NotSupportedException("Use Enum.GetValues (enumType) to get the values, then use IEnumerationGlobalizationService.GetEnumerationValueDisplayName (...) for each value. Use a CultureScope set the CurrentUICulture. (Version 1.13.223.0)");
    }

    [NotNull]
    [Obsolete ("Use IEnumerationGlobalizationService.GetEnumerationValueDisplayName (...). (Version 1.13.223.0)", true)]
    public static string GetDescription ([NotNull] Enum value)
    {
      throw new NotSupportedException("Use IEnumerationGlobalizationService.GetEnumerationValueDisplayName (...). (Version 1.13.223.0)");
    }

    [NotNull]
    [Obsolete ("Use IEnumerationGlobalizationService.GetEnumerationValueDisplayName (...).  Use a CultureScope set the CurrentUICulture. (Version 1.13.223.0)", true)]
    public static string GetDescription ([NotNull] Enum value, [CanBeNull] CultureInfo culture)
    {
      throw new NotSupportedException("Use IEnumerationGlobalizationService.GetEnumerationValueDisplayName (...).  Use a CultureScope set the CurrentUICulture. (Version 1.13.223.0)");
    }
  }
}

#nullable restore
