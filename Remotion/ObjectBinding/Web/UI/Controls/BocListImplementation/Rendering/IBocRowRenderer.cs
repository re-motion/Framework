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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Interface for classes responsible for rendering a single row of the table in a <see cref="IBocList"/> control.
  /// </summary>
 public interface IBocRowRenderer
  {
    /// <summary> Renders the table row containing the column titles and sorting buttons. </summary>
    /// <remarks> Title format: &lt;span&gt;label button &lt;span&gt;sort order&lt;/span&gt;&lt;/span&gt; </remarks>
    void RenderTitlesRow (BocListRenderingContext renderingContext);

    /// <summary>
    /// Renders a row containing the empty list message in <see cref="IBocList"/>'s 
    /// <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.EmptyListMessage"/> property.
    /// </summary>
    /// <remarks>
    /// If the property is not set, a default message will be loaded from the resource file, using 
    /// <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier.EmptyListMessage"/> as key. 
    /// </remarks>
    void RenderEmptyListDataRow (BocListRenderingContext renderingContext);

    /// <summary>Renders a table row containing the data of the <see cref="IBusinessObject"/> wrapped inside <paramref name="rowRenderingContext"/>. </summary>
    /// <param name="renderingContext">The <see cref="BocListRenderingContext"/>.</param>
    /// <param name="rowRenderingContext"> The <see cref="BocListRowRenderingContext"/> contains all data about the row being rendererd. </param>
    /// <param name="arguments">The row-specific rendering arguments.</param>
    void RenderDataRow (BocListRenderingContext renderingContext, BocListRowRenderingContext rowRenderingContext, in BocRowRenderArguments arguments);
  }
}
