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
using Remotion.Security.UnitTests.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.NullSecurityClientTests
{
  public class NullSecurityClientTestHelper
  {
    public static NullSecurityClientTestHelper CreateForStatelessSecurity()
    {
      return new NullSecurityClientTestHelper();
    }

    public static NullSecurityClientTestHelper CreateForStatefulSecurity()
    {
      return new NullSecurityClientTestHelper();
    }

    private MockRepository _mocks;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private SecurableObject _securableObject;

    private NullSecurityClientTestHelper()
    {
      _mocks = new MockRepository();
      _mockObjectSecurityStrategy = _mocks.StrictMock<IObjectSecurityStrategy>();
      
      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    public NullSecurityClient CreateSecurityClient()
    {
      return new NullSecurityClient();
    }

    public SecurableObject SecurableObject
    {
      get { return _securableObject; }
    }

    public void ReplayAll()
    {
      _mocks.ReplayAll();
    }

    public void VerifyAll()
    {
      _mocks.VerifyAll();
    }
  }
}
