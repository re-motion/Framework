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
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="ImageButtonControlObject"/>.
  /// </summary>
  public class ImageButtonSelector
      : ControlSelectorBase<ImageButtonControlObject>,
          IFirstControlSelector<ImageButtonControlObject>,
          IIndexControlSelector<ImageButtonControlObject>,
          ISingleControlSelector<ImageButtonControlObject>
  {
    private const string c_inputTag = "input";

    /// <inheritdoc/>
    public ImageButtonControlObject SelectFirst (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindTagWithAttribute(c_inputTag, "type", "image");
      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public ImageButtonControlObject? SelectFirstOrNull (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindTagWithAttribute(c_inputTag, "type", "image");

      if (scope.ExistsWorkaround())
        return CreateControlObject(context, scope);

      return null;
    }

    /// <inheritdoc/>
    public ImageButtonControlObject SelectSingle (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindTagWithAttribute(c_inputTag, "type", "image");
      scope.EnsureSingle();
      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public ImageButtonControlObject? SelectSingleOrNull (ControlSelectionContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var scope = context.Scope.FindTagWithAttribute(c_inputTag, "type", "image");

      if (!scope.ExistsWithEnsureSingleWorkaround())
        return null;

      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public ImageButtonControlObject SelectPerIndex (ControlSelectionContext context, int oneBasedIndex)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var hasAttributeCheck = DomSelectorUtility.CreateHasAttributeCheckForXPath("type", "image");
      var xPathSelector = string.Format("(.//{0}{1})[{2}]", c_inputTag, hasAttributeCheck, oneBasedIndex);
      var scope = context.Scope.FindXPath(xPathSelector);
      return CreateControlObject(context, scope);
    }

    /// <inheritdoc/>
    public ImageButtonControlObject? SelectOptionalPerIndex (ControlSelectionContext context, int oneBasedIndex)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var hasAttributeCheck = DomSelectorUtility.CreateHasAttributeCheckForXPath("type", "image");
      var xPathSelector = string.Format("(.//{0}{1})[{2}]", c_inputTag, hasAttributeCheck, oneBasedIndex);
      var scope = context.Scope.FindXPath(xPathSelector);

      if (scope.ExistsWorkaround())
        return CreateControlObject(context, scope);

      return null;
    }

    /// <inheritdoc/>
    public bool ExistsPerIndex (ControlSelectionContext context, int oneBasedIndex)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var hasAttributeCheck = DomSelectorUtility.CreateHasAttributeCheckForXPath("type", "image");
      var xPathSelector = string.Format("(.//{0}{1})[{2}]", c_inputTag, hasAttributeCheck, oneBasedIndex);
      var scope = context.Scope.FindXPath(xPathSelector);

      return scope.ExistsWorkaround();
    }

    /// <inheritdoc/>
    protected override ImageButtonControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull("newControlObjectContext", newControlObjectContext);

      return new ImageButtonControlObject(newControlObjectContext);
    }
  }
}
