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
using Coypu;
using JetBrains.Annotations;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Base class for control objects representing Remotion WebForms controls which features diagnostic metadata rendering.
  /// </summary>
  public abstract class WebFormsControlObjectWithDiagnosticMetadata : ControlObject
  {
    protected WebFormsControlObjectWithDiagnosticMetadata ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    protected override ICompletionDetectionStrategy GetDefaultCompletionDetectionStrategy (ElementScope scope)
    {
      if (scope[DiagnosticMetadataAttributes.TriggersPostBack] != null)
      {
        var hasAutoPostBack = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersPostBack]);
        if (hasAutoPostBack)
          return ((IWebFormsPageObject) Context.PageObject).PostBackCompletionDetectionStrategy;
      }

      if (scope[DiagnosticMetadataAttributes.TriggersNavigation] != null)
      {
        var triggersNavigation = bool.Parse (scope[DiagnosticMetadataAttributes.TriggersNavigation]);
        if (triggersNavigation)
          return ((IWebFormsPageObject) Context.PageObject).NavigationCompletionDetectionStrategy;
      }

      return Continue.Immediately;
    }
  }
}