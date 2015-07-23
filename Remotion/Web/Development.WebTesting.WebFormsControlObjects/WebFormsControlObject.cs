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
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects
{
  /// <summary>
  /// Base class for control objects representing an ASP.NET WebForms control.
  /// </summary>
  public abstract class WebFormsControlObject : ControlObject
  {
    protected WebFormsControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    protected override ICompletionDetectionStrategy GetDefaultCompletionDetectionStrategy (ElementScope scope)
    {
      var webFormsPageObject = (IWebFormsPageObject) Context.PageObject;

      // Note: we assume that ASP.NET WebForms control objects have auto-postback enabled, that is no the default value, however, it is the 90% case
      // in re-motion-based projects.
      return webFormsPageObject.PostBackCompletionDetectionStrategy;
    }
  }
}