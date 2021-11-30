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
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  /// Informs the globalization infrastructure of the localization cultures available via satelite assemblies for the assembly the attribute has been applied to.
  /// </summary>
  /// <remarks>
  /// Note that if the <see cref="AvailableResourcesLanguagesAttribute"/> is not applied, all system cultures are checked to see if they contain
  /// localizations for this assembly, resulting in a significant performance penality. The <see cref="AvailableResourcesLanguagesAttribute"/> is
  /// only required when working with the <see cref="MultiLingualResourcesAttribute"/>.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
  public class AvailableResourcesLanguagesAttribute : Attribute
  {
    private readonly string[] _cultureNames;

    public AvailableResourcesLanguagesAttribute (string cultureName1)
      : this(new[] { cultureName1 })
    {
    }

    public AvailableResourcesLanguagesAttribute (string cultureName1, string cultureName2)
      : this(new[] { cultureName1, cultureName2 })
    {
    }

    public AvailableResourcesLanguagesAttribute (string cultureName1, string cultureName2, string cultureName3)
      : this(new[] { cultureName1, cultureName2, cultureName3 })
    {
    }

    public AvailableResourcesLanguagesAttribute (string cultureName1, string cultureName2, string cultureName3, string cultureName4)
      : this(new[] { cultureName1, cultureName2, cultureName3, cultureName4 })
    {
    }

    public AvailableResourcesLanguagesAttribute (
        string cultureName1,
        string cultureName2,
        string cultureName3,
        string cultureName4,
        string cultureName5)
      : this(new[] { cultureName1, cultureName2, cultureName3, cultureName4, cultureName5 })
    {
    }

    public AvailableResourcesLanguagesAttribute (
        string cultureName1,
        string cultureName2,
        string cultureName3,
        string cultureName4,
        string cultureName5,
        string cultureName6)
      : this(new[] { cultureName1, cultureName2, cultureName3, cultureName4, cultureName5, cultureName6 })
    {
    }

    public AvailableResourcesLanguagesAttribute (
        string cultureName1,
        string cultureName2,
        string cultureName3,
        string cultureName4,
        string cultureName5,
        string cultureName6,
        string cultureName7)
      : this(new[] { cultureName1, cultureName2, cultureName3, cultureName4, cultureName5, cultureName6, cultureName7 })
    {
    }

    public AvailableResourcesLanguagesAttribute (
        string cultureName1,
        string cultureName2,
        string cultureName3,
        string cultureName4,
        string cultureName5,
        string cultureName6,
        string cultureName7,
        string cultureName8)
      : this(new[] { cultureName1, cultureName2, cultureName3, cultureName4, cultureName5, cultureName6, cultureName7, cultureName8 })
    {
    }

    public AvailableResourcesLanguagesAttribute (
        string cultureName1,
        string cultureName2,
        string cultureName3,
        string cultureName4,
        string cultureName5,
        string cultureName6,
        string cultureName7,
        string cultureName8,
        string cultureName9)
      : this(new[] { cultureName1, cultureName2, cultureName3, cultureName4, cultureName5, cultureName6, cultureName7, cultureName8, cultureName9 })
    {
    }

    public AvailableResourcesLanguagesAttribute (string[] cultureNames)
    {
      ArgumentUtility.CheckNotNullOrEmpty("cultureNames", cultureNames);

      _cultureNames = cultureNames;
    }

    public string[] CultureNames
    {
      get { return _cultureNames; }
    }
  }
}
