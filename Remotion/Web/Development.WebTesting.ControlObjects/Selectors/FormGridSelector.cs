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
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Control object selector for <see cref="FormGridControlObject"/>.
  /// </summary>
  public class FormGridSelector
      : TypedControlSelectorBase<FormGridControlObject>, ITitleControlSelector<FormGridControlObject>
  {
    public FormGridSelector ()
        : base ("FormGrid")
    {
    }

    /// <inheritdoc/>
    public FormGridControlObject SelectPerTitle (ControlSelectionContext context, string title)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("title", title);

      // Todo RM-6337: Replace with CSS-based search as soon as FormGridManager is able to render the data-title attribute.
      // Note: it is not that easy, as we do not know the content of the title row on the server...FormGrid is just a design transformator...
      //var scope = context.Scope.FindCss (string.Format ("table[{0}='{1}']", DiagnosticMetadataAttributes.FormGridTitle, title));

      // Note: this implementation assumes that the title cell has the CSS class formGridTitleCell.
      var hasClassCheck = XPathUtils.CreateHasClassCheck ("formGridTitleCell");
      var scope = context.Scope.FindXPath (string.Format (".//table[tbody/tr/td{0}='{1}']", hasClassCheck, title));

      // This alterantive implementation assumes that the title cell is the very first row and column.
      // var scope = context.Scope.FindXPath (string.Format (".//table[tbody/tr[1]/td[1]='{0}']", title));

      return CreateControlObject (context, scope);
    }

    /// <inheritdoc/>
    protected override FormGridControlObject CreateControlObject (
        ControlObjectContext newControlObjectContext,
        ControlSelectionContext controlSelectionContext)
    {
      ArgumentUtility.CheckNotNull ("controlSelectionContext", controlSelectionContext);
      ArgumentUtility.CheckNotNull ("newControlObjectContext", newControlObjectContext);

      return new FormGridControlObject (newControlObjectContext);
    }
  }
}