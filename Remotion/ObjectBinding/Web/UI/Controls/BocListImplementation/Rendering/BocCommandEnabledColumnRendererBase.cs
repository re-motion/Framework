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
using System.Web.UI;
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Abstract base class for column renderers that can handle derived classes of <see cref="BocCommandEnabledColumnDefinition"/>.
  /// Defines common utility methods.
  /// </summary>
  /// <typeparam name="TBocColumnDefinition">The column definition class which the deriving class can handle.</typeparam>
  public abstract class BocCommandEnabledColumnRendererBase<TBocColumnDefinition> : BocColumnRendererBase<TBocColumnDefinition>
      where TBocColumnDefinition : BocCommandEnabledColumnDefinition
  {
    protected BocCommandEnabledColumnRendererBase (
        IResourceUrlFactory resourceUrlFactory,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(resourceUrlFactory, renderingFeatures, cssClasses, fallbackNavigationUrlProvider)
    {
    }

    protected void RenderCellIcon (BocColumnRenderingContext<TBocColumnDefinition> renderingContext, IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      IconInfo? icon = BusinessObjectBoundWebControl.GetIcon(businessObject, businessObject.BusinessObjectClass.BusinessObjectProvider);

      if (icon != null)
      {
        icon.Render(renderingContext.Writer, renderingContext.Control);
        renderingContext.Writer.Write(c_whiteSpace);
      }
    }

    protected bool RenderBeginTagDataCellCommand (
        BocColumnRenderingContext<TBocColumnDefinition> renderingContext,
        IBusinessObject businessObject,
        int originalRowIndex)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      BocListItemCommand? command = renderingContext.ColumnDefinition.Command;
      if (command == null)
        return false;

      if (string.IsNullOrEmpty(command.ItemID))
        command.ItemID = "Column_" + renderingContext.ColumnIndex + "_Command";

      bool isReadOnly = renderingContext.Control.IsReadOnly;
      bool isActive = command.Show == CommandShow.Always
                      || isReadOnly && command.Show == CommandShow.ReadOnly
                      || !isReadOnly && command.Show == CommandShow.EditMode;

      bool isCommandAllowed = (command.Type != CommandType.None) && !renderingContext.Control.EditModeController.IsRowEditModeActive;
      bool isCommandEnabled = (command.CommandState == null)
                              || command.CommandState.IsEnabled(renderingContext.Control, businessObject, renderingContext.ColumnDefinition);
      if (isActive && isCommandAllowed && isCommandEnabled)
      {
        string? objectID = null;
        IBusinessObjectWithIdentity? businessObjectWithIdentity = businessObject as IBusinessObjectWithIdentity;
        if (businessObjectWithIdentity != null)
          objectID = businessObjectWithIdentity.UniqueIdentifier;

        string argument = renderingContext.Control.GetListItemCommandArgument(
            renderingContext.ColumnIndex,
            new BocListRow(originalRowIndex, businessObject));
        string postBackEvent = renderingContext.Control.Page!.ClientScript.GetPostBackEventReference(renderingContext.Control, argument) + ";";
        string onClick = renderingContext.Control.HasClientScript ? OnCommandClickScript : string.Empty;
        if (command.Type == CommandType.None)
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.Disabled);

        var commandIDBackup = command.ItemID;
        try
        {
          command.ItemID = command.ItemID + "_Row_" + originalRowIndex;
          command.RenderBegin(
              renderingContext.Writer,
              RenderingFeatures,
              postBackEvent,
              onClick,
              originalRowIndex,
              objectID,
              businessObject as ISecurableObject);
        }
        finally
        {
          command.ItemID = commandIDBackup;
        }
        return true;
      }
      return false;
    }

    protected void RenderEndTagDataCellCommand (BocColumnRenderingContext<TBocColumnDefinition> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      Assertion.IsNotNull(renderingContext.ColumnDefinition.Command, "renderingContext.ColumnDefinition.Command must not be null.");

      renderingContext.ColumnDefinition.Command.RenderEnd(renderingContext.Writer);
    }
  }
}
