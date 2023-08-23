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
// using NUnit.Framework;
//
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Wraps an instance of the <see cref="IValidator" /> interface and evaluates the error message during the render phase.
  /// </summary>
  public class LazyEvaluatedValidationMessageControl : HtmlContainerControl
  {
    private readonly IValidator _validator;

    /// <summary>Initializes a new instance of the <see cref="LazyEvaluatedValidationMessageControl" /> class using the specified tag name and validator.</summary>
    /// <param name="tag">A string that specifies the tag name of the control. </param>
    /// <param name="validator">The validator for which the error message should be displayed.</param>
    public LazyEvaluatedValidationMessageControl (string tag, IValidator validator)
        : base(tag)
    {
      ArgumentUtility.CheckNotNull(nameof(tag), tag);
      ArgumentUtility.CheckNotNull(nameof(validator), validator);

      _validator = validator;
    }

    protected override void RenderChildren (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull(nameof(writer), writer);

      PlainTextString.CreateFromText(_validator.ErrorMessage).WriteTo(writer);
    }
  }
}
