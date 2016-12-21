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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Web.IntegrationTests.TestDomain;
using Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class DomainObjectParametersTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void CreateNoneTransactionMode_FunctionCanUseObjectsFromOuterTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var outerTransaction = ClientTransaction.Current;

        var inParameter = SampleObject.NewObject ();
        var inParameterArray = new[] { SampleObject.NewObject () };
        inParameter.Int32Property = 7;
        inParameterArray[0].Int32Property = 8;

        SampleObject outParameter;
        SampleObject[] outParameterArray;
        ExecuteDelegateInWxeFunctionWithParameters (WxeTransactionMode<ClientTransactionFactory>.None, (ctx, f) =>
        {
          var clientTransaction1 = f.Transaction.GetNativeTransaction<ClientTransaction> ();
          Assert.That (clientTransaction1, Is.Null);
          Assert.That (ClientTransaction.Current, Is.SameAs (outerTransaction));

          Assert.That (outerTransaction.IsEnlisted (f.InParameter), Is.True);
          Assert.That (outerTransaction.IsEnlisted (f.InParameterArray[0]));

          Assert.That (f.InParameter.Int32Property, Is.EqualTo (7));
          Assert.That (f.InParameterArray[0].Int32Property, Is.EqualTo (8));

          f.OutParameter = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<SampleObject> ();
          f.OutParameter.Int32Property = 12;

          f.OutParameterArray = new[] { DomainObjectIDs.ClassWithAllDataTypes2.GetObject<SampleObject> () };
          f.OutParameterArray[0].Int32Property = 13;
        }, inParameter, inParameterArray, out outParameter, out outParameterArray);

        // Since everything within the function occurred in the same transaction that called the function, all enlisted objects are the same
        // and all changes are visible after the function call.
        Assert.That (ClientTransaction.Current.IsEnlisted (outParameter), Is.True);
        Assert.That (outParameter.Int32Property, Is.EqualTo (12));

        Assert.That (ClientTransaction.Current.IsEnlisted (outParameterArray[0]), Is.True);
        Assert.That (outParameterArray[0].Int32Property, Is.EqualTo (13));
      }
    }

    [Test]
    public void CreateNoneTransactionMode_FunctionCannotUseParametersNotFromOuterTransaction ()
    {
      var objectFromOtherTransaction =
          (SampleObject) LifetimeService.GetObject (ClientTransaction.CreateRootTransaction(), DomainObjectIDs.ClassWithAllDataTypes1, true);

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var inParameter = objectFromOtherTransaction;
        var inParameterArray = new[] { objectFromOtherTransaction };

        Assert.That (ClientTransaction.Current.IsEnlisted (objectFromOtherTransaction), Is.False);

        SampleObject outParameter;
        SampleObject[] outParameterArray;
        ExecuteDelegateInWxeFunctionWithParameters (WxeTransactionMode<ClientTransactionFactory>.None, (ctx, f) =>
        {
          Assert.That (ClientTransaction.Current.IsEnlisted (f.InParameter), Is.False);
          Assert.That (ClientTransaction.Current.IsEnlisted (f.InParameterArray[0]), Is.False);
          f.OutParameter = f.InParameter;
          f.OutParameterArray = new[] { f.InParameterArray[0] };
        },
                                                    inParameter,
                                                    inParameterArray,
                                                    out outParameter,
                                                    out outParameterArray);

        Assert.That (ClientTransaction.Current.IsEnlisted (outParameter), Is.False);
        Assert.That (ClientTransaction.Current.IsEnlisted (outParameterArray[0]), Is.False);
      }
    }

    [Test]
    public void CreateRootTransactionMode_InParametersOfDomainObjectType_CauseException ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var inParameter = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<SampleObject> ();
        var inParameterArray = new[] { DomainObjectIDs.ClassWithAllDataTypes2.GetObject<SampleObject> () };

        inParameter.Int32Property = 7;
        inParameterArray[0].Int32Property = 8;

        var function = new DomainObjectParameterTestTransactedFunction (
            WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) => { }, inParameter, inParameterArray);
        Assert.That (
            () => function.Execute (Context),
            Throws.TypeOf<WxeException>()
                  .With.Message.StringStarting (
                      "One or more of the input parameters passed to the WxeFunction are incompatible with the function's transaction. "
                      + "The following objects are incompatible with the target transaction: ")
                  .And.Message.StringContaining (DomainObjectIDs.ClassWithAllDataTypes1.ToString())
                  .And.Message.StringContaining (DomainObjectIDs.ClassWithAllDataTypes1.ToString())
                  .And.Message.StringEnding (". Objects of type 'Remotion.Data.DomainObjects.IDomainObjectHandle`1[T]' could be used instead."));
      }
    }

    [Test]
    public void CreateRootTransactionMode_OutParametersOfDomainObjectType_WithoutSurroundingFunction ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        SampleObject outParameter;
        SampleObject[] outParameterArray;
        ExecuteDelegateInWxeFunctionWithParameters (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
        {
          f.OutParameter = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<SampleObject> ();
          f.OutParameter.Int32Property = 12;

          f.OutParameterArray = new[] { DomainObjectIDs.ClassWithAllDataTypes2.GetObject<SampleObject> () };
          f.OutParameterArray[0].Int32Property = 13;
        }, null, null, out outParameter, out outParameterArray);


        Assert.That (ClientTransaction.Current.IsEnlisted (outParameter), Is.False);
        Assert.That (ClientTransaction.Current.IsEnlisted (outParameterArray[0]), Is.False);
      }
    }

    [Test]
    public void CreateRootTransactionMode_OutParametersOfDomainObjectType_WithSurroundingFunction ()
    {
      SampleObject outParameter;
      SampleObject[] outParameterArray;

      var wxeUnhandledException = Assert.Throws<WxeUnhandledException> (
          () => ExecuteDelegateInSubWxeFunctionWithParameters (
              WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
              WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
              (ctx, f) =>
              {
                f.OutParameter = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<SampleObject>();
                f.OutParameter.Int32Property = 12;

                f.OutParameterArray = new[] { DomainObjectIDs.ClassWithAllDataTypes2.GetObject<SampleObject>() };
                f.OutParameterArray[0].Int32Property = 13;
              },
              null,
              null,
              out outParameter,
              out outParameterArray));

      Assert.That (
          wxeUnhandledException.Message,
          Is.EqualTo (
              "An exception ocured while executing WxeFunction "
              + "'Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions.TransactedFunctionWithChildFunction'."));

      Assert.That (wxeUnhandledException.InnerException, Is.InstanceOf<WxeUnhandledException>());
      Assert.That (
          wxeUnhandledException.InnerException.Message,
          Is.EqualTo (
              "An exception ocured while executing WxeFunction "
              + "'Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions.DomainObjectParameterTestTransactedFunction'."));

      Assert.That (wxeUnhandledException.InnerException.InnerException, Is.Not.Null);
      Assert.That (
          wxeUnhandledException.InnerException.InnerException.Message,
          Is.StringStarting (
              "One or more of the output parameters returned from the WxeFunction are incompatible with the function's parent transaction. "
              + "The following objects are incompatible with the target transaction: ")
            .And.StringContaining (DomainObjectIDs.ClassWithAllDataTypes1.ToString())
            .And.StringContaining (DomainObjectIDs.ClassWithAllDataTypes2.ToString())
            .And.StringEnding (". Objects of type 'Remotion.Data.DomainObjects.IDomainObjectHandle`1[T]' could be used instead."));
    }

    [Test]
    public void CreateChildTransactionMode_InAndOutParametersCanBeUsed ()
    {
      ExecuteDelegateInWxeFunction (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (parentCtx, parentF) =>
          {
            var inParameter = SampleObject.NewObject ();
            var inParameterArray = new[] { SampleObject.NewObject () };
            inParameter.Int32Property = 7;
            inParameterArray[0].Int32Property = 8;

            var parentTransaction = parentF.Transaction.GetNativeTransaction<ClientTransaction> ();

            var subFunction = new DomainObjectParameterTestTransactedFunction (
                WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
                (ctx, f) =>
                {
                  var clientTransaction = f.Transaction.GetNativeTransaction<ClientTransaction> ();
                  Assert.That (clientTransaction, Is.Not.Null.And.SameAs (ClientTransaction.Current));
                  Assert.That (clientTransaction, Is.Not.SameAs (parentTransaction));
                  Assert.That (clientTransaction.ParentTransaction, Is.SameAs (parentTransaction));

                  Assert.That (clientTransaction.IsEnlisted (f.InParameter), Is.True);
                  Assert.That (clientTransaction.IsEnlisted (f.InParameterArray[0]));

                  // Since this function is running in a subtransaction, the properties set in the parent transaction are visible from here.
                  Assert.That (f.InParameter.Int32Property, Is.EqualTo (7));
                  Assert.That (f.InParameterArray[0].Int32Property, Is.EqualTo (8));

                  // Since this function is running in a subtransaction, out parameters are visible within the parent function if the transaction is 
                  // committed.
                  f.OutParameter = SampleObject.NewObject ();
                  f.OutParameter.Int32Property = 17;
                  f.OutParameterArray = new[] { SampleObject.NewObject (), SampleObject.NewObject () };
                  f.OutParameterArray[0].Int32Property = 4;

                  ClientTransaction.Current.Commit ();

                  f.OutParameterArray[1].Int32Property = 5;
                },
                inParameter,
                inParameterArray);

            subFunction.SetParentStep (parentF);
            subFunction.Execute (parentCtx);

            var outParameter = subFunction.OutParameter;
            var outParameterArray = subFunction.OutParameterArray;

            Assert.That (parentTransaction.IsEnlisted (outParameter), Is.True);
            Assert.That (outParameter.Int32Property, Is.EqualTo (17));
            Assert.That (parentTransaction.IsEnlisted (outParameterArray[0]), Is.True);
            Assert.That (outParameterArray[0].Int32Property, Is.EqualTo (4));
            Assert.That (parentTransaction.IsEnlisted (outParameterArray[1]), Is.True);
            Assert.That (outParameterArray[1].Int32Property, Is.Not.EqualTo (4));
          });
    }

    [Test]
    public void CreateChildTransactionMode_WithNonLoadableInParameter ()
    {
      ExecuteDelegateInWxeFunction (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (parentCtx, parentF) =>
          {
            var inParameter = SampleObject.NewObject ();
            inParameter.Delete ();

            var inParameterArray = new[] { SampleObject.NewObject () };
            inParameterArray[0].Delete ();

            var subFunction = new DomainObjectParameterTestTransactedFunction (
                WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
                (ctx, f) =>
                {
                  Assert.That (f.InParameter.State, Is.EqualTo (StateType.Invalid));
                  Assert.That (() => f.InParameter.EnsureDataAvailable (), Throws.TypeOf<ObjectInvalidException> ());

                  Assert.That (f.InParameterArray[0].State, Is.EqualTo (StateType.Invalid));
                  Assert.That (() => f.InParameterArray[0].EnsureDataAvailable (), Throws.TypeOf<ObjectInvalidException> ());
                },
                inParameter,
                inParameterArray);

            subFunction.SetParentStep (parentF);
            subFunction.Execute (parentCtx);
          });
    }

    [Test]
    public void CreateChildTransactionMode_WithNonLoadableOutParameter ()
    {
      ExecuteDelegateInWxeFunction (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          (parentCtx, parentF) =>
          {
            var subFunction = new DomainObjectParameterTestTransactedFunction (
                WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
                (ctx, f) =>
                {
                  f.OutParameter = SampleObject.NewObject();
                  f.OutParameterArray = new[] { SampleObject.NewObject() };
                },
                null,
                null);

            subFunction.SetParentStep (parentF);
            subFunction.Execute (parentCtx);

            var parentTransaction = parentF.Transaction.GetNativeTransaction<ClientTransaction>();
            Assert.That (parentTransaction.IsEnlisted (subFunction.OutParameter), Is.True);
            Assert.That (subFunction.OutParameter.State, Is.EqualTo (StateType.Invalid));
            Assert.That (() => subFunction.OutParameter.EnsureDataAvailable(), Throws.TypeOf<ObjectInvalidException>());

            Assert.That (parentTransaction.IsEnlisted (subFunction.OutParameterArray[0]), Is.True);
            Assert.That (subFunction.OutParameterArray[0].State, Is.EqualTo (StateType.Invalid));
            Assert.That (() => subFunction.OutParameterArray[0].EnsureDataAvailable(), Throws.TypeOf<ObjectInvalidException>());
          });
    }

    [Test]
    public void PassingDomainObjectsAround_BetweenTransactions_WithDomainObjectHandles ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var inParameter = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<SampleObject> ();
        inParameter.Int32Property = 7;

        var function = new DomainObjectHandleParameterTestTransactedFunction (
            WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
            (ctx, f) =>
            {
              var inParameterInFunction = f.InParameter.GetObject ();
              Assert.That (inParameterInFunction, Is.Not.SameAs (inParameter));
              Assert.That (inParameterInFunction.ID, Is.EqualTo (DomainObjectIDs.ClassWithAllDataTypes1));
              Assert.That (inParameterInFunction.Int32Property, Is.Not.EqualTo (7));
              f.OutParameter = DomainObjectIDs.ClassWithAllDataTypes2.GetHandle<SampleObject> ();
              f.OutParameter.GetObject ().Int32Property = 8;
            },
            inParameter.GetHandle ());
        function.Execute (Context);

        Assert.That (function.OutParameter.ObjectID, Is.EqualTo (DomainObjectIDs.ClassWithAllDataTypes2));
        var outParameter = function.OutParameter.GetObject ();
        Assert.That (outParameter.Int32Property, Is.Not.EqualTo (8));
      }
    }
  }
}