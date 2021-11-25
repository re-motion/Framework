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

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for the default <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/>.
  /// </summary>
  public class DropDownMenuControlObject : DropDownMenuControlObjectBase
  {
    public DropDownMenuControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Returns the button type of the DropDownMenu's button.
    /// </summary>
    public ButtonType GetButtonType ()
    {
      return (ButtonType)Enum.Parse(typeof(ButtonType), Scope[DiagnosticMetadataAttributes.ButtonType]);
    }

    [Obsolete("Use the Open() method instead. (Version 1.17.15.0)", false)]
    protected void OpenDropDownMenu ()
    {
      Open();
    }

    protected override void PerformOpen (ElementScope menuButtonScope)
    {
      menuButtonScope.Click();
    }
  }
}
