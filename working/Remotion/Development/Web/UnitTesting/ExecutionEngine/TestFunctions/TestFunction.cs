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

namespace Remotion.Development.Web.UnitTesting.ExecutionEngine.TestFunctions
{
  public class TestFunction : WxeFunction
  {
    public static readonly string Parameter1Name = "Parameter1";
    public static readonly string ReturnUrlValue = "DefaultReturn.html";

    private WxeContext _wxeContextStep2;
    private string _lastExecutedStepID;
    private string _executionOrder = string.Empty;

    public TestFunction ()
      :base (new NoneTransactionMode())
    {
      ReturnUrl = TestFunction.ReturnUrlValue;
    }

    public TestFunction (params object[] args)
        : base (new NoneTransactionMode(), args)
    {
      ReturnUrl = TestFunction.ReturnUrlValue;
    }

    [WxeParameter (1, false, WxeParameterDirection.In)]
    public string Parameter1
    {
      get { return (string) Variables["Parameter1"]; }
      set { Variables["Parameter1"] = value; }
    }

    private void Step1 ()
    {
      _lastExecutedStepID = "1";
    }

    private void Step2 (WxeContext context)
    {
      _wxeContextStep2 = context;
      _lastExecutedStepID = "2";
    }

    private TestStep Step3 = new TestStep();

    private void Step4 ()
    {
      _lastExecutedStepID = "4";
    }

    public void PublicStepMethod ()
    {
      Step1 ();
    }

    public void PublicStepMethodWithContext (WxeContext context)
    {
      Step2 (context);
    }

    public string LastExecutedStepID
    {
      get { return _lastExecutedStepID; }
    }

    public string ExecutionOrder
    {
      get { return _executionOrder; }
    }

    public TestStep TestStep
    {
      get { return Step3; }
    }

    public WxeContext WxeContextStep2
    {
      get { return _wxeContextStep2; }
    }
  }
}
