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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebFormsControlObjects;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an arbitrary re-motion-based business object control.
  /// </summary>
  public abstract class BocControlObject : WebFormsControlObjectWithDiagnosticMetadata
  {
    protected BocControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    protected abstract ElementScope GetLabeledElementScope ();

    /// <summary>
    /// Returns whether the control is read-only.
    /// </summary>
    public bool IsReadOnly ()
    {
      return Scope.GetAttribute(DiagnosticMetadataAttributes.IsReadOnly, Logger) == "true";
    }

    /// <summary>
    /// Returns whether the control is enabled.
    /// </summary>
    public bool IsDisabled ()
    {
      return Scope.GetAttribute(DiagnosticMetadataAttributes.IsDisabled, Logger) == "true";
    }

    /// <summary>
    /// Returns the validation errors for the <param name="scope" />.
    /// </summary>
    /// <exception cref="MissingHtmlException">Thrown if the validation errors cannot be found due to faulty markup or missing validator.</exception>
    protected IReadOnlyList<string> GetValidationErrors ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      if (IsReadOnly())
        throw AssertionExceptionUtility.CreateControlReadOnlyException(Driver);

      return GetValidationErrorsInner(scope);
    }

    /// <summary>
    /// Returns the validation errors for the read-only <param name="scope" />.
    /// </summary>
    /// <exception cref="MissingHtmlException">Thrown if the validation errors cannot be found due to faulty markup or missing validator.</exception>
    protected IReadOnlyList<string> GetValidationErrorsForReadOnly ([NotNull] ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      if (!IsReadOnly())
        throw AssertionExceptionUtility.CreateControlNotReadOnlyException(Driver);

      return GetValidationErrorsInner(scope);
    }

    private IReadOnlyList<string> GetValidationErrorsInner (ElementScope scope)
    {
      if (IsValid(scope))
        return new List<string>();

      string describedBy;

      try
      {
        describedBy = scope.GetAttribute("aria-describedby", Logger);
      }
      catch (MissingHtmlException)
      {
        throw CreateMissingValidationErrorFieldException();
      }

      var validationErrorIDs = describedBy.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      var validationErrorIndex = int.Parse(scope.GetAttribute(DiagnosticMetadataAttributes.ValidationErrorIDIndex, Logger));
      var validationErrorsScope = Scope.FindId(validationErrorIDs[validationErrorIndex]);

      // re-motion renders the delimiter as <br />, but the browser (and Selenium) show it as <br>
      const string validationErrorDelimiter = "<br>";

      try
      {
        return validationErrorsScope.InnerHTML.Split(new[] { validationErrorDelimiter }, StringSplitOptions.RemoveEmptyEntries);
      }
      catch (MissingHtmlException)
      {
        throw CreateMissingValidationErrorFieldException();
      }
    }

    private MissingHtmlException CreateMissingValidationErrorFieldException ()
    {
      return new MissingHtmlException(
          $"Could not find validation error field of the control object with the ID '{GetHtmlID()}'. This could be due to wrong markup or a missing validator.");
    }

    private static bool IsValid (ElementScope scope)
    {
      return scope["aria-invalid"] != "true";
    }

    /// <summary>
    /// Returns the label for this control.
    /// </summary>
    public IReadOnlyList<LabelControlObject> GetLabels ()
    {
      var scope = GetLabeledElementScope();

      var labelledBy = scope["aria-labelledby"];

      if (string.IsNullOrEmpty(labelledBy))
        return new List<LabelControlObject>();

      var labelID = labelledBy.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      var nameLabelIndex = scope[DiagnosticMetadataAttributes.LabelIDIndex];

      if (string.IsNullOrEmpty(nameLabelIndex))
        return new List<LabelControlObject>();

      var nameLabelIndexes = nameLabelIndex.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      var labels = new List<LabelControlObject>();
      foreach (var index in nameLabelIndexes)
      {
        var indexAsInt = Int32.Parse(index);
        labels.Add(new LabelControlObject(Context.CloneForControl(Context.RootScope.FindId(labelID[indexAsInt]))));
      }

      return labels;
    }
  }
}
