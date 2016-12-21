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
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ControlReplacing
{
  /// <summary>
  /// The <see cref="StateLoadingStrategy"/> type is used when the state of a <see cref="ControlReplacer"/>'s control tree should be 
  /// loaded during a regular page-lifecycle, i.e. when the view state is to be loaded without modification
  /// </summary>
  public class StateLoadingStrategy : IStateModificationStrategy
  {
    public void LoadControlState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      //NOP
    }

    public void LoadViewState (ControlReplacer replacer, IInternalControlMemberCaller memberCaller)
    {
      ArgumentUtility.CheckNotNull ("replacer", replacer);
      ArgumentUtility.CheckNotNull ("memberCaller", memberCaller);

      //NOP
    }
  }
}
