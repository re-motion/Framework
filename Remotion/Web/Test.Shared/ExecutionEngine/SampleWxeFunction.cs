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
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.ExecutionEngine.Obsolete;

namespace Remotion.Web.Test.Shared.ExecutionEngine
{
  public class SampleWxeFunction : WxeFunction, ISampleFunctionVariables
  {
    public SampleWxeFunction ()
        : base(new NoneTransactionMode())
    {
      ReturnUrl = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>().CreateResourceUrl(typeof(Start), TestResourceType.Root, "Start.aspx").GetUrl();
    }

    // parameters and local variables

    public string Var1
    {
      get { return (string)Variables["Var1"]; }
      set { Variables["Var1"] = value; }
    }

    public string Var2
    {
      get { return (string)Variables["Var2"]; }
      set { Variables["Var2"] = value; }
    }

    // steps

    private void Step1 ()
    {
      Var1 = "SampleWxeFunction Step1";
      Var2 = "Var2 - Step1";
    }

    private WxeStep Step2 = new WxeResourcePageStep(typeof(WebForm1), "ExecutionEngine/WebForm1.aspx");
    private WxeStep Step3 = new SampleWxeSubFunction(varref("Var2"), "constant for Var2");
    private WxeStep Step4 = new WxeResourcePageStep(typeof(WebForm1), "ExecutionEngine/WebForm1.aspx");
  }

  public class SampleWxeSubFunction : WxeFunction, ISampleFunctionVariables
  {
    public SampleWxeSubFunction ()
        : base(new NoneTransactionMode())
    {
    }

    public SampleWxeSubFunction (params object[] args)
        : base(new NoneTransactionMode(), args)
    {
    }

    public SampleWxeSubFunction (string var1, string var2)
        : base(new NoneTransactionMode(), var1, var2)
    {
    }

    // parameters and local variables

    [WxeParameter(1, true, WxeParameterDirection.InOut)]
    public string Var1
    {
      get { return (string)Variables["Var1"]; }
      set { Variables["Var1"] = value; }
    }

    [WxeParameter(2, true, WxeParameterDirection.In)]
    public string Var2
    {
      get { return (string)Variables["Var2"]; }
      set { Variables["Var2"] = value; }
    }

    // steps

    private class Step1 : WxeTryCatch
    {
      private class Try : WxeStepList
      {
        private SampleWxeSubFunction Function
        {
          get { return (SampleWxeSubFunction)ParentFunction; }
        }

        private void Step1 (WxeContext context)
        {
          // Var1 = "SampleWxeSubFunction Step1";
        }

        private WxeStep Step2 = new WxeResourcePageStep(typeof(WebForm1), "ExecutionEngine/WebForm1.aspx");

        private void Step3 (WxeContext context)
        {
          Function.Var1 = "SampleWxeSubFunction Step3";
        }

        private WxeStep Step4 = new WxeResourcePageStep(typeof(WebForm1), "ExecutionEngine/WebForm1.aspx");

        private void Step5 ()
        {
          Function.Var1 = "exit SampleWxeSubFunction";
        }
      }

      [WxeException(typeof(ApplicationException))]
      private class Catch1 : WxeCatchBlock
      {
        private SampleWxeSubFunction Function
        {
          get { return (SampleWxeSubFunction)ParentFunction; }
        }

        private class Step1 : WxeIf
        {
          private SampleWxeSubFunction Function
          {
            get { return (SampleWxeSubFunction)ParentFunction; }
          }

          private bool If ()
          {
            return CurrentException.Message != null && CurrentException.Message.Length > 0;
          }

          private class Then : WxeStepList
          {
            private SampleWxeSubFunction Function
            {
              get { return (SampleWxeSubFunction)ParentFunction; }
            }

            private void Step1 ()
            {
              Function.Var1 = CurrentException.Message;
            }

            private WxeStep Step2 = new WxeResourcePageStep(typeof(WebForm1), "ExecutionEngine/WebForm1.aspx");
          }
        }

        private void Step2 (WxeContext context)
        {
          Function.Var1 = "Exception caught.";
        }

        private WxeStep Step3 = new WxeResourcePageStep(typeof(WebForm1), "ExecutionEngine/WebForm1.aspx");
      }

      private class Finally : WxeStepList
      {
        private SampleWxeSubFunction Function
        {
          get { return (SampleWxeSubFunction)ParentFunction; }
        }

        private void Step1 ()
        {
          Function.Var2 = "finally";
        }

        private WxeStep Step2 = new WxeResourcePageStep(typeof(WebForm1), "ExecutionEngine/WebForm1.aspx");
      }
    }
  }

  /// <summary>
  /// This interface exists so that WebForm1.aspx can access both SampleWxeFunction and 
  /// SampleWxeSubFunction variables in a type safe way.
  /// Outside of demo scenarios, this would usually not make sense.
  /// </summary>
  public interface ISampleFunctionVariables
  {
    string Var1 { get; set; }
    string Var2 { get; set; }
  }
}
