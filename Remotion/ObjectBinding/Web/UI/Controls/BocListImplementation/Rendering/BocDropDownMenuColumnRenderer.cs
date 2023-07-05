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
using System.Web.UI;
using Remotion.Mixins;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering cells of <see cref="BocDropDownMenuColumnDefinition"/> columns.
  /// </summary>
  [ImplementationFor(typeof(IBocDropDownMenuColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocDropDownMenuColumnRenderer : BocColumnRendererBase<BocDropDownMenuColumnDefinition>, IBocDropDownMenuColumnRenderer
  {
    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocDropDownMenuColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// factory to obtain instances of this class.
    /// </remarks>
    public BocDropDownMenuColumnRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(resourceUrlFactory, renderingFeatures, cssClasses, fallbackNavigationUrlProvider)
    {
    }

    /// <summary>
    /// Renders a <see cref="DropDownMenu"/> with the options for the current row.
    /// <seealso cref="BocColumnRendererBase{TBocColumnDefinition}.RenderCellContents"/>
    /// </summary>
    /// <remarks>
    /// The menu title is generated from the <see cref="DropDownMenu.TitleText"/> and <see cref="DropDownMenu.TitleText"/> properties of
    /// the column definition in <see cref="BocColumnRenderingContext.ColumnDefinition"/>, and populated with the menu items in
    /// the <see cref="IBocList.RowMenus"/> property.
    /// </remarks>
    protected override void RenderCellContents (BocColumnRenderingContext<BocDropDownMenuColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      if (renderingContext.Control.RowMenus.Count <= arguments.RowIndex)
      {
        renderingContext.Writer.Write(c_whiteSpace);
        return;
      }

      var dropDownMenu = renderingContext.Control.RowMenus[arguments.RowIndex];

      if (renderingContext.Control.HasClientScript)
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Onclick, OnCommandClickScript);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div); // Begin div

      dropDownMenu.Enabled = !renderingContext.Control.EditModeController.IsRowEditModeActive;

      dropDownMenu.ShowTitle = renderingContext.ColumnDefinition.ShowMenuTitle;
      dropDownMenu.TitleText = renderingContext.ColumnDefinition.MenuTitleText;
      dropDownMenu.TitleIcon = renderingContext.ColumnDefinition.MenuTitleIcon;

      if (!string.IsNullOrEmpty(renderingContext.Control.ControlServicePath))
      {
        var stringValueParametersDictionary = new Dictionary<string, string?>();
        stringValueParametersDictionary.Add("controlID", renderingContext.Control.ID);
        stringValueParametersDictionary.Add(
            "controlType",
            TypeUtility.GetPartialAssemblyQualifiedName(MixinTypeUtility.GetUnderlyingTargetType(renderingContext.Control.GetType())));
        stringValueParametersDictionary.Add("businessObjectClass", renderingContext.BusinessObjectWebServiceContext.BusinessObjectClass);
        stringValueParametersDictionary.Add("businessObjectProperty", renderingContext.BusinessObjectWebServiceContext.BusinessObjectProperty);
        stringValueParametersDictionary.Add("businessObject", renderingContext.BusinessObjectWebServiceContext.BusinessObjectIdentifier);
        stringValueParametersDictionary.Add("rowIndex", arguments.ListIndex.ToString());
        stringValueParametersDictionary.Add(
            "rowBusinessObject",
            (arguments.BusinessObject as IBusinessObjectWithIdentity)?.UniqueIdentifier);
        stringValueParametersDictionary.Add("arguments", renderingContext.BusinessObjectWebServiceContext.Arguments);

        dropDownMenu.SetLoadMenuItemStatus(
            renderingContext.Control.ControlServicePath,
            nameof(IBocListWebService.GetMenuItemStatusForRowMenu),
            stringValueParametersDictionary);
      }

      dropDownMenu.RenderControl(renderingContext.Writer);

      renderingContext.Writer.RenderEndTag(); // End div
    }

    protected override void AddDiagnosticMetadataAttributes (BocCellAttributeRenderingContext<BocDropDownMenuColumnDefinition> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);

      renderingContext.AddAttributeToRender(DiagnosticMetadataAttributesForObjectBinding.BocListWellKnownRowDropDownMenuCell, "true");
    }
  }
}
