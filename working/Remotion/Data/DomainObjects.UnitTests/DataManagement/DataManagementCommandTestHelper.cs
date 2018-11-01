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
using Remotion.Data.DomainObjects.DataManagement;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  public static class DataManagementCommandTestHelper
  {
    public static void ExpectNotifyAndPerform (IDataManagementCommand commandMock)
    {
      using (commandMock.GetMockRepository ().Ordered ())
      {
        commandMock.Expect (mock => mock.Begin());
        commandMock.Expect (mock => mock.Begin());
        commandMock.Expect (mock => mock.Perform());
        commandMock.Expect (mock => mock.End());
        commandMock.Expect (mock => mock.End());
      }
    }

    public static void AssertNotifyAndPerformWasCalled (IDataManagementCommand commandMock)
    {
      commandMock.AssertWasCalled (mock => mock.Begin ());
      commandMock.AssertWasCalled (mock => mock.Begin ());
      commandMock.AssertWasCalled (mock => mock.Perform ());
      commandMock.AssertWasCalled (mock => mock.End ());
      commandMock.AssertWasCalled (mock => mock.End ());
    }
  }
}