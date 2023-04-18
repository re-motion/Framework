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
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering cells of <see cref="BocDropDownMenuColumnDefinition"/> columns.
  /// </summary>
  [ImplementationFor(typeof(IBocValidationErrorIndicatorColumnRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocValidationErrorIndicatorColumnRenderer : BocColumnRendererBase<BocValidationErrorIndicatorColumnDefinition>, IBocValidationErrorIndicatorColumnRenderer
  {
    private readonly IInfrastructureResourceUrlFactory _infrastructureResourceUrlFactory;

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render, an <see cref="HtmlTextWriter"/> to render to, and a
    /// <see cref="BocDropDownMenuColumnDefinition"/> column for which to render cells.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocRowRenderer"/> should use a
    /// factory to obtain instances of this class.
    /// </remarks>
    public BocValidationErrorIndicatorColumnRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IRenderingFeatures renderingFeatures,
        BocListCssClassDefinition cssClasses,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider,
        IInfrastructureResourceUrlFactory infrastructureResourceUrlFactory)
        : base(resourceUrlFactory, renderingFeatures, cssClasses, fallbackNavigationUrlProvider)
    {
      _infrastructureResourceUrlFactory = infrastructureResourceUrlFactory;
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
    protected override void RenderCellContents (BocColumnRenderingContext<BocValidationErrorIndicatorColumnDefinition> renderingContext, in BocDataCellRenderArguments arguments)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      if (renderingContext.Control is IBocListWithValidationSupport bocListWithValidationSupport)
      {
        var validationFailureRepository = bocListWithValidationSupport.ValidationFailureRepository;
        if (validationFailureRepository.HasValidationFailures(arguments.BusinessObject))
        {
          var url = _infrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Image, "sprite.svg#ValidationError").GetUrl();
          var iconInfo = new IconInfo(url);

          iconInfo.Render(renderingContext.Writer, renderingContext.Control);
        }
      }
    }
  }
}
