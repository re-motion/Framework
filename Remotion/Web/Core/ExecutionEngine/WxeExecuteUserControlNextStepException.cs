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

namespace Remotion.Web.ExecutionEngine
{
  /// <summary> This exception is used by the execution engine to end the execution of a <see cref="WxeUserControlStep"/>. </summary>
  public class WxeExecuteUserControlNextStepException : WxeExecutionControlException
  {
    public WxeExecuteUserControlNextStepException ()
      : base(
      "This exception does not indicate an error. It is used to roll back the call stack. "
      + "It is recommended to disable breaking on this exeption type while debugging."
      )
    {
    }
  }
}
