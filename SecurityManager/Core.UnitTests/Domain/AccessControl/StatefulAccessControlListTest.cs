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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class StatefulAccessControlListTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void GetClass ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      StatefulAccessControlList acl = _testHelper.CreateStatefulAcl(classDefinition);

      Assert.That(acl.Class, Is.SameAs(classDefinition));
    }

    [Test]
    public void SetAndGet_Index ()
    {
      StatefulAccessControlList acl = StatefulAccessControlList.NewObject();

      acl.Index = 1;
      Assert.That(acl.Index, Is.EqualTo(1));
    }

    [Test]
    public void CreateStateCombination ()
    {
      SecurableClassDefinition classDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      StatefulAccessControlList acl = _testHelper.CreateStatefulAcl(classDefinition);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        acl.EnsureDataAvailable();
        Assert.That(acl.State.IsUnchanged, Is.True);

        StateCombination stateCombination = acl.CreateStateCombination();

        Assert.That(stateCombination.AccessControlList, Is.SameAs(acl));
        Assert.That(stateCombination.Class, Is.EqualTo(acl.Class));
        Assert.That(stateCombination.GetStates(), Is.Empty);
        Assert.That(acl.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void CreateStateCombination_WithoutClassDefinition ()
    {
      StatefulAccessControlList acl = _testHelper.CreateStatefulAcl(SecurableClassDefinition.NewObject());
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        acl.EnsureDataAvailable();
        Assert.That(acl.State.IsUnchanged, Is.True);

        acl.CreateStateCombination();

        Assert.That(acl.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void CreateStateCombination_TwoNewEntries ()
    {
      StatefulAccessControlList acl = StatefulAccessControlList.NewObject();
      var securableClassDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      securableClassDefinition.StatefulAccessControlLists.Add(acl);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        acl.EnsureDataAvailable();
        Assert.That(acl.State.IsUnchanged, Is.True);

        StateCombination stateCombination0 = acl.CreateStateCombination();
        StateCombination stateCombination1 = acl.CreateStateCombination();

        Assert.That(acl.StateCombinations.Count, Is.EqualTo(2));
        Assert.That(acl.StateCombinations[0], Is.SameAs(stateCombination0));
        Assert.That(stateCombination0.Index, Is.EqualTo(0));
        Assert.That(acl.StateCombinations[1], Is.SameAs(stateCombination1));
        Assert.That(stateCombination1.Index, Is.EqualTo(1));
        Assert.That(acl.State.IsChanged, Is.True);
      }
    }

    [Test]
    public void CreateStateCombination_DeleteEntries ()
    {
      StatefulAccessControlList acl = StatefulAccessControlList.NewObject();
      var securableClassDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      securableClassDefinition.StatefulAccessControlLists.Add(acl);
      using (_testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope())
      {
        acl.EnsureDataAvailable();
        Assert.That(acl.State.IsUnchanged, Is.True);

        StateCombination stateCombination0 = acl.CreateStateCombination();
        StateCombination stateCombination1 = acl.CreateStateCombination();
        StateCombination stateCombination2 = acl.CreateStateCombination();
        StateCombination stateCombination3 = acl.CreateStateCombination();

        Assert.That(acl.StateCombinations, Is.EqualTo(new[] { stateCombination0, stateCombination1, stateCombination2, stateCombination3 }));
        Assert.That(stateCombination0.Index, Is.EqualTo(0));
        Assert.That(stateCombination1.Index, Is.EqualTo(1));
        Assert.That(stateCombination2.Index, Is.EqualTo(2));
        Assert.That(stateCombination3.Index, Is.EqualTo(3));

        stateCombination1.Delete();

        Assert.That(acl.StateCombinations, Is.EqualTo(new[] { stateCombination0, stateCombination2, stateCombination3 }));
        Assert.That(stateCombination0.Index, Is.EqualTo(0));
        Assert.That(stateCombination2.Index, Is.EqualTo(1));
        Assert.That(stateCombination3.Index, Is.EqualTo(2));
      }
    }

    [Test]
    public void Get_StateCombinationsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      var expectedAcl = dbFixtures.CreateAndCommitAccessControlListWithStateCombinations(10, ClientTransactionScope.CurrentTransaction);
      var expectedStateCombinations = expectedAcl.StateCombinations;

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        var actualAcl = (StatefulAccessControlList)LifetimeService.GetObject(ClientTransaction.Current, expectedAcl.ID, false);

        Assert.That(actualAcl.StateCombinations.Count, Is.EqualTo(9));
        for (int i = 0; i < 9; i++)
          Assert.That(actualAcl.StateCombinations[i].ID, Is.EqualTo(expectedStateCombinations[i].ID));
      }
    }

    [Test]
    public void CreateStateCombination_BeforeClassIsSet ()
    {
      StatefulAccessControlList acl = StatefulAccessControlList.NewObject();
      Assert.That(
          () => acl.CreateStateCombination(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot create StateCombination if no SecurableClassDefinition is assigned to this StatefulAccessControlList."));
    }

    [Test]
    public void OnCommitting_RegistersClassForCommit ()
    {
      var acl = StatefulAccessControlList.NewObject();
      var classDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      classDefinition.StatefulAccessControlLists.Add(acl);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        acl.RegisterForCommit();

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }

    [Test]
    public void OnCommitting_WithDeletedAcl_RegistersClassForCommit ()
    {
      var acl = StatefulAccessControlList.NewObject();
      var classDefinition = _testHelper.CreateClassDefinition("SecurableClass");
      classDefinition.StatefulAccessControlLists.Add(acl);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        bool commitOnClassWasCalled = false;
        classDefinition.Committing += (sender, e) =>
        {
          commitOnClassWasCalled = true;
          Assert.That(GetDataContainer((DomainObject)sender).HasBeenMarkedChanged, Is.True);
        };
        acl.Delete();

        ClientTransaction.Current.Commit();

        Assert.That(commitOnClassWasCalled, Is.True);
      }
    }
  }
}
