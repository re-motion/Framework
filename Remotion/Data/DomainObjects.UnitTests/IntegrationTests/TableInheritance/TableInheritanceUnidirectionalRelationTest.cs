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
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.TableInheritance
{
  [TestFixture]
  public class TableInheritanceUnidirectionalRelationTest : TableInheritanceMappingTest
  {
    [Test]
    [Ignore("TODO: Implement referential integrity for unidirectional relationships.")]
    public void DeleteAndCommitWithConcreteTableInheritance ()
    {
      TIClassWithUnidirectionalRelation classWithUnidirectionalRelation =
          DomainObjectIDs.ClassWithUnidirectionalRelation.GetObject<TIClassWithUnidirectionalRelation>();
      classWithUnidirectionalRelation.DomainBase.Delete();
      ClientTransactionScope.CurrentTransaction.Commit();

      try
      {
        Dev.Null = classWithUnidirectionalRelation.DomainBase;
        Assert.Fail("Expected ObjectInvalidException");
      }
      catch (ObjectInvalidException)
      {
        // succeed
      }

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        TIClassWithUnidirectionalRelation reloadedObject =
            DomainObjectIDs.ClassWithUnidirectionalRelation.GetObject<TIClassWithUnidirectionalRelation>();

        Assert.That(reloadedObject.DomainBase, Is.Null);
      }
    }
  }
}
