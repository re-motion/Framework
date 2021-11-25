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
  /// Default implementation of the <see cref="IValidationErrorRenderer" /> interface.
  /// </summary>
  /// <seealso cref="IValidationErrorRenderer"/>
  [ImplementationFor(typeof(IValidationErrorRenderer), Lifetime = LifetimeKind.Singleton)]
  public class ValidationErrorRenderer : IValidationErrorRenderer
  {
    private struct ValidationErrorsAttributeData
    {
      public string DescribedByAttributeValue;
      public string ValidationErrorsIDIndex;
    }

    private readonly IRenderingFeatures _renderingFeatures;

    public ValidationErrorRenderer ([NotNull] IRenderingFeatures renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("renderingFeatures", renderingFeatures);

      _renderingFeatures = renderingFeatures;
    }

    public void SetValidationErrorsReferenceOnControl (
        IAttributeAccessor attributeAccessor,
        string validationErrorID,
        IReadOnlyCollection<string> validationErrors)
    {
      ArgumentUtility.CheckNotNull("attributeAccessor", attributeAccessor);
      ArgumentUtility.CheckNotNullOrEmpty("validationErrorID", validationErrorID);
      ArgumentUtility.CheckNotNull("validationErrors", validationErrors);

      if (!validationErrors.Any())
        return;

      var describedByAttribute = HtmlTextWriterAttribute2.AriaDescribedBy;

      var describedByAttributeValue = GetValidationErrorsAttributeData(attributeAccessor.GetAttribute(describedByAttribute), validationErrorID);

      attributeAccessor.SetAttribute(describedByAttribute, describedByAttributeValue.DescribedByAttributeValue);

      if (_renderingFeatures.EnableDiagnosticMetadata)
        attributeAccessor.SetAttribute(DiagnosticMetadataAttributes.ValidationErrorIDIndex, describedByAttributeValue.ValidationErrorsIDIndex);

      attributeAccessor.SetAttribute(HtmlTextWriterAttribute2.AriaInvalid, HtmlAriaInvalidAttributeValue.True);
    }

    public void AddValidationErrorsReference (AttributeCollection attributeCollection, string validationErrorID, IReadOnlyCollection<string> validationErrors)
    {
      ArgumentUtility.CheckNotNull("attributeCollection", attributeCollection);
      ArgumentUtility.CheckNotNullOrEmpty("validationErrorID", validationErrorID);
      ArgumentUtility.CheckNotNull("validationErrors", validationErrors);

      if (!validationErrors.Any())
        return;

      var describedByAttributeName = HtmlTextWriterAttribute2.AriaDescribedBy;
      var describedByAttributeValue = GetValidationErrorsAttributeData(attributeCollection[describedByAttributeName], validationErrorID);

      attributeCollection[describedByAttributeName] = describedByAttributeValue.DescribedByAttributeValue;

      if (_renderingFeatures.EnableDiagnosticMetadata)
        attributeCollection[DiagnosticMetadataAttributes.ValidationErrorIDIndex] = describedByAttributeValue.ValidationErrorsIDIndex;

      attributeCollection[HtmlTextWriterAttribute2.AriaInvalid] = HtmlAriaInvalidAttributeValue.True;
    }

    public void RenderValidationErrors (HtmlTextWriter htmlTextWriter, string validationErrorID, IReadOnlyCollection<string> validationErrors)
    {
      ArgumentUtility.CheckNotNull("htmlTextWriter", htmlTextWriter);
      ArgumentUtility.CheckNotNullOrEmpty("validationErrorID", validationErrorID);
      ArgumentUtility.CheckNotNull("validationErrors", validationErrors);

      if (!validationErrors.Any())
        return;

      htmlTextWriter.AddAttribute(HtmlTextWriterAttribute.Id, validationErrorID);
      htmlTextWriter.AddAttribute(HtmlTextWriterAttribute2.Hidden, HtmlHiddenAttributeValue.Hidden);
      htmlTextWriter.RenderBeginTag(HtmlTextWriterTag.Span);

      foreach (var validationError in validationErrors)
      {
        htmlTextWriter.Write(validationError);
        htmlTextWriter.WriteBreak();
      }

      htmlTextWriter.RenderEndTag();
    }

    private ValidationErrorsAttributeData GetValidationErrorsAttributeData (string? attributeValue, string validationErrorsID)
    {
      if (string.IsNullOrEmpty(attributeValue))
      {
        return new ValidationErrorsAttributeData { DescribedByAttributeValue = validationErrorsID, ValidationErrorsIDIndex = "0" };
      }
      else
      {
        var attributeValueSplit = attributeValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var validationErrorsIDIndex = attributeValueSplit.Length;

        var describedByValue = string.Concat(attributeValue, " ", validationErrorsID);

        return new ValidationErrorsAttributeData { DescribedByAttributeValue = describedByValue, ValidationErrorsIDIndex = validationErrorsIDIndex.ToString() };
      }
    }
  }
}
