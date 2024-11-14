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
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class DomainObjectDeleteHandlerTest : DomainTest
  {
    [Test]
    public void InitializeWithOneObjectList_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      testHelper.Transaction.EnterDiscardingScope();
      var list = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant("name", "id"), testHelper.CreateTenant("name", "id") };
      testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler(list);

      deleteHandler.Delete();

      Assert.That(deleteHandler.IsDeleted);
      Assert.That(list[0].State.IsDeleted, Is.True);
      Assert.That(list[1].State.IsDeleted, Is.True);
    }

    [Test]
    public void InitializeWithTwoObjectLists_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      testHelper.Transaction.EnterDiscardingScope();
      var list1 = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant("name", "id"), testHelper.CreateTenant("name", "id") };
      var list2 = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant("name", "id"), testHelper.CreateTenant("name", "id") };
      testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler(list1, list2);

      deleteHandler.Delete();

      Assert.That(deleteHandler.IsDeleted);
      Assert.That(list1[0].State.IsDeleted, Is.True);
      Assert.That(list1[1].State.IsDeleted, Is.True);
      Assert.That(list2[0].State.IsDeleted, Is.True);
      Assert.That(list2[1].State.IsDeleted, Is.True);
    }

    [Test]
    public void InitializeWithEmptyObjectList_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      testHelper.Transaction.EnterDiscardingScope();
      var list = new ObjectList<BaseSecurityManagerObject>();
      testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler(list);

      deleteHandler.Delete();

      Assert.That(deleteHandler.IsDeleted);
    }

    [Test]
    public void InitializeWithoutObjectLists_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      testHelper.Transaction.EnterDiscardingScope();
      testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler();

      deleteHandler.Delete();

      Assert.That(deleteHandler.IsDeleted);
    }

    [Test]
    public void InitializeWithDuplicateObject_AndDelete ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      testHelper.Transaction.EnterDiscardingScope();
      var list = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant("name", "id") };
      testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler(list, list);

      deleteHandler.Delete();

      Assert.That(deleteHandler.IsDeleted);
      Assert.That(list[0].State.IsDeleted, Is.True);
    }

    [Test]
    public void InitializeWithDuplicateObject_AndDeleteWhileObjectIsNew ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      testHelper.Transaction.EnterDiscardingScope();
      var list = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant("name", "id") };

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler(list, list);

      deleteHandler.Delete();

      Assert.That(deleteHandler.IsDeleted);
      Assert.That(list[0].State.IsInvalid, Is.True);
    }

    [Test]
    public void Delete_Twice ()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper();
      testHelper.Transaction.EnterDiscardingScope();
      var list = new ObjectList<BaseSecurityManagerObject> { testHelper.CreateTenant("name", "id")};
      testHelper.Transaction.CreateSubTransaction().EnterDiscardingScope();

      DomainObjectDeleteHandler deleteHandler = new DomainObjectDeleteHandler(list);

      deleteHandler.Delete();
      Assert.That(deleteHandler.IsDeleted);
      Assert.That(
          () => deleteHandler.Delete(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The Delete operation my only be performed once."));
    }

  }
}
