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
using JetBrains.Annotations;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTreeViewImplementation.Rendering
{
  /// <summary>
  /// Currently this class provides only the <see cref="AddDiagnosticMetadataAttributes"/> method in order to prevent code duplication with
  /// <see cref="BocRendererBase{TControl}.AddDiagnosticMetadataAttributes"/>. It is planned, that some time in the future the
  /// <see cref="BocTreeView"/> control is completely rendererd using the renderer at hand.
  /// </summary>
  [ImplementationFor (typeof(IBocTreeViewRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocTreeViewRenderer : BocRendererBase<IBocTreeView>, IBocTreeViewRenderer
  {
    public BocTreeViewRenderer (
        [NotNull] IResourceUrlFactory resourceUrlFactory,
        [NotNull] IGlobalizationService globalizationService,
        [NotNull] IRenderingFeatures renderingFeatures)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    /// <summary>
    /// <see cref="BocTreeView"/> does not really use the <see cref="BocRendererBase{TControl}"/> infrastructure for rendering yet, however,  we do
    /// not want any code duplication with <see cref="BocRendererBase{TControl}.AddDiagnosticMetadataAttributes"/>, so we have to provide the
    /// <see cref="AddDiagnosticMetadataAttributes"/> method with public visibility.
    /// </summary>
    public new void AddDiagnosticMetadataAttributes (RenderingContext<IBocTreeView> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);
    }

    /// <summary>
    /// <see cref="BocTreeView"/> must not be bound to a specific property, it may also be bound to the business object as a whole. Therefore we
    /// overwrite the default implementation of <see cref="BocRendererBase{TControl}"/>.
    /// </summary>
    protected override bool IsBoundToBusinessObject (IBocTreeView control)
    {
      return control.DataSource != null && (control.DataSource.BusinessObject != null || control.DataSource.BusinessObjectClass != null);
    }

    public override string GetCssClassBase (IBocTreeView control)
    {
      return "bocTreeView";
    }
  }
}
