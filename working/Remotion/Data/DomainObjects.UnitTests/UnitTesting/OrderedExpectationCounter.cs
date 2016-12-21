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
using NUnit.Framework;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.UnitTesting
{
  /// <summary>
  /// Provides support for ordered Rhino.Mocks expectations without dedicated <see cref="MockRepository"/> instance. 
  /// See <see cref="MethodOptionsExtensions.Ordered{T}"/> for details.
  /// </summary>
  public class OrderedExpectationCounter
  {
    private int _nextExpectedPosition;
    private int _currentPosition;

    public int GetNextExpectedPosition ()
    {
      return _nextExpectedPosition++;
    }

    public void CheckPosition (string operationName, int expectedPosition, string message = null)
    {
      Assert.That (
          _currentPosition,
          Is.EqualTo (expectedPosition),
          "Expected operation '{0}' at sequence number {1}, but was {2}. {3}",
          operationName,
          expectedPosition,
          _currentPosition,
          message);
      ++_currentPosition;
    }
  }
}