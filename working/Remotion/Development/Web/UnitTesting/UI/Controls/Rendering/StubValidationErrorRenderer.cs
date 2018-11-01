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
  /// Used to verify that <see cref="Control"/>s which render validation errors are using a <see cref="IValidationErrorRenderer"/> to render validation errors.
  /// </summary>
  public class StubValidationErrorRenderer : IValidationErrorRenderer
  {
    public const string ValidationErrorsIDAttribute = "fakeValidationErrorsReferenceIDAttribute";
    public const string ValidationErrorsAttribute = "fakeValidationErrorsReferenceAttribute";

    public void SetValidationErrorsReferenceOnControl (
        IAttributeAccessor attributeAccessor,
        string validationErrorID,
        IReadOnlyCollection<string> validationErrors)
    {
      ArgumentUtility.CheckNotNull ("attributeAccessor", attributeAccessor);
      ArgumentUtility.CheckNotNullOrEmpty ("validationErrorID", validationErrorID);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);

      attributeAccessor.SetAttribute (ValidationErrorsIDAttribute, validationErrorID);
      attributeAccessor.SetAttribute (ValidationErrorsAttribute, string.Join (" ", validationErrors));
    }

    public void AddValidationErrorsReference (
        AttributeCollection attributeCollection,
        string validationErrorID,
        IReadOnlyCollection<string> validationErrors)
    {
      ArgumentUtility.CheckNotNull ("attributeCollection", attributeCollection);
      ArgumentUtility.CheckNotNullOrEmpty ("validationErrorID", validationErrorID);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);

      attributeCollection[ValidationErrorsIDAttribute] = validationErrorID;
      attributeCollection[ValidationErrorsAttribute] = string.Join (" ", validationErrors);
    }

    public void RenderValidationErrors (HtmlTextWriter htmlTextWriter, string validationErrorID, IReadOnlyCollection<string> validationErrors)
    {
      ArgumentUtility.CheckNotNull ("htmlTextWriter", htmlTextWriter);
      ArgumentUtility.CheckNotNullOrEmpty ("validationErrorID", validationErrorID);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);

      htmlTextWriter.AddAttribute (ValidationErrorsIDAttribute, validationErrorID);
      htmlTextWriter.AddAttribute (ValidationErrorsAttribute, string.Join (" ", validationErrors));
      htmlTextWriter.RenderBeginTag ("fake");
      htmlTextWriter.RenderEndTag();
    }
  }
}