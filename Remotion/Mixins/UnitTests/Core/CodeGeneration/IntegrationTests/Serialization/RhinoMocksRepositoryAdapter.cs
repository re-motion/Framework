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
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.Serialization
{
  public class RhinoMocksRepositoryAdapter : IMockRepository
  {
    private readonly MockRepository _mockRepository;

    public RhinoMocksRepositoryAdapter ()
        : this (new MockRepository())
    {
    }

    public RhinoMocksRepositoryAdapter (MockRepository mockRepository)
    {
      ArgumentUtility.CheckNotNull ("mockRepository", mockRepository);
      _mockRepository = mockRepository;
    }

    public T StrictMock<T> (params object[] argumentsForConstructor)
    {
      return _mockRepository.StrictMock<T> (argumentsForConstructor);
    }

    public void ReplayAll ()
    {
      _mockRepository.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mockRepository.VerifyAll ();
    }

    public IDisposable Ordered ()
    {
      return _mockRepository.Ordered ();
    }

    public void LastCall_IgnoreArguments ()
    {
      LastCall.IgnoreArguments ();
    }
  }
}
