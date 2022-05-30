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
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Development.Web.UnitTesting.UI.Controls.Rendering
{
  /// <summary>
  /// Used to verify that <see cref="Control"/>s which render labels are using a <see cref="ILabelReferenceRenderer"/> to render label references.
  /// </summary>
  public class StubLabelReferenceRenderer : ILabelReferenceRenderer
  {
    public const string LabelReferenceAttribute = "fakeLabelReference";
    public const string AccessibilityAnnotationsAttribute = "fakeAccessibilityAnnotations";

    public void SetLabelsReferenceOnControl (
        IAttributeAccessor attributeAccessor,
        IReadOnlyCollection<string> labelIDs,
        IReadOnlyCollection<string> accessibilityAnnotationIDs)
    {
      ArgumentUtility.CheckNotNull("attributeAccessor", attributeAccessor);
      ArgumentUtility.CheckNotNull("labelIDs", labelIDs);
      ArgumentUtility.CheckNotNull("accessibilityAnnotationIDs", accessibilityAnnotationIDs);

      var labelIDsJoined = string.Join(" ", labelIDs);
      attributeAccessor.SetAttribute(LabelReferenceAttribute, labelIDsJoined);

      var accessibilityAnnotationIDsJoined = string.Join(" ", accessibilityAnnotationIDs);
      attributeAccessor.SetAttribute(AccessibilityAnnotationsAttribute, accessibilityAnnotationIDsJoined);
    }

    public void AddLabelsReference (
        HtmlTextWriter htmlTextWriter,
        IReadOnlyCollection<string> labelIDs,
        IReadOnlyCollection<string> accessibilityAnnotationIDs)
    {
      ArgumentUtility.CheckNotNull("htmlTextWriter", htmlTextWriter);
      ArgumentUtility.CheckNotNull("labelIDs", labelIDs);

      var labelIDsJoined = string.Join(" ", labelIDs);
      htmlTextWriter.AddAttribute(LabelReferenceAttribute, labelIDsJoined);

      var accessibilityAnnotationIDsJoined = string.Join(" ", accessibilityAnnotationIDs);
      htmlTextWriter.AddAttribute(AccessibilityAnnotationsAttribute, accessibilityAnnotationIDsJoined);
    }
  }
}
