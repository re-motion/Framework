// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using Moq;
using NUnit.Framework;
using Remotion.Development.Moq.UnitTesting.Threading;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Development.Moq.UnitTesting.Threading
{
  [TestFixture]
  public class LockingDecoratorTestHelperTest
  {
    private LockingDecoratorTestHelper<IMyInterface> _helperForLockingDecorator;
    private LockingDecoratorTestHelper<IMyInterface> _helperForNonLockingDecorator;
    private LockingDecoratorTestHelper<IMyInterface> _helperForNonDelegatingDecorator;
    private LockingDecoratorTestHelper<IMyInterface> _helperForFaultyDecorator;

    [SetUp]
    public void SetUp ()
    {
      var lockObject = new object();

      _helperForLockingDecorator = CreateLockingDecoratorTestHelper(
          inner =>
          {
            lock (lockObject)
              return inner.Get();
          },
          (inner, s) =>
          {
            lock (lockObject)
              inner.Do(s);
          },
          lockObject);

      _helperForNonLockingDecorator = CreateLockingDecoratorTestHelper(inner => inner.Get(), (inner, s) => inner.Do(s), lockObject);
      _helperForNonDelegatingDecorator = CreateLockingDecoratorTestHelper(inner => "Abc", (inner, s) => { }, lockObject);
      _helperForFaultyDecorator = CreateLockingDecoratorTestHelper(
          inner =>
          {
            lock (lockObject)
              inner.Get();
            return "faulty";
          },
          (inner, s) =>
          {
            lock (lockObject)
              inner.Do("faulty");
          },
          lockObject);
    }

    [Test]
    public void ExpectSynchronizedDelegation_Func ()
    {
      Assert.That(() => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Get(), "Abc"), Throws.Nothing);
      Assert.That(
          () => _helperForNonLockingDecorator.ExpectSynchronizedDelegation(d => d.Get(), "Abc"),
          Throws.TypeOf<AssertionException>().And.Message.StartsWith("  Parallel thread should have been blocked."));
      Assert.That(
          () => _helperForNonDelegatingDecorator.ExpectSynchronizedDelegation(d => d.Get(), "Abc"),
          Throws.TypeOf<MockException>()
              .And.Message.Matches(
                  "^Mock<LockingDecoratorTestHelperTest\\.IMyInterface:\\d+>:\n"
                  + "This mock failed verification due to the following:\r\n\r\n"
                  + "   LockingDecoratorTestHelperTest\\.IMyInterface d => d\\.Get\\(\\):\n"
                  + "   This setup was not matched\\.$"));
      Assert.That(
          () => _helperForFaultyDecorator.ExpectSynchronizedDelegation(d => d.Get(), "Abc"),
          Throws.TypeOf<AssertionException>().And.Message.Contains("  Expected string length 3 but was 6. Strings differ at index 0."));
    }

    [Test]
    public void ExpectSynchronizedDelegation_Func_MultipleCallsForSameMock ()
    {
      Assert.That(() => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Get(), "Abc"), Throws.Nothing);
      Assert.That(() => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Get(), "Abc"), Throws.Nothing);
    }

    [Test]
    public void ExpectSynchronizedDelegation_Func_WithResultChecker ()
    {
      Assert.That(
          () => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Get(), "Abc", s => Assert.That(s, Is.EqualTo("Abc"))),
          Throws.Nothing);
      Assert.That(
          () => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Get(), "Abc", s => Assert.That(s, Is.EqualTo("Expected"))),
          Throws.TypeOf<AssertionException>());
    }

    [Test]
    public void ExpectSynchronizedDelegation_Action ()
    {
      Assert.That(() => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Do("Abc")), Throws.Nothing);
      Assert.That(
          () => _helperForNonLockingDecorator.ExpectSynchronizedDelegation(d => d.Do("Abc")),
          Throws.TypeOf<AssertionException>().And.Message.StartsWith("  Parallel thread should have been blocked."));
      Assert.That(
          () => _helperForNonDelegatingDecorator.ExpectSynchronizedDelegation(d => d.Do("Abc")),
          Throws.TypeOf<MockException>()
              .And.Message.Matches(
                  "^Mock<LockingDecoratorTestHelperTest\\.IMyInterface:\\d+>:\n"
                  + "This mock failed verification due to the following:\r\n\r\n"
                  + "   LockingDecoratorTestHelperTest\\.IMyInterface d => d\\.Do\\(\"Abc\"\\):\n"
                  + "   This setup was not matched\\.$"));
      Assert.That(
          () => _helperForFaultyDecorator.ExpectSynchronizedDelegation(d => d.Do("Abc")),
          Throws.TypeOf<MockException>()
              .And.Message.Matches(
                  "^Mock<LockingDecoratorTestHelperTest\\.IMyInterface:\\d+>:\n"
                  + "This mock failed verification due to the following:\r\n\r\n"
                  + "   LockingDecoratorTestHelperTest\\.IMyInterface d => d\\.Do\\(\"Abc\"\\):\n"
                  + "   This setup was not matched\\.$"));
    }

    [Test]
    public void ExpectSynchronizedDelegation_Action_MultipleCallsForSameMock ()
    {
      Assert.That(() => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Do("Abc")), Throws.Nothing);
      Assert.That(() => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Do("Abc")), Throws.Nothing);
      Assert.That(() => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Do("Abc")), Throws.Nothing);
    }

    [Test]
    public void ExpectSynchronizedDelegation_Mixed_MultipleCallsForSameMock ()
    {
      Assert.That(() => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Do("Abc")), Throws.Nothing);
      Assert.That(() => _helperForLockingDecorator.ExpectSynchronizedDelegation(d => d.Get(), "test"), Throws.Nothing);
    }

    private LockingDecoratorTestHelper<IMyInterface> CreateLockingDecoratorTestHelper (
        Func<IMyInterface, string> getMethod, Action<IMyInterface, string> doMethod, object lockObject)
    {
      var innerMock = new Mock<IMyInterface>();
      var decorator = new Decorator(innerMock.Object, getMethod, doMethod);

      return new LockingDecoratorTestHelper<IMyInterface>(decorator, lockObject, innerMock);
    }

    public interface IMyInterface
    {
      string Get ();
      void Do (string s);
    }

    private class Decorator : IMyInterface
    {
      private readonly IMyInterface _inner;
      private readonly Func<IMyInterface, string> _getMethod;
      private readonly Action<IMyInterface, string> _doMethod;

      public Decorator (IMyInterface inner, Func<IMyInterface, string> getMethod, Action<IMyInterface, string> doMethod)
      {
        _inner = inner;
        _getMethod = getMethod;
        _doMethod = doMethod;
      }

      public string Get ()
      {
        return _getMethod(_inner);
      }

      public void Do (string s)
      {
        _doMethod(_inner, s);
      }
    }
  }
}
