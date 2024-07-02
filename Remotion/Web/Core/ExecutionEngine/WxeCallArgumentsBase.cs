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
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  public abstract class WxeCallArgumentsBase : IWxeCallArguments
  {
    private readonly IWxeCallOptions _options;

    protected WxeCallArgumentsBase (IWxeCallOptions options)
    {
      ArgumentUtility.CheckNotNull("options", options);

      _options = options;
    }

    public IWxeCallOptions Options
    {
      get { return _options; }
    }

    protected virtual void Dispatch (IWxeExecutor executor, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull("executor", executor);
      ArgumentUtility.CheckNotNull("function", function);

      //_options.Dispatch (executor, function, _sender);
    }

    void IWxeCallArguments.Dispatch (IWxeExecutor executor, WxeFunction function)
    {
      Dispatch(executor, function);
    }
  }
}
