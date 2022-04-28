// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using System;
using Moq;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  [TestFixture]
  public class WxeStepListTest : WxeTest
  {
    [Test]
    public void EvaluateDirtyState_WithoutSteps_ReturnsFalse ()
    {
      var stepList = new WxeStepList();

      Assert.That(stepList.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void EvaluateDirtyState_WithStepsButExecutionHasNotStarted_ReturnsFalse ()
    {
      var stepList = new WxeStepList();

      var notExecutedStepStub = new Mock<WxeStep>();
      notExecutedStepStub.Setup(_ => _.EvaluateDirtyState()).Throws(new AssertionException("Should not be called because step is not executed."));

      stepList.Add(notExecutedStepStub.Object);

      Assert.That(stepList.LastExecutedStep, Is.Null);

      Assert.That(stepList.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void EvaluateDirtyState_DirtyFromExecutedStep_ReturnsTrue ()
    {
      var stepList = new WxeStepList();

      var dirtyStepStub = new Mock<WxeStep>();
      dirtyStepStub.Setup(_ => _.EvaluateDirtyState()).Returns(true);

      stepList.Add(dirtyStepStub.Object);
      stepList.Execute(CurrentWxeContext);

      Assert.That(stepList.LastExecutedStep, Is.Null);
      dirtyStepStub.Verify(_=>_.EvaluateDirtyState(), Times.Once());

      dirtyStepStub.Setup(_ => _.EvaluateDirtyState()).Throws(new AssertionException("Should not be called after WxeStepList.Execute() has completed."));

      Assert.That(stepList.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void EvaluateDirtyState_DirtyFromSomeExecutedSteps_StopsAfterFirstDirtyStep ()
    {
      var stepList = new WxeStepList();

      var notDirtyStepStub = new Mock<WxeStep>();
      notDirtyStepStub.Setup(_ => _.EvaluateDirtyState()).Returns(false);

      var dirtyStepStub1 = new Mock<WxeStep>();
      dirtyStepStub1.Setup(_ => _.EvaluateDirtyState()).Returns(true);

      var dirtyStepStub2 = new Mock<WxeStep>();
      dirtyStepStub2.Setup(_ => _.EvaluateDirtyState()).Returns(true);

      stepList.Add(notDirtyStepStub.Object);
      stepList.Add(dirtyStepStub1.Object);
      stepList.Add(dirtyStepStub2.Object);
      stepList.Execute(CurrentWxeContext);

      Assert.That(stepList.LastExecutedStep, Is.Null);
      Assert.That(stepList.EvaluateDirtyState(), Is.True);

      notDirtyStepStub.Verify(_=>_.EvaluateDirtyState(), Times.Once());
      dirtyStepStub1.Verify(_=>_.EvaluateDirtyState(), Times.Once());
      dirtyStepStub2.Verify(_=>_.EvaluateDirtyState(), Times.Never());
    }

    [Test]
    public void EvaluateDirtyState_FutureStepIsNotEvaluated ()
    {
      var stepList = new WxeStepList();

      var dirtyStepStub = new Mock<WxeStep>();
      dirtyStepStub.Setup(_ => _.EvaluateDirtyState()).Returns(true);

      var notCompletedStepStub = new Mock<WxeStep>();
      notCompletedStepStub.Setup(_ => _.Execute(CurrentWxeContext)).Throws(new ApplicationException("Pause execution."));
      notCompletedStepStub.Setup(_ => _.EvaluateDirtyState()).Returns(false);

      var notExecutedStepStub = new Mock<WxeStep>();
      notExecutedStepStub.Setup(_ => _.Execute(CurrentWxeContext)).Throws(new AssertionException("Should not be called because step is not executed."));
      notExecutedStepStub.Setup(_ => _.EvaluateDirtyState()).Throws(new AssertionException("Should not be called because step is not executed."));

      stepList.Add(dirtyStepStub.Object);
      stepList.Add(notCompletedStepStub.Object);
      stepList.Add(notExecutedStepStub.Object);
      Assert.That(() => stepList.Execute(CurrentWxeContext), Throws.Exception.TypeOf<ApplicationException>());

      Assert.That(stepList.LastExecutedStep, Is.SameAs(notCompletedStepStub.Object));
      Assert.That(stepList.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void EvaluateDirtyState_WithIsDirtyStateEnabled_AndDirtyFromExecutedStep_ReturnsTrue ()
    {
      var stepList = new WxeStepList();
      var function = new TestFunction();
      stepList.SetParentStep(function);

      var dirtyStepStub = new Mock<WxeStep>();
      dirtyStepStub.Setup(_ => _.EvaluateDirtyState()).Returns(true);

      stepList.Add(dirtyStepStub.Object);
      stepList.Execute(CurrentWxeContext);

      Assert.That(stepList.LastExecutedStep, Is.Null);

      Assert.That(stepList.EvaluateDirtyState(), Is.True);
    }

    [Test]
    public void EvaluateDirtyState_WithIsDirtyStateDisabled_AndDirtyFromExecutedStep_ReturnsFalse ()
    {
      var stepList = new WxeStepList();
      var function = new TestFunction();
      stepList.SetParentStep(function);
      function.DisableDirtyState();

      var dirtyStepStub = new Mock<WxeStep>();
      dirtyStepStub.Setup(_ => _.EvaluateDirtyState()).Returns(true);

      stepList.Add(dirtyStepStub.Object);
      stepList.Execute(CurrentWxeContext);

      Assert.That(stepList.LastExecutedStep, Is.Null);

      Assert.That(stepList.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void ResetDirtyStateForExecutedSteps_ClearsDirtyStateRecursively_AndStopsAfterLastExecutedStep ()
    {
      var stepList = new WxeStepList();

      var dirtyStepStub = new Mock<WxeStep>();
      dirtyStepStub.Setup(_ => _.EvaluateDirtyState()).Returns(true);

      var notCompletedStepStub = new Mock<WxeStep>();
      notCompletedStepStub.Setup(_ => _.Execute(CurrentWxeContext)).Throws(new ApplicationException("Pause execution."));
      notCompletedStepStub.Setup(_ => _.EvaluateDirtyState()).Returns(false);

      stepList.Add(dirtyStepStub.Object);
      stepList.Add(notCompletedStepStub.Object);
      Assert.That(() => stepList.Execute(CurrentWxeContext), Throws.Exception.TypeOf<ApplicationException>());

      Assert.That(stepList.LastExecutedStep, Is.SameAs(notCompletedStepStub.Object));
      Assert.That(stepList.EvaluateDirtyState(), Is.True);

      stepList.ResetDirtyStateForExecutedSteps();

      dirtyStepStub.Setup(_ => _.EvaluateDirtyState()).Throws(new AssertionException("Should not be called after WxeStepList.Execute() has completed."));

      Assert.That(stepList.EvaluateDirtyState(), Is.False);
      dirtyStepStub.Verify(_=>_.ResetDirtyStateForExecutedSteps(), Times.Once());
      notCompletedStepStub.Verify(_=>_.ResetDirtyStateForExecutedSteps(), Times.Never());
    }

    [Test]
    public void IsDirtyStateEnabled_PassesCallToBaseImplementation_WithParentFunctionHasDirtyStateEnabled_ReturnsTrue ()
    {
      var stepList = new WxeStepList();
      var function = new TestFunction();
      stepList.SetParentStep(function);

      Assert.That(function.IsDirtyStateEnabled, Is.True);

      Assert.That(stepList.IsDirtyStateEnabled, Is.True);
    }

    [Test]
    public void IsDirtyStateEnabled_PassesCallToBaseImplementation_WithParentFunctionHasDirtyStateDisabled_ReturnsFalse ()
    {
      var stepList = new WxeStepList();
      var function = new TestFunction();
      stepList.SetParentStep(function);

      function.DisableDirtyState();
      Assert.That(function.IsDirtyStateEnabled, Is.False);

      Assert.That(stepList.IsDirtyStateEnabled, Is.False);
    }
  }
}
