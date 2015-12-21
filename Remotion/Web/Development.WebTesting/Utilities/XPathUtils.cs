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
using System.Linq;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// XPath utility methods.
  /// </summary>
  public static class XPathUtils
  {
    /// <summary>
    /// Creates an XPath predicate, checking that a given <paramref name="attribtueName"/> has the given <paramref name="attributeValue"/>.
    /// </summary>
    /// <param name="attribtueName">The name of the attribute to check.</param>
    /// <param name="attributeValue">The value, the attribute should have.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateHasAttributeCheck (string attribtueName, string attributeValue)
    {
      return string.Format ("[@{0}='{1}']", attribtueName, attributeValue);
    }

    /// <summary>
    /// Creates an XPath predicate, checking for a specific CSS class.
    /// </summary>
    /// <param name="cssClass">The CSS class to check for.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateHasClassCheck (string cssClass)
    {
      return string.Format ("[{0}]", CreateClassCheckClause (cssClass));
    }

    /// <summary>
    /// Creates an XPath predicate, checking for "at least one" of the given CSS classes.
    /// </summary>
    /// <param name="cssClasses">The CSS classes to check for.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateHasOneOfClassesCheck (params string[] cssClasses)
    {
      var checkClauses = cssClasses.Select (CreateClassCheckClause);
      return "[" + string.Join (" or ", checkClauses) + "]";
    }

    /// <summary>
    /// Creates an XPath predicate clause, checking for a specific CSS class.
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/a/9133579/1400869 for more information on the implementation.
    /// </remarks>
    /// <param name="cssClass">The CSS class to check for.</param>
    /// <returns>The XPath predicate clause.</returns>
    private static string CreateClassCheckClause (string cssClass)
    {
      return string.Format ("contains(concat(' ', normalize-space(@class), ' '), ' {0} ')", cssClass);
    }
  }
}