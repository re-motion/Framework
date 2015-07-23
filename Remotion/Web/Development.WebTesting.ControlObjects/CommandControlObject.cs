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
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a <see cref="T:Remotion.Web.UI.Controls.Command"/>.
  /// </summary>
  public class CommandControlObject : WebFormsControlObjectWithDiagnosticMetadata, IClickableControlObject
  {
    public CommandControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject Click (IWebTestActionOptions actionOptions = null)
    {
      var actualActionOptions = MergeWithDefaultActionOptions (Scope, actionOptions);
      new ClickAction (this, Scope).Execute (actualActionOptions);
      return UnspecifiedPage();
    }
  }
}