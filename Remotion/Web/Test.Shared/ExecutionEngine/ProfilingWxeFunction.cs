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
using System.Diagnostics;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.Test.Shared.ExecutionEngine
{
  public class ProfilingWxeFunction : WxeFunction
  {
    private DateTime _start;
    private DateTime _end;

    public ProfilingWxeFunction ()
        : base(new NoneTransactionMode())
    {
      ReturnUrl = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>().CreateResourceUrl(typeof(Start), TestResourceType.Root, "Start.aspx").GetUrl();
    }

    public ProfilingWxeFunction (params object[] args)
        : base(new NoneTransactionMode(), args)
    {
      ReturnUrl = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>().CreateResourceUrl(typeof(Start), TestResourceType.Root, "Start.aspx").GetUrl();
    }

    // steps

    private void Step10 ()
    {
      _start = DateTime.Now;
    }

    private WxeStep Step21 = new WxeResourcePageStep(typeof(ProfilingForm), "ExecutionEngine/ProfilingForm.aspx");
    private WxeStep Step22 = new WxeResourcePageStep(typeof(ProfilingForm), "ExecutionEngine/ProfilingForm.aspx");
    private WxeStep Step23 = new WxeResourcePageStep(typeof(ProfilingForm), "ExecutionEngine/ProfilingForm.aspx");
    private WxeStep Step24 = new WxeResourcePageStep(typeof(ProfilingForm), "ExecutionEngine/ProfilingForm.aspx");
    private WxeStep Step25 = new WxeResourcePageStep(typeof(ProfilingForm), "ExecutionEngine/ProfilingForm.aspx");
    private WxeStep Step26 = new WxeResourcePageStep(typeof(ProfilingForm), "ExecutionEngine/ProfilingForm.aspx");
    private WxeStep Step27 = new WxeResourcePageStep(typeof(ProfilingForm), "ExecutionEngine/ProfilingForm.aspx");
    private WxeStep Step28 = new WxeResourcePageStep(typeof(ProfilingForm), "ExecutionEngine/ProfilingForm.aspx");
    private WxeStep Step29 = new WxeResourcePageStep(typeof(ProfilingForm), "ExecutionEngine/ProfilingForm.aspx");

    // Tracing: 100ms/start-end
    // Profiling: 1sek/start-end
    private void Step30 ()
    {
      _end = DateTime.Now;
      TimeSpan diff = _end - _start;
      Debug.WriteLine(string.Format("Runtime: {0} ms", diff.Ticks / 10000));
    }
  }
}
