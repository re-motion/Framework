// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using Moq;
using NUnit.Framework;
using Remotion.Development.Moq.UnitTesting;

#nullable enable

namespace Remotion.UnitTests.Development.Moq.UnitTesting
{
  [TestFixture]
  public class DecoratorTestHelperTest
  {
    private DecoratorTestHelper<IMyInterface> _helperForDecorator = default!;
    private DecoratorTestHelper<IMyInterface> _helperForNonDelegatingDecorator = default!;
    private DecoratorTestHelper<IMyInterface> _helperForFaultyDecorator = default!;

    [SetUp]
    public void SetUp ()
    {
      _helperForDecorator = CreateDecoratorTestHelper(
          inner => inner.Get(),
          (inner, s) => inner.Do(s));

      _helperForNonDelegatingDecorator = CreateDecoratorTestHelper(inner => "Abc", (inner, s) => { });
      _helperForFaultyDecorator = CreateDecoratorTestHelper(
          inner =>
          {
            inner.Get();
            return "faulty";
          },
          (inner, s) => inner.Do("faulty"));
    }

    [Test]
    public void CheckDelegation_Func ()
    {
      Assert.That(() => _helperForDecorator.CheckDelegation(d => d.Get(), "Abc"), Throws.Nothing);
      Assert.That(
          () => _helperForNonDelegatingDecorator.CheckDelegation(d => d.Get(), "Abc"),
          Throws.TypeOf<MockException>().And.Message.Matches(
              @"Mock<DecoratorTestHelperTest\.IMyInterface:\d+>:\s*This mock failed verification due to the following:\s*DecoratorTestHelperTest\.IMyInterface d => d\.Get\(\):\s*This setup was not matched\."));
      Assert.That(
          () => _helperForFaultyDecorator.CheckDelegation(d => d.Get(), "Abc"),
          Throws.TypeOf<AssertionException>().And.Message.Contains("  Expected string length 3 but was 6. Strings differ at index 0."));
    }

    [Test]
    public void CheckDelegation_Func_MultipleCallsForSameMock ()
    {
      Assert.That(() => _helperForDecorator.CheckDelegation(d => d.Get(), "Abc"), Throws.Nothing);
      Assert.That(() => _helperForDecorator.CheckDelegation(d => d.Get(), "Abc"), Throws.Nothing);
    }

    [Test]
    public void CheckDelegation_Action ()
    {
      Assert.That(() => _helperForDecorator.CheckDelegation(d => d.Do("Abc")), Throws.Nothing);
      Assert.That(
          () => _helperForNonDelegatingDecorator.CheckDelegation(d => d.Do("Abc")),
          Throws.TypeOf<MockException>().And.Message.Matches(
              @"Mock<DecoratorTestHelperTest.IMyInterface:\d+>:\s*This mock failed verification due to the following:\s*DecoratorTestHelperTest\.IMyInterface d => d\.Do\(""Abc""\):"));
      Assert.That(
          () => _helperForFaultyDecorator.CheckDelegation(d => d.Do("Abc")),
          Throws.TypeOf<MockException>().And.Message.Matches(
              @"DecoratorTestHelperTest.IMyInterface.Do\(""faulty""\) invocation failed with mock behavior Strict.\s*All invocations on the mock must have a corresponding setup\."));
    }

    [Test]
    public void CheckDelegation_Action_MultipleCallsForSameMock ()
    {
      Assert.That(() => _helperForDecorator.CheckDelegation(d => d.Do("Abc")), Throws.Nothing);
      Assert.That(() => _helperForDecorator.CheckDelegation(d => d.Do("Abc")), Throws.Nothing);
      Assert.That(() => _helperForDecorator.CheckDelegation(d => d.Do("Abc")), Throws.Nothing);
    }

    [Test]
    public void CheckDelegation_Mixed_MultipleCallsForSameMock ()
    {
      Assert.That(() => _helperForDecorator.CheckDelegation(d => d.Do("Abc")), Throws.Nothing);
      Assert.That(() => _helperForDecorator.CheckDelegation(d => d.Get(), "test"), Throws.Nothing);
    }

    private DecoratorTestHelper<IMyInterface> CreateDecoratorTestHelper (Func<IMyInterface, string> getMethod, Action<IMyInterface, string> doMethod)
    {
      var innerMock = new Mock<IMyInterface>(MockBehavior.Strict);
      var decorator = new Decorator(innerMock.Object, getMethod, doMethod);

      return new DecoratorTestHelper<IMyInterface>(decorator, innerMock);
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
