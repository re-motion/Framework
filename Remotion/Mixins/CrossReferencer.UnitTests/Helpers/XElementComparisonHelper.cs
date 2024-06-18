// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Helpers
{
  public static class XElementComparisonHelper
  {
    private class XElementComparisonStringGenerator
    {
      private readonly IEnumerable<string> _ignoredAttributes;

      public XElementComparisonStringGenerator (params string[] ignoredAttributes)
      {
        _ignoredAttributes = ignoredAttributes;
      }

      public string Generate (XElement x)
      {
        var s = x.ToString ();
        foreach (var ignoredAttribute in _ignoredAttributes)
          s = Regex.Replace (s, string.Format ("{0}=\"[^\"]*\" ", ignoredAttribute), "");

        return s;
      }
    }

    private static readonly XElementComparisonStringGenerator s_stringGenerator =
      new XElementComparisonStringGenerator("metadataToken");

    public static void Compare(XElement actual, XElement expected)
    {
      Assert.That(s_stringGenerator.Generate(actual), Is.EqualTo(s_stringGenerator.Generate(expected)));
    }
  }

  
}
