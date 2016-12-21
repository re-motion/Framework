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
using System.Web;
using Remotion.ObjectBinding.Sample;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace OBWTest
{

public class ClientFormWxeFunction: WxeFunction
{
  public ClientFormWxeFunction ()
    : base (new NoneTransactionMode ())
  {
    Object = Person.GetObject (new Guid (0,0,0,0,0,0,0,0,0,0,1));
    SetExecutionCompletedScript ("javascript:window.close();");
  }

  // parameters
  public BindableXmlObject Object 
  {
    get { return (BindableXmlObject) Variables["Object"]; }
    set { Variables["Object"] = value; }
  }

  [WxeParameter (1, true)]
  public bool ReadOnly
  {
    get { return (bool) Variables["ReadOnly"]; }
    set { Variables["ReadOnly"] = value; }
  }

  // steps

  void Step1()
  {
    HttpContext.Current.Session["key"] = 123456789;
  }

  class Step2: WxeStepList
  {
    ClientFormWxeFunction Function { get { return (ClientFormWxeFunction) ParentFunction; } }
    WxeStep Step1_ = new WxePageStep ("ClientForm.aspx");
  }

  class Step3: WxeStepList
  {
    ClientFormWxeFunction Function { get { return (ClientFormWxeFunction) ParentFunction; } }
    WxeStep Step1_ = new WxePageStep ("ClientForm.aspx");
  }
}

public class ClientFormClosingWxeFunction: WxeFunction
{
  public ClientFormClosingWxeFunction ()
    : base (new NoneTransactionMode ())
  {
  }

  void Step1()
  {
    object val = HttpContext.Current.Session["key"];
    if (val != null)
    {
      int i = (int) val;
    }
  }
}

public class ClientFormKeepAliveWxeFunction: WxeFunction
{
  public ClientFormKeepAliveWxeFunction ()
    : base (new NoneTransactionMode ())
  {
  }

  void Step1()
  {
    object val = HttpContext.Current.Session["key"];
    if (val != null)
    {
      int i = (int) val;
    }
  }
}

}
