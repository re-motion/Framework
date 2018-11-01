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
using NUnit.Framework;
using Remotion.Development.RhinoMocks.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Development.RhinoMocks.UnitTesting
{
  [TestFixture]
  public class DecoratorTestHelperTest
  {
    private DecoratorTestHelper<IMyInterface> _helperForDecorator;
    private DecoratorTestHelper<IMyInterface> _helperForNonDelegatingDecorator;
    private DecoratorTestHelper<IMyInterface> _helperForFaultyDecorator;

    [SetUp]
    public void SetUp ()
    {
      _helperForDecorator = CreateDecoratorTestHelper (
          inner => inner.Get (),
          (inner, s) => inner.Do (s));

      _helperForNonDelegatingDecorator = CreateDecoratorTestHelper (inner => "Abc", (inner, s) => { });
      _helperForFaultyDecorator = CreateDecoratorTestHelper (
          inner =>
          {
            inner.Get ();
            return "faulty";
          },
          (inner, s) => inner.Do ("faulty"));
    }

    [Test]
    public void CheckDelegation_Func ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Get (), "Abc"), Throws.Nothing);
      Assert.That (
          () => _helperForNonDelegatingDecorator.CheckDelegation (d => d.Get (), "Abc"),
          Throws.TypeOf<ExpectationViolationException> ().And.Message.EqualTo ("IMyInterface.Get(); Expected #1, Actual #0."));
      Assert.That (
          () => _helperForFaultyDecorator.CheckDelegation (d => d.Get (), "Abc"),
          Throws.TypeOf<AssertionException> ().And.Message.StringStarting ("  Expected string length 3 but was 6. Strings differ at index 0."));
    }

    [Test]
    public void CheckDelegation_Func_MultipleCallsForSameMock ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Get (), "Abc"), Throws.Nothing);
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Get (), "Abc"), Throws.Nothing);
    }

    [Test]
    public void CheckDelegation_Action ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (
          () => _helperForNonDelegatingDecorator.CheckDelegation (d => d.Do ("Abc")),
          Throws.TypeOf<ExpectationViolationException> ().And.Message.EqualTo ("IMyInterface.Do(\"Abc\"); Expected #1, Actual #0."));
      Assert.That (
          () => _helperForFaultyDecorator.CheckDelegation (d => d.Do ("Abc")),
          Throws.TypeOf<ExpectationViolationException> ().And.Message.EqualTo (
              "IMyInterface.Do(\"faulty\"); Expected #0, Actual #1.\r\nIMyInterface.Do(\"Abc\"); Expected #1, Actual #0."));
    }

    [Test]
    public void CheckDelegation_Action_MultipleCallsForSameMock ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
    }

    [Test]
    public void CheckDelegation_Mixed_MultipleCallsForSameMock ()
    {
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Do ("Abc")), Throws.Nothing);
      Assert.That (() => _helperForDecorator.CheckDelegation (d => d.Get (), "test"), Throws.Nothing);
    }

    private DecoratorTestHelper<IMyInterface> CreateDecoratorTestHelper (Func<IMyInterface, string> getMethod, Action<IMyInterface, string> doMethod)
    {
      var innerMock = MockRepository.GenerateStrictMock<IMyInterface> ();
      var decorator = new Decorator (innerMock, getMethod, doMethod);

      return new DecoratorTestHelper<IMyInterface> (decorator, innerMock);
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
        return _getMethod (_inner);
      }

      public void Do (string s)
      {
        _doMethod (_inner, s);
      }
    }
  }
}