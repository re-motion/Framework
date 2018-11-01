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
using Remotion.Collections;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{

[TestFixture]
public class WxeStepTest: WxeTest
{
  private TestFunction _rootFunction;
  private TestFunction _nestedLevel1Function;
  private TestFunction _nestedLevel2Function;
  private TestStep _rootFunctionStep;
  private TestStep _nestedLevel1FunctionStep;
  private TestStep _nestedLevel2FunctionStep;
  private TestStep _standAloneStep;

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();

    _rootFunction = new TestFunction();
    _nestedLevel1Function = new TestFunction();
    _nestedLevel2Function = new TestFunction();
    _rootFunctionStep = new TestStep();
    _nestedLevel1FunctionStep = new TestStep();
    _nestedLevel2FunctionStep = new TestStep();
    _standAloneStep = new TestStep();

    _rootFunction.Add (new TestStep());
    _rootFunction.Add (new TestStep());
    _rootFunction.Add (_nestedLevel1Function);
    _rootFunction.Add (_rootFunctionStep);

    _nestedLevel1Function.Add (new TestStep());
    _nestedLevel1Function.Add (new TestStep());
    _nestedLevel1Function.Add (_nestedLevel2Function);
    _nestedLevel1Function.Add (_nestedLevel1FunctionStep);

    _nestedLevel2Function.Add (new TestStep());
    _nestedLevel2Function.Add (new TestStep());
    _nestedLevel2Function.Add (_nestedLevel2FunctionStep);
  }

  [Test]
  public void GetStepByTypeForNull()
  {
    WxeStep step = TestStep.GetStepByType<WxeStep> (null);
    Assert.That (step, Is.Null);
  }

  [Test]
  public void GetStepByTypeForTestStep()
  {
    TestStep step = TestStep.GetStepByType<TestStep> (_standAloneStep);
    Assert.That (step, Is.SameAs (_standAloneStep));
  }

  [Test]
  public void GetStepByTypeForWxeFunction()
  {
    WxeFunction step = TestStep.GetStepByType<WxeFunction> (_nestedLevel2FunctionStep);
    Assert.That (step, Is.SameAs (_nestedLevel2Function));
  }

  [Test]
  public void GetStepByTypeForWrongType()
  {
    TestFunctionWithInvalidSteps step = TestStep.GetStepByType<TestFunctionWithInvalidSteps> (_nestedLevel2FunctionStep);
    Assert.That (step, Is.Null);
  }

  [Test]
  public void GetFunctionForStep()
  {
    WxeFunction function = WxeStep.GetFunction (_nestedLevel1FunctionStep);
    Assert.That (function, Is.SameAs (_nestedLevel1Function));
  }

  [Test]
  public void GetFunctionForNestedFunction()
  {
    WxeFunction function = WxeStep.GetFunction (_nestedLevel1Function);
    Assert.That (function, Is.SameAs (_nestedLevel1Function));
  }

  [Test]
  public void GetParentStepForStep()
  {
    WxeStep parentStep = _nestedLevel2FunctionStep.ParentStep;
    Assert.That (parentStep, Is.SameAs (_nestedLevel2Function));
  }

  [Test]
  public void GetParentFunctionForStep()
  {
    WxeFunction parentFunction = _nestedLevel2FunctionStep.ParentFunction;
    Assert.That (parentFunction, Is.SameAs (_nestedLevel2Function));
  }

  [Test]
  public void GetParentFunctionForNestedFunction()
  {
    WxeFunction parentFunction = _nestedLevel2Function.ParentFunction;
    Assert.That (parentFunction, Is.SameAs (_nestedLevel1Function));
  }

  [Test]
  public void GetParentStepForStandAloneStep()
  {
    WxeStep parentStep = _standAloneStep.ParentStep;
    Assert.That (parentStep, Is.Null);
  }

  [Test]
  public void GetParentFunctionForStandAloneStep()
  {
    WxeFunction parentFunction = _standAloneStep.ParentFunction;
    Assert.That (parentFunction, Is.Null);
  }

  [Test]
  public void GetRootFunctionForStep()
  {
    WxeFunction rootFunction = _nestedLevel2FunctionStep.RootFunction;
    Assert.That (rootFunction, Is.SameAs (_rootFunction));
  }

  [Test]
  public void GetRootFunctionForStandAloneStep()
  {
    WxeFunction rootFunction = _standAloneStep.RootFunction;
    Assert.That (rootFunction, Is.Null);
  }

  [Test]
  public void GetRootFunctionForRootFunction()
  {
    WxeFunction rootFunction = _rootFunction.RootFunction;
    Assert.That (rootFunction, Is.SameAs (_rootFunction));
  }

  [Test]
  public void AbortStep()
  {
    _standAloneStep.Abort ();
    Assert.That (_standAloneStep.IsAbortRecursiveCalled, Is.True);
    Assert.That (_standAloneStep.IsAborted, Is.True);
  }

  [Test]
  public void ExecuteStep()
  {
    _standAloneStep.Execute ();
    Assert.That (_standAloneStep.IsExecuteCalled, Is.True);
    Assert.That (_standAloneStep.WxeContext, Is.SameAs (CurrentWxeContext));
  }

  [Test]
  public void SetParentStep()
  {
    TestStep parentStep = new TestStep();
    _standAloneStep.SetParentStep (parentStep);
    Assert.That (_standAloneStep.ParentStep, Is.SameAs (parentStep));
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void SetParentStepNull()
  {
    _standAloneStep.SetParentStep (null);
  }

  [Test]
  public void GetVariablesForFunctionStep()
  {
    NameObjectCollection variables = _nestedLevel2FunctionStep.Variables;
    Assert.That (variables, Is.SameAs (_nestedLevel2Function.Variables));
  }

  [Test]
  public void GetVariablesForStandAloneStep()
  {
    NameObjectCollection variables = _standAloneStep.Variables;
    Assert.That (variables, Is.Null);
  }
}

}
