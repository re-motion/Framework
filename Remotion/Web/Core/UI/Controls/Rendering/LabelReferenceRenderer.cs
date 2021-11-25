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
using System.Linq;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;

namespace Remotion.Web.UI.Controls.Rendering
{
  /// <summary>
  /// Default implementation of the <see cref="ILabelReferenceRenderer" /> interface.
  /// </summary>
  /// <seealso cref="ILabelReferenceRenderer"/>
  [ImplementationFor (typeof(ILabelReferenceRenderer), Lifetime = LifetimeKind.Singleton)]
  public class LabelReferenceRenderer : ILabelReferenceRenderer
  {
    private readonly IRenderingFeatures _renderingFeatures;

    public LabelReferenceRenderer ([NotNull] IRenderingFeatures renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);

      _renderingFeatures = renderingFeatures;
    }

    public void SetLabelsReferenceOnControl (
        IAttributeAccessor attributeAccessor,
        IReadOnlyCollection<string> labelIDs,
        IReadOnlyCollection<string> accessibilityAnnotationIDs)
    {
      ArgumentUtility.CheckNotNull("attributeAccessor", attributeAccessor);
      ArgumentUtility.CheckNotNull("labelIDs", labelIDs);
      ArgumentUtility.CheckNotNull("accessibilityAnnotationIDs", accessibilityAnnotationIDs);

      if (!labelIDs.Any() && !accessibilityAnnotationIDs.Any())
        return;

      var labelIDsJoined = string.Join(" ", labelIDs.Concat(accessibilityAnnotationIDs));
      attributeAccessor.SetAttribute(HtmlTextWriterAttribute2.AriaLabelledBy, labelIDsJoined);

      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        var nameLabelIndex = GetJoinedNameLabelIndex(labelIDs.Count);
        attributeAccessor.SetAttribute(DiagnosticMetadataAttributes.LabelIDIndex, nameLabelIndex);
      }
    }

    public void AddLabelsReference (
        HtmlTextWriter htmlTextWriter,
        IReadOnlyCollection<string> labelIDs,
        IReadOnlyCollection<string> accessibilityAnnotationIDs)
    {
      ArgumentUtility.CheckNotNull("htmlTextWriter", htmlTextWriter);
      ArgumentUtility.CheckNotNull("labelIDs", labelIDs);


      if (!labelIDs.Any() && !accessibilityAnnotationIDs.Any())
        return;

      var labelIDsJoined = string.Join(" ", labelIDs.Concat(accessibilityAnnotationIDs));
      htmlTextWriter.AddAttribute(HtmlTextWriterAttribute2.AriaLabelledBy, labelIDsJoined);

      if (_renderingFeatures.EnableDiagnosticMetadata)
      {
        var nameLabelIndex = GetJoinedNameLabelIndex(labelIDs.Count);
        htmlTextWriter.AddAttribute(DiagnosticMetadataAttributes.LabelIDIndex, nameLabelIndex);
      }
   }

    private string GetJoinedNameLabelIndex (int numberOfLabelIDs)
    {
      switch (numberOfLabelIDs)
      {
        case 1:
          return "0";
        case 2:
          return "0 1";
        default:
          var labelIDIndexes = Enumerable.Range(0, numberOfLabelIDs);
          return string.Join(" ", labelIDIndexes);
      }
    }
  }
}