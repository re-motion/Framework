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

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for a context menu based on <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/>.
  /// </summary>
  public class ContextMenuControlObject : DropDownMenuControlObjectBase
  {
    public ContextMenuControlObject ([NotNull] ControlObjectContext context)
        : base(context)
    {
    }

    [Obsolete("Use the Open() method instead. (Version 1.17.15.0)", false)]
    protected void OpenDropDownMenu ()
    {
      Open();
    }

    /// <inheritdoc/>
    protected override void PerformOpen (ElementScope menuButtonScope)
    {
      menuButtonScope.ContextClick(Context, Logger);
    }
  }
}
