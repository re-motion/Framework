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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls.Rendering
{
  public static class LabelReferenceRendererExtensions
  {
    private static readonly string[] s_emptyAccessibilityAnnotationIDs = new string[0];

    public static void SetLabelReferenceOnControl (
        [NotNull] this ILabelReferenceRenderer labelReferenceRenderer,
        [NotNull] IAttributeAccessor attributeAccessor,
        [NotNull] IReadOnlyCollection<string> labelIDs
        )
    {
      ArgumentUtility.CheckNotNull ("labelReferenceRenderer", labelReferenceRenderer);
      ArgumentUtility.CheckNotNull ("attributeAccessor", attributeAccessor);
      ArgumentUtility.CheckNotNull ("labelIDs", labelIDs);

      labelReferenceRenderer.SetLabelsReferenceOnControl (attributeAccessor, labelIDs, s_emptyAccessibilityAnnotationIDs);
    }

    public static void AddLabelsReference (
        [NotNull] this ILabelReferenceRenderer labelReferenceRenderer,
        [NotNull] HtmlTextWriter htmlTextWriter,
        [NotNull] IReadOnlyCollection<string> labelIDs
        )
    {
      ArgumentUtility.CheckNotNull ("labelReferenceRenderer", labelReferenceRenderer);
      ArgumentUtility.CheckNotNull ("htmlTextWriter", htmlTextWriter);
      ArgumentUtility.CheckNotNull ("labelIDs", labelIDs);

      labelReferenceRenderer.AddLabelsReference (htmlTextWriter, labelIDs, s_emptyAccessibilityAnnotationIDs);
    }
  }
}
