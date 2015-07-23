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
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="WebButtonControlObject"/>.
  /// </summary>
  public class WebButtonSelector
      : TypedControlSelectorBase<WebButtonControlObject>,
          ITextContentControlSelector<WebButtonControlObject>,
          IItemIDControlSelector<WebButtonControlObject>
  {
    public WebButtonSelector ()
        : base ("WebButton")
    {
    }

    /// <inheritdoc/>
    public WebButtonControlObject SelectPerTextContent (ControlSelectionContext context, string textContent)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("textContent", textContent);

      var scope = context.Scope.FindTagWithAttributes (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributes.Content, textContent }
          });
      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    public WebButtonControlObject SelectPerItemID (ControlSelectionContext context, string itemID)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("itemID", itemID);

      var scope = context.Scope.FindTagWithAttributes (
          "*",
          new Dictionary<string, string>
          {
              { DiagnosticMetadataAttributes.ControlType, ControlType },
              { DiagnosticMetadataAttributes.ItemID, itemID }
          });
      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    protected override WebButtonControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new WebButtonControlObject (newControlObjectContext);
    }
  }
}