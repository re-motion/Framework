﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Coypu;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="AnchorControlObject"/>.
  /// </summary>
  public class AnchorSelector
      : ControlSelectorBase<AnchorControlObject>,
          IFirstControlSelector<AnchorControlObject>,
          IIndexControlSelector<AnchorControlObject>,
          ISingleControlSelector<AnchorControlObject>,
          ITextContentControlSelector<AnchorControlObject>
  {
    private const string c_htmlAnchorTag = "a";

    /// <inheritdoc/>
    public AnchorControlObject SelectFirst (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindCss(c_htmlAnchorTag);
      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public AnchorControlObject? SelectFirstOrNull (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindCss(c_htmlAnchorTag);

      if (scope.ExistsWorkaround())
        return CreateControlObject(context, scope);

      return null;
    }

    /// <inheritdoc/>
    public AnchorControlObject SelectSingle (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindCss(c_htmlAnchorTag, Options.Single);
      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public AnchorControlObject? SelectSingleOrNull (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindCss(c_htmlAnchorTag, Options.Single);

      if (scope.ExistsWorkaround())
        return CreateControlObject(context, scope);

      return null;
    }

    /// <inheritdoc/>
    public AnchorControlObject SelectPerIndex (ControlSelectionContext context, int oneBasedIndex)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindXPath(string.Format("(.//{0})[{1}]", c_htmlAnchorTag, oneBasedIndex));
      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public AnchorControlObject? SelectOptionalPerIndex (ControlSelectionContext context, int oneBasedIndex)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindXPath(string.Format("(.//{0})[{1}]", c_htmlAnchorTag, oneBasedIndex));

      if (scope.ExistsWorkaround())
        return CreateControlObject(context, scope);

      return null;
    }

    /// <inheritdoc/>
    public bool ExistsPerIndex (ControlSelectionContext context, int oneBasedIndex)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindXPath(string.Format("(.//{0})[{1}]", c_htmlAnchorTag, oneBasedIndex));

      return scope.ExistsWorkaround();
    }

    /// <inheritdoc/>
    public AnchorControlObject SelectPerTextContent (ControlSelectionContext context, string textContent)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("textContent", textContent);

      var scope = context.Scope.FindXPath(
          string.Format("(.//{0})[.={1}]", c_htmlAnchorTag, DomSelectorUtility.CreateMatchValueForXPath(textContent)));
      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public AnchorControlObject? SelectOptionalPerTextContent (ControlSelectionContext context, string textContent)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("textContent", textContent);

      var scope = context.Scope.FindXPath(
          string.Format("(.//{0})[.={1}]", c_htmlAnchorTag, DomSelectorUtility.CreateMatchValueForXPath(textContent)));

      if (scope.ExistsWorkaround())
        return CreateControlObject(context, scope);

      return null;
    }

    /// <inheritdoc/>
    public bool ExistsPerTextContent (ControlSelectionContext context, string textContent)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("textContent", textContent);

      var scope = context.Scope.FindXPath(string.Format("(.//{0})[.={1}]", c_htmlAnchorTag, DomSelectorUtility.CreateMatchValueForXPath(textContent)));

      return scope.ExistsWorkaround();
    }

    /// <inheritdoc/>
    protected override AnchorControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull("newControlObjectContext", newControlObjectContext);

      return new AnchorControlObject(newControlObjectContext);
    }
  }
}
