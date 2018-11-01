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

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// XPath utility methods.
  /// </summary>
  public static class XPathUtils
  {
    [Obsolete ("Use DomSelectorUtility.CreateHasAttributeCheckForXPath (string, string) instead. (1.19.2)")]
    public static string CreateHasAttributeCheck (string attributeName, string attributeValue)
    {
      return DomSelectorUtility.CreateHasAttributeCheckForXPath (attributeName, attributeValue);
    }

    [Obsolete ("Use DomSelectorUtility.CreateHasClassCheck (string) instead. (1.19.2)")]
    public static string CreateHasClassCheck (string cssClass)
    {
      return DomSelectorUtility.CreateHasClassCheckForXPath (cssClass);
    }

    [Obsolete ("Use DomSelectorUtility.CreateHasOneOfClassesCheck (string[]) instead. (1.19.2)")]
    public static string CreateHasOneOfClassesCheck (params string[] cssClasses)
    {
      return DomSelectorUtility.CreateHasOneOfClassesCheckForXPath (cssClasses);
    }
  }
}