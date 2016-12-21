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
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// This control object represents controls (or areas within a control) which do not have any additional features than hosting other
  /// controls (<see cref="IControlHost"/>). Typcially this control object is returned by other control objects in order to scope into a specific
  /// area (e.g. top controls or bottom controls in <see cref="T:Remotion.Web.Development.WebTesting.ControlObjects.TabbedMultiViewControlObject"/>.
  /// </summary>
  public class ScopeControlObject : ControlObject, IControlHost
  {
    public ScopeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl (controlSelectionCommand);
    }

    /// <inheritdoc/>
    protected override ICompletionDetectionStrategy GetDefaultCompletionDetectionStrategy (ElementScope scope)
    {
      throw new NotSupportedException ("The ScopeControlObject does not support any interaction.");
    }
  }
}