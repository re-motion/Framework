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
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Various additional Find*() extension methods for Coypu's <see cref="ElementScope"/> class.
  /// </summary>
  public static class CoypuElementScopeFindExtensions
  {
    /// <summary>
    /// Returns a child element of a control, specified by a <paramref name="idSuffix"/>.
    /// </summary>
    public static ElementScope FindChild ([NotNull] this ElementScope scope, [NotNull] string idSuffix)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("idSuffix", idSuffix);

      var fullId = string.Format ("{0}_{1}", scope.Id, idSuffix);
      return scope.FindId (fullId);
    }

    /// <summary>
    /// Find an element with the given <paramref name="tagSelector"/> bearing a given attribute name/value combination.
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <param name="tagSelector">The CSS selector for the HTML tags to check for the attributes.</param>
    /// <param name="attributeName">The attribute name.</param>
    /// <param name="attributeValue">The attribute value.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindTagWithAttribute (
        [NotNull] this ElementScope scope,
        [NotNull] string tagSelector,
        [NotNull] string attributeName,
        [NotNull] string attributeValue)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("tagSelector", tagSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("attributeName", attributeName);
      ArgumentUtility.CheckNotNullOrEmpty ("attributeValue", attributeValue);

      var cssSelector = string.Format ("{0}[{1}='{2}']", tagSelector, attributeName, attributeValue);
      return scope.FindCss (cssSelector);
    }

    /// <summary>
    /// Find an element with the given <paramref name="tagSelector"/> bearing a given attribute name/value combination (value is compared using the
    /// given <paramref name="op"/>).
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <param name="tagSelector">The CSS selector for the HTML tags to check for the attributes.</param>
    /// <param name="op">The CSS operator to use when comparing the <paramref name="attributeValue"/>.</param>
    /// <param name="attributeName">The attribute name.</param>
    /// <param name="attributeValue">The attribute value.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindTagWithAttributeUsingOperator (
        [NotNull] this ElementScope scope,
        [NotNull] string tagSelector,
        CssComparisonOperator op,
        [NotNull] string attributeName,
        [NotNull] string attributeValue)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("tagSelector", tagSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("attributeName", attributeName);
      ArgumentUtility.CheckNotNullOrEmpty ("attributeValue", attributeValue);

      var cssSelector = string.Format ("{0}[{1}{2}'{3}']", tagSelector, attributeName, op.ToCssString(), attributeValue);
      return scope.FindCss (cssSelector);
    }

    /// <summary>
    /// Find an element with the given <paramref name="tagSelector"/> bearing one or more given attribute name/value combinations.
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <param name="tagSelector">The CSS selector for the HTML tags to check for the attributes.</param>
    /// <param name="attributes">The attribute name/value pairs to check for.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindTagWithAttributes (
        [NotNull] this ElementScope scope,
        [NotNull] string tagSelector,
        [NotNull] IDictionary<string, string> attributes)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("tagSelector", tagSelector);
      ArgumentUtility.CheckNotNull ("attributes", attributes);

      const string dmaCheckPattern = "[{0}='{1}']";
      var dmaCheck = string.Concat (attributes.Select (dm => string.Format (dmaCheckPattern, dm.Key, dm.Value)));
      var cssSelector = tagSelector + dmaCheck;
      return scope.FindCss (cssSelector);
    }

    /// <summary>
    /// Finds the first &lt;a&gt; element within the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The parent <see cref="ElementScope"/> which serves as the root element for the search.</param>
    /// <returns>The <see cref="ElementScope"/> of the found element.</returns>
    public static ElementScope FindLink ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      return scope.FindCss ("a");
    }
  }
}