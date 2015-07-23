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
using System.Threading.Tasks;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.UnitTests
{
  [TestFixture]
  public class DoubleCheckedLockingContainerTest
  {
    public interface IFactory
    {
      SampleClass Create();
    }

    public class SampleClass
    {
    }

    private MockRepository _mocks;

    [SetUp]
    public void SetUp()
    {
      _mocks = new MockRepository();
    }

    [Test]
    public void SetAndGetValue()
    {
      SampleClass expected = new SampleClass();
      DoubleCheckedLockingContainer<SampleClass> container =
          new DoubleCheckedLockingContainer<SampleClass> (delegate { throw new NotImplementedException(); });

      container.Value = expected;
      Assert.That (container.Value, Is.SameAs (expected));
    }

    [Test]
    public void GetValueFromFactory()
    {
      SampleClass expected = new SampleClass();
      IFactory mockFactory = _mocks.StrictMock<IFactory>();
      DoubleCheckedLockingContainer<SampleClass> container =
          new DoubleCheckedLockingContainer<SampleClass> (delegate { return mockFactory.Create(); });
      Expect.Call (mockFactory.Create()).Return (expected);
      _mocks.ReplayAll();

      SampleClass actual = container.Value;

      _mocks.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void SetNull()
    {
      SampleClass expected = new SampleClass ();
      IFactory mockFactory = _mocks.StrictMock<IFactory> ();
      DoubleCheckedLockingContainer<SampleClass> container =
          new DoubleCheckedLockingContainer<SampleClass> (delegate { return mockFactory.Create (); });
      _mocks.ReplayAll ();

      container.Value = null;

      _mocks.VerifyAll ();

      _mocks.BackToRecordAll ();
      Expect.Call (mockFactory.Create ()).Return (expected);

      _mocks.ReplayAll ();

      SampleClass actual = container.Value;

      _mocks.VerifyAll ();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void HasValue ()
    {
      SampleClass expected = new SampleClass ();
      IFactory mockFactory = _mocks.StrictMock<IFactory> ();
      DoubleCheckedLockingContainer<SampleClass> container =
          new DoubleCheckedLockingContainer<SampleClass> (delegate { return mockFactory.Create (); });

      _mocks.ReplayAll ();

      Assert.That (container.HasValue, Is.False);

      _mocks.VerifyAll ();

      _mocks.BackToRecordAll ();
      Expect.Call (mockFactory.Create ()).Return (expected);

      _mocks.ReplayAll ();

      SampleClass actual = container.Value;

      Assert.That (container.HasValue, Is.True);
      _mocks.VerifyAll ();

      container.Value = null;
      Assert.That (container.HasValue, Is.False);
    }
  }
}
