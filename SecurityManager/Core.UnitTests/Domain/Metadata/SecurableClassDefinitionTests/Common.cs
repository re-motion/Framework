// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata.SecurableClassDefinitionTests
{
  [TestFixture]
  public class Common : DomainTest
  {
    [Test]
    public void StateProperties_Empty ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();

        Assert.That(orderClass.StateProperties, Is.Empty);
      }
    }

    [Test]
    public void StateProperties_OrderStateAndPaymentState ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties();

        Assert.That(orderClass.StateProperties.Count, Is.EqualTo(AccessControlTestHelper.OrderClassPropertyCount));
      }
    }

    [Test]
    public void StateProperties_IsNotCached ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties();

        var firstCollection = orderClass.StateProperties;
        var secondCollection = orderClass.StateProperties;

        Assert.That(secondCollection, Is.Not.SameAs(firstCollection));
      }
    }

    [Test]
    public void StateProperties_IsReadOnly ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties();

        Assert.That(((ICollection<StatePropertyDefinition>)orderClass.StateProperties).IsReadOnly, Is.True);
      }
    }

    [Test]
    public void AccessTypes_Empty ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();

        Assert.That(orderClass.AccessTypes, Is.Empty);
      }
    }

    [Test]
    public void AccessTypes_OneAccessType ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        testHelper.AttachJournalizeAccessType(orderClass);

        Assert.That(orderClass.AccessTypes.Count, Is.EqualTo(1));
      }
    }

    [Test]
    public void AccessTypes_IsNotCached ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        testHelper.AttachJournalizeAccessType(orderClass);

        var firstCollection = orderClass.AccessTypes;
        var secondCollection = orderClass.AccessTypes;

        Assert.That(secondCollection, Is.Not.SameAs(firstCollection));
      }
    }

    [Test]
    public void AccessTypes_IsReadOnly ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        testHelper.AttachJournalizeAccessType(orderClass);

        Assert.That(((ICollection<AccessTypeDefinition>)orderClass.AccessTypes).IsReadOnly, Is.True);
      }
    }

    [Test]
    public void FindByName_ValidClassName ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      SecurableClassDefinition invoiceClass;
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        testHelper.CreateOrderClassDefinition();
        invoiceClass = testHelper.CreateInvoiceClassDefinition();
        testHelper.Transaction.Commit();
      }

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition foundClass = SecurableClassDefinition.FindByName("Remotion.SecurityManager.UnitTests.TestDomain.Invoice");

        MetadataObjectAssert.AreEqual(invoiceClass, testHelper.Transaction, foundClass);
      }
    }

    [Test]
    public void FindByName_InvalidClassName ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        testHelper.CreateOrderClassDefinition();
        testHelper.CreateInvoiceClassDefinition();
        testHelper.Transaction.Commit();
      }

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition foundClass = SecurableClassDefinition.FindByName("Invce");

        Assert.That(foundClass, Is.Null);
      }
    }

    [Test]
    public void FindAll_EmptyResult ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateEmptyDomain();

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        DomainObjectCollection result = SecurableClassDefinition.FindAll();

        Assert.That(result.Count, Is.EqualTo(0));
      }
    }

    [Test]
    public void FindAll_TenFound ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      SecurableClassDefinition[] expectedClassDefinitions;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        expectedClassDefinitions = dbFixtures.CreateAndCommitSecurableClassDefinitions(10, ClientTransactionScope.CurrentTransaction);
      }

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        DomainObjectCollection result = SecurableClassDefinition.FindAll();

        Assert.That(result.Count, Is.EqualTo(10));
        for (int i = 0; i < result.Count; i++)
          Assert.That(result[i].ID, Is.EqualTo(expectedClassDefinitions[i].ID), "Wrong Index.");
      }
    }

    [Test]
    public void FindAllBaseClasses_TenFound ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      SecurableClassDefinition[] expectedClassDefinitions;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        expectedClassDefinitions =
            dbFixtures.CreateAndCommitSecurableClassDefinitionsWithSubClassesEach(10, 10, ClientTransactionScope.CurrentTransaction);
      }

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        DomainObjectCollection result = SecurableClassDefinition.FindAllBaseClasses();

        Assert.That(result.Count, Is.EqualTo(10));
        for (int i = 0; i < result.Count; i++)
          Assert.That(result[i].ID, Is.EqualTo(expectedClassDefinitions[i].ID), "Wrong Index.");
      }
    }

    [Test]
    public void GetDerivedClasses_TenFound ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      SecurableClassDefinition expectedBaseClassDefinition;
      ObjectList<SecurableClassDefinition> expectedDerivedClasses;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition[] expectedBaseClassDefinitions =
            dbFixtures.CreateAndCommitSecurableClassDefinitionsWithSubClassesEach(10, 10, ClientTransactionScope.CurrentTransaction);
        expectedBaseClassDefinition = expectedBaseClassDefinitions[4];
        expectedDerivedClasses = expectedBaseClassDefinition.DerivedClasses;
      }

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition actualBaseClassDefinition = expectedBaseClassDefinition.ID.GetObject<SecurableClassDefinition>();

        Assert.That(actualBaseClassDefinition.DerivedClasses.Count, Is.EqualTo(10));
        for (int i = 0; i < actualBaseClassDefinition.DerivedClasses.Count; i++)
          Assert.That(actualBaseClassDefinition.DerivedClasses[i].ID, Is.EqualTo(expectedDerivedClasses[i].ID), "Wrong Index.");
      }
    }

    [Test]
    public void CreateStatelessAccessControlList ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          classDefinition.EnsureDataAvailable();
          Assert.That(classDefinition.State.IsUnchanged, Is.True);

          StatelessAccessControlList accessControlList = classDefinition.CreateStatelessAccessControlList();

          Assert.That(accessControlList.Class, Is.SameAs(classDefinition));
          Assert.IsNotEmpty(accessControlList.AccessControlEntries);
          Assert.That(classDefinition.State.IsChanged, Is.True);
        }
      }
    }

    [Test]
    public void CreateStatelessAccessControlList_Twice ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          classDefinition.EnsureDataAvailable();
          Assert.That(classDefinition.State.IsUnchanged, Is.True);

          classDefinition.CreateStatelessAccessControlList();
          Assert.That(
              () => classDefinition.CreateStatelessAccessControlList(),
              Throws.InvalidOperationException
                  .With.Message.EqualTo("A SecurableClassDefinition only supports a single StatelessAccessControlList at a time."));
        }
      }
    }

    [Test]
    public void CreateStatefulAccessControlList ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          classDefinition.EnsureDataAvailable();
          Assert.That(classDefinition.State.IsUnchanged, Is.True);

          StatefulAccessControlList accessControlList = classDefinition.CreateStatefulAccessControlList();

          Assert.That(accessControlList.Class, Is.SameAs(classDefinition));
          Assert.IsNotEmpty(accessControlList.AccessControlEntries);
          Assert.IsNotEmpty(accessControlList.StateCombinations);
          Assert.That(classDefinition.State.IsChanged, Is.True);
        }
      }
    }

    [Test]
    public void CreateStatefulAccessControlList_TwoNewAcls ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          classDefinition.EnsureDataAvailable();
          Assert.That(classDefinition.State.IsUnchanged, Is.True);

          StatefulAccessControlList acccessControlList0 = classDefinition.CreateStatefulAccessControlList();
          StatefulAccessControlList acccessControlListl = classDefinition.CreateStatefulAccessControlList();

          Assert.That(classDefinition.StatefulAccessControlLists.Count, Is.EqualTo(2));
          Assert.That(classDefinition.StatefulAccessControlLists[0], Is.SameAs(acccessControlList0));
          Assert.That(acccessControlList0.Index, Is.EqualTo(0));
          Assert.That(classDefinition.StatefulAccessControlLists[1], Is.SameAs(acccessControlListl));
          Assert.That(acccessControlListl.Index, Is.EqualTo(1));
          Assert.That(classDefinition.State.IsChanged, Is.True);
        }
      }
    }

    [Test]
    public void CreateStatefulAccessControlList_DeleteAcl ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          classDefinition.EnsureDataAvailable();
          Assert.That(classDefinition.State.IsUnchanged, Is.True);

          var acccessControlList0 = classDefinition.CreateStatefulAccessControlList();
          var acccessControlListl = classDefinition.CreateStatefulAccessControlList();
          var acccessControlList2 = classDefinition.CreateStatefulAccessControlList();
          var acccessControlList3 = classDefinition.CreateStatefulAccessControlList();

          Assert.That(classDefinition.StatefulAccessControlLists, Is.EqualTo(new[] { acccessControlList0, acccessControlListl, acccessControlList2, acccessControlList3 }));
          Assert.That(acccessControlList0.Index, Is.EqualTo(0));
          Assert.That(acccessControlListl.Index, Is.EqualTo(1));
          Assert.That(acccessControlList2.Index, Is.EqualTo(2));
          Assert.That(acccessControlList3.Index, Is.EqualTo(3));

          acccessControlListl.Delete();

          Assert.That(classDefinition.StatefulAccessControlLists, Is.EqualTo(new[] { acccessControlList0, acccessControlList2, acccessControlList3 }));
          Assert.That(acccessControlList0.Index, Is.EqualTo(0));
          Assert.That(acccessControlList2.Index, Is.EqualTo(1));
          Assert.That(acccessControlList3.Index, Is.EqualTo(2));
        }
      }
    }

    [Test]
    public void Get_AccessTypesFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      SecurableClassDefinition expectedClassDefinition;
      IList<AccessTypeDefinition> expectedAccessTypes;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        expectedClassDefinition = dbFixtures.CreateAndCommitSecurableClassDefinitionWithAccessTypes(10, ClientTransactionScope.CurrentTransaction);
        expectedAccessTypes = expectedClassDefinition.AccessTypes;
      }

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition actualClassDefinition = expectedClassDefinition.ID.GetObject<SecurableClassDefinition>();

        Assert.That(actualClassDefinition.AccessTypes.Count, Is.EqualTo(10));
        for (int i = 0; i < 10; i++)
          Assert.That(actualClassDefinition.AccessTypes[i].ID, Is.EqualTo(expectedAccessTypes[i].ID));
      }
    }

    [Test]
    public void Get_AccessControlListsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();

      SecurableClassDefinition expectedClassDefinition;
      ObjectList<StatefulAccessControlList> expectedAcls;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        expectedClassDefinition =
            dbFixtures.CreateAndCommitSecurableClassDefinitionWithAccessControlLists(10, ClientTransactionScope.CurrentTransaction);
        expectedAcls = expectedClassDefinition.StatefulAccessControlLists;
      }

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition actualClassDefinition = expectedClassDefinition.ID.GetObject<SecurableClassDefinition>();

        Assert.That(actualClassDefinition.StatefulAccessControlLists.Count, Is.EqualTo(9));
        for (int i = 0; i < 9; i++)
          Assert.That(actualClassDefinition.StatefulAccessControlLists[i].ID, Is.EqualTo(expectedAcls[i].ID));
      }
    }

    [Test]
    public void RegisterForCommit_AfterCreation ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();

        Assert.That(classDefinition.State.IsNew, Is.True);

        Assert.That(() => classDefinition.RegisterForCommit(), Throws.Nothing);
        Assert.That(classDefinition.State.IsNew, Is.True);
      }
    }

    [Test]
    public void RegisterForCommit_InNotLoadedState ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(classDefinition.State.IsNotLoadedYet, Is.True);

          Assert.That(() => classDefinition.RegisterForCommit(), Throws.Nothing);
          Assert.That(classDefinition.State.IsChanged, Is.True);
        }
      }
    }

    [Test]
    public void RegisterForCommit_AfterDelete ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(classDefinition.State.IsNotLoadedYet, Is.True);

          classDefinition.Delete();
          Assert.That(classDefinition.State.IsDeleted, Is.True);

          Assert.That(() => classDefinition.RegisterForCommit(), Throws.Nothing);
          Assert.That(classDefinition.State.IsDeleted, Is.True);
        }
      }
    }

    [Test]
    public void RegisterForCommit_AfterDiscard ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        SecurableClassDefinition classDefinition = SecurableClassDefinition.NewObject();

        classDefinition.Delete();

        Assert.That(classDefinition.State.IsInvalid, Is.True);

        Assert.That(() => classDefinition.RegisterForCommit(), Throws.TypeOf<ObjectInvalidException>());
      }
    }

    [Test]
    public void Validate_Valid ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        testHelper.CreateOrderStateAndPaymentStateCombinations(orderClass);

        SecurableClassValidationResult result = orderClass.Validate();

        Assert.That(result.IsValid, Is.True);
      }
    }

    [Test]
    public void Validate_DoubleStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations(orderClass);
        var states = stateCombinations[0].GetStates();
        StatePropertyDefinition orderStateProperty = states[0].StateProperty;
        StatePropertyDefinition paymentProperty = states[1].StateProperty;
        testHelper.CreateStateCombination(
            orderClass, orderStateProperty[EnumWrapper.Get(OrderState.Received).Name], paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);

        SecurableClassValidationResult result = orderClass.Validate();

        Assert.That(result.IsValid, Is.False);
      }
    }

    [Test]
    public void ValidateUniqueStateCombinations_NoStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();

        SecurableClassValidationResult result = new SecurableClassValidationResult();
        orderClass.ValidateUniqueStateCombinations(result);

        Assert.That(result.IsValid, Is.True);
      }
    }

    [Test]
    public void ValidateUniqueStateCombinations_TwoEmptyStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination statelessCombination1 = testHelper.CreateStateCombination(orderClass);
        StateCombination statelessCombination2 = testHelper.CreateStateCombination(orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();
        orderClass.ValidateUniqueStateCombinations(result);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.DuplicateStateCombinations, Has.Member(statelessCombination1));
        Assert.That(result.DuplicateStateCombinations, Has.Member(statelessCombination2));
      }
    }

    [Test]
    public void ValidateUniqueStateCombinations_TwoStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty(orderClass);
        StateCombination paidCombination1 = testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);
        StateCombination paidCombination2 = testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);
        testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.None).Name]);

        SecurableClassValidationResult result = new SecurableClassValidationResult();
        orderClass.ValidateUniqueStateCombinations(result);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.DuplicateStateCombinations.Count, Is.EqualTo(2));
        Assert.That(result.DuplicateStateCombinations, Has.Member(paidCombination1));
        Assert.That(result.DuplicateStateCombinations, Has.Member(paidCombination2));
      }
    }

    [Test]
    public void Commit_TwoStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty(orderClass);
        testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);
        testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);
        testHelper.CreateStateCombination(orderClass, paymentProperty[EnumWrapper.Get(PaymentState.None).Name]);

        Assert.That(
            () => testHelper.Transaction.Commit(),
            Throws.InstanceOf<ConstraintViolationException>()
                .With.Message.EqualTo(
                    "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
                    + "that has been defined twice."));
      }
    }

    [Test]
    public void ValidateUniqueStateCombinations_DoubleStateCombinationAndObjectIsDeleted ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations(orderClass);
        var states = stateCombinations[0].GetStates();
        StatePropertyDefinition orderStateProperty = states[0].StateProperty;
        StatePropertyDefinition paymentProperty = states[1].StateProperty;

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          testHelper.CreateStateCombination(
              orderClass,
              ClientTransaction.Current,
              orderStateProperty[EnumWrapper.Get(OrderState.Received).Name],
              paymentProperty[EnumWrapper.Get(PaymentState.Paid).Name]);
          Assert.That(orderClass.StateCombinations, Is.Not.Empty);
          orderClass.Delete();

          SecurableClassValidationResult result = new SecurableClassValidationResult();
          orderClass.ValidateUniqueStateCombinations(result);

          Assert.That(result.IsValid, Is.True);
          Assert.That(orderClass.State.IsDeleted, Is.True);
        }
      }
    }

    [Test]
    public void ValidateStateCombinationsAgainstStateProperties_EmptyStateCombinationInClassWithStateProperties ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        testHelper.CreateOrderStateAndPaymentStateCombinations(orderClass);
        StateCombination statelessCombination = testHelper.CreateStateCombination(orderClass);

        SecurableClassValidationResult result = new SecurableClassValidationResult();
        orderClass.ValidateStateCombinationsAgainstStateProperties(result);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.InvalidStateCombinations, Is.EquivalentTo(new[] { statelessCombination }));
      }
    }

    [Test]
    public void Commit_EmptyStateCombinationInClassWithStateProperties ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        testHelper.CreateOrderStateAndPaymentStateCombinations(orderClass);
        testHelper.CreateStateCombination(orderClass);

        Assert.That(
            () => testHelper.Transaction.Commit(),
            Throws.InstanceOf<ConstraintViolationException>()
                .With.Message.EqualTo(
                    "The securable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination "
                    + "that does not match the class's properties."));
      }
    }

    [Test]
    public void GetStatePropertyTest_ValidName ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations(orderClass);
        var states = stateCombinations[0].GetStates();
        StatePropertyDefinition orderStateProperty = states[0].StateProperty;
        StatePropertyDefinition paymentProperty = states[1].StateProperty;

        Assert.That(orderClass.GetStateProperty(orderStateProperty.Name), Is.EqualTo(orderStateProperty));
        Assert.That(orderClass.GetStateProperty(paymentProperty.Name), Is.EqualTo(paymentProperty));
      }
    }

    [Test]
    public void GetStatePropertyTest_InvalidName ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();

        Assert.That(
            () => orderClass.GetStateProperty("Invalid"),
            Throws.ArgumentException
                .With.ArgumentExceptionMessageEqualTo(
                    "A state property with the name 'Invalid' is not defined for the secureable class definition 'Remotion.SecurityManager.UnitTests.TestDomain.Order'.",
                    "propertyName"));
      }
    }

    [Test]
    public void GetStateCombinations_TwoCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper();
      using (testHelper.Transaction.EnterNonDiscardingScope())
      {
        SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition();
        StateCombination stateCombination = testHelper.CreateStateCombination(orderClass);
        List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations(orderClass);

        Assert.That(orderClass.StateCombinations, Is.EqualTo(ArrayUtility.Combine(stateCombination, stateCombinations.ToArray())));
      }
    }
  }
}
