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

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public class NullUserControlExecutor
      : IUserControlExecutor
  {
    public static readonly IUserControlExecutor Null = new NullUserControlExecutor();

    private NullUserControlExecutor ()
    {
    }

    public bool IsNull
    {
      get { return true; }
    }

    public void Execute (WxeContext context)
    {
      //NOP
    }

    public WxeFunction Function
    {
      // TODO RM-8118: Check if this could be made conditional through NRT attributes instead of throwing an exception
      get { throw new InvalidOperationException("Function must not be accessed on the null object."); }
    }

    public string? BackedUpUserControlState
    {
      get { return null; }
    }

    public string? BackedUpUserControl
    {
      get { return null; }
    }

    public string UserControlID
    {
      // TODO RM-8118: Check if this could be made conditional through NRT attributes instead of throwing an exception
      get { throw new InvalidOperationException("UserControlID must not be accessed on the null object."); }
    }

    public bool IsReturningPostBack
    {
      get { return false; }
    }

    public WxeStep? ExecutingStep
    {
      get { return null; }
    }
  }
}
