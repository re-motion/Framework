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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Test.ExecutionEngine
{
  [Serializable]
  public class SessionWxeFunction: WxeFunction
  {
    public SessionWxeFunction ()
      : base (new NoneTransactionMode ())
    {
    }

    public SessionWxeFunction (params object[] args)
        : base (new NoneTransactionMode(), args)
    {
    }

    [WxeParameter (0, true)]
    public bool ReadOnly
    {
      get { return (bool) Variables["ReadOnly"]; }
      set { Variables["ReadOnly"] = value; }
    }

    // steps

    void Step1()
    {
    }

    [Serializable]
    class Step2 : WxeStepList
    {
      SessionWxeFunction Function { get { return (SessionWxeFunction) ParentFunction; } }
      WxeStep Step1_ = new WxePageStep ("~/ExecutionEngine/SessionForm.aspx");
    }

    [Serializable]
    class Step3 : WxeStepList
    {
      SessionWxeFunction Function { get { return (SessionWxeFunction) ParentFunction; } }
      WxeStep Step1_ = new WxePageStep ("~/ExecutionEngine/SessionForm.aspx");
    }
  }
}
