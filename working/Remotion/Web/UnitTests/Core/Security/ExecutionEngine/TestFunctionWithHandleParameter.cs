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
using Remotion.Web.UnitTests.Core.Security.Domain;

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
{
  public class TestFunctionWithHandleParameter : WxeFunction
  {
    public TestFunctionWithHandleParameter (params object[] actualParameters)
        : base(new NoneTransactionMode(), actualParameters)
    {
    }

    [WxeParameter (0, false, WxeParameterDirection.In)]
    public IHandle<SecurableObject> HandleWithSecurableObject 
    {
      get { return (IHandle<SecurableObject>) Variables["HandleWithSecurableObject"]; }
      set { Variables["HandleWithSecurableObject"] = value; }
    }

    [WxeParameter (1, false, WxeParameterDirection.In)]
    public IHandle<object> HandleWithNonSecurableObject
    {
      get { return (IHandle<object>) Variables["HandleWithNonSecurableObject"]; }
      set { Variables["HandleWithNonSecurableObject"] = value; }
    }

    [WxeParameter (2, false, WxeParameterDirection.In)]
    public HandleInheritingAttribute<SecurableObject> HandleInheritingAttribute
    {
      get { return (HandleInheritingAttribute<SecurableObject>) Variables["HandleInheritingAttribute"]; }
      set { Variables["HandleInheritingAttribute"] = value; }
    }
  }
}