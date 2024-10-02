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
using Microsoft.Extensions.Logging;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.WebTestActions
{
  /// <summary>
  /// Fills a text box with a value.
  /// </summary>
  public class FillWithAction : WebTestAction
  {
    private readonly string _value;
    private readonly FinishInputWithAction _finishInputWithAction;

    public FillWithAction (
        [NotNull] ControlObject control,
        [NotNull] ElementScope scope,
        [NotNull] string value,
        [NotNull] FinishInputWithAction finishInputWithAction,
        [NotNull] ILogger logger)
        : base(control, scope, logger)
    {
      ArgumentUtility.CheckNotNull("value", value);
      ArgumentUtility.CheckNotNull("finishInputWithAction", finishInputWithAction);

      _value = value;
      _finishInputWithAction = finishInputWithAction;
    }

    /// <inheritdoc/>
    protected override string ActionName
    {
      get { return "FillWith"; }
    }

    /// <inheritdoc/>
    protected override void ExecuteInteraction (ElementScope scope)
    {
      ArgumentUtility.CheckNotNull("scope", scope);

      OutputDebugMessage(string.Format("New value: '{0}'", _value));
      scope.FillInWithFixed(_value, _finishInputWithAction, Logger);
    }
  }
}
