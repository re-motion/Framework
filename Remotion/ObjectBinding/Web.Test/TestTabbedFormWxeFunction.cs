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
using Remotion.ObjectBinding.Sample;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace OBWTest
{

public class TestTabbedFormWxeFunction: WxeFunction
{
  public TestTabbedFormWxeFunction ()
    : base(new NoneTransactionMode())
  {
    Object = Person.GetObject(new Guid(0,0,0,0,0,0,0,0,0,0,1));
    ReturnUrl = "StartForm.aspx";
  }

  public TestTabbedFormWxeFunction (params object[] parameters)
    : base(new NoneTransactionMode(), parameters)
  {
    Object = Person.GetObject(new Guid(0,0,0,0,0,0,0,0,0,0,1));
  }

//  public TestTabbedFormWxeFunction (object Object, object ReadOnly, object Action)
//    : base (Object, ReadOnly, Action)
//  {
//  }

  public TestTabbedFormWxeFunction (object ReadOnly)
    : base(new NoneTransactionMode(), ReadOnly)
  {
  }

  // parameters

  public BindableXmlObject Object
  {
    get { return (BindableXmlObject)Variables["Object"]; }
    set { Variables["Object"] = value; }
  }

  [WxeParameter(1, true)]
  public bool ReadOnly
  {
    get { return (bool)Variables["ReadOnly"]; }
    set { Variables["ReadOnly"] = value; }
  }

//  [WxeParameter (3, false)]
//  public CnObject Action
//  {
//    get { return (CnObject) Variables["Action"]; }
//    set { Variables["Action"] = value; }
//  }

  // steps

  class Step1: WxeStepList
  {
    TestTabbedFormWxeFunction Function { get { return (TestTabbedFormWxeFunction)ParentFunction; } }

    WxeStep Step1_ = new WxePageStep("TestTabbedForm.aspx");
  }
}

}
