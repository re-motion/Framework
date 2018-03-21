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
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// DOM Selector utility methods.
  /// </summary>
  public static class DomSelectorUtility
  {
    /// <summary>
    /// Creates a quoted match value from the <paramref name="propertyValue"/>, which can be used in an CSS selector query.
    /// </summary>
    public static string CreateMatchValueForCssSelector (string propertyValue)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyValue", propertyValue);

      return string.Format ("'{0}'", propertyValue.Replace ("'", @"\'"));
    }

    /// <summary>
    /// Creates a quoted match value from the <paramref name="propertyValue"/>, which can be used in an XPath query.
    /// </summary>
    /// <remarks>
    /// Applies the correct string parameter delimiter (double quote, single quote) for the given <paramref name="propertyValue"/>.
    /// WebDriver use XPath 1.0, where it is not possible to use escape sequences. To have a correctly formatted XPath,
    /// we have to switch between the correct string delimiter or use the XPath concat() method if double quote AND single quote is used.
    /// </remarks>
    public static string CreateMatchValueForXPath (string propertyValue)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyValue", propertyValue);

      if (propertyValue.Contains ("'") && propertyValue.Contains ("\""))
        return string.Format ("concat('{0}')", propertyValue.Replace ("'", "',\"'\",'"));
      else if (propertyValue.Contains ("'"))
        return string.Format ("\"{0}\"", propertyValue);

      return string.Format ("'{0}'", propertyValue);
    }

    /// <summary>
    /// Creates an XPath predicate, checking that a given <paramref name="attributeName"/> has the given <paramref name="attributeValue"/>.
    /// </summary>
    /// <param name="attributeName">The name of the attribute to check.</param>
    /// <param name="attributeValue">The value, the attribute should have.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateHasAttributeCheckForXPath (string attributeName, string attributeValue)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("attributeName", attributeName);
      ArgumentUtility.CheckNotNull ("attributeValue", attributeValue);

      return string.Format ("[@{0}='{1}']", attributeName, attributeValue);
    }

    /// <summary>
    /// Creates an XPath predicate, checking for a specific CSS class.
    /// </summary>
    /// <param name="cssClass">The CSS class to check for.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateHasClassCheckForXPath (string cssClass)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("cssClass", cssClass);

      return string.Format ("[{0}]", CreateClassCheckClauseForXPath (cssClass));
    }

    /// <summary>
    /// Creates an XPath predicate, checking for "at least one" of the given CSS classes.
    /// </summary>
    /// <param name="cssClasses">The CSS classes to check for.</param>
    /// <returns>The XPath predicate.</returns>
    public static string CreateHasOneOfClassesCheckForXPath (params string[] cssClasses)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("cssClasses", cssClasses);

      var checkClauses = cssClasses.Select (CreateClassCheckClauseForXPath);
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
    private static string CreateClassCheckClauseForXPath (string cssClass)
    {
      return string.Format ("contains(concat(' ', normalize-space(@class), ' '), ' {0} ')", cssClass);
    }
  }
}
