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
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Default implementation of <see cref="IStyleInformation"/>, suitable for most control objects.
  /// </summary>
  public class DefaultStyleInformation : IStyleInformation
  {
    private readonly ControlObject _controlObject;
    private readonly ElementScope _styledScope;

    public DefaultStyleInformation ([NotNull] ControlObject controlObject, [NotNull] ElementScope styledScope)
    {
      ArgumentUtility.CheckNotNull("controlObject", controlObject);
      ArgumentUtility.CheckNotNull("styledScope", styledScope);

      _controlObject = controlObject;
      _styledScope = styledScope;
    }

    /// <inheritdoc/>
    public bool HasCssClass (string cssClass)
    {
      ArgumentUtility.CheckNotNullOrEmpty("cssClass", cssClass);

      return _styledScope["class"].Split(' ').Contains(cssClass);
    }

    /// <inheritdoc/>
    public WebColor GetBackgroundColor ()
    {
      return _styledScope.GetComputedBackgroundColor(_controlObject.Logger);
    }

    /// <inheritdoc/>
    public WebColor GetTextColor ()
    {
      return _styledScope.GetComputedTextColor(_controlObject.Logger);
    }
  }
}
