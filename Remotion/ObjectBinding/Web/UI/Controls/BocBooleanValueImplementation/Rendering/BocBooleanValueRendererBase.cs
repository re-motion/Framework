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
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering
{
  /// <summary>
  /// Base class for the renderers in the same namespace. Contains common constants and methods.
  /// <seealso cref="BocBooleanValueRenderer"/>
  /// <seealso cref="BocCheckBoxRenderer"/>
  /// </summary>
  /// <typeparam name="TControl">The concrete control or corresponding interface that will be rendered.</typeparam>
  public abstract class BocBooleanValueRendererBase<TControl> : BocRendererBase<TControl>
      where TControl : IBocBooleanValueBase
  {
    protected BocBooleanValueRendererBase (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    /// <inheritdoc />
    protected override bool UseThemingContext
    {
      get { return true; }
    }

    protected IEnumerable<PlainTextString> GetValidationErrorsToRender (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      return renderingContext.Control.GetValidationErrors();
    }

    protected string GetValidationErrorsID (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      return renderingContext.Control.ClientID + "_ValidationErrors";
    }
  }
}
