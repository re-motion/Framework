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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Relations
{
  [TestFixture]
  public class BigRelationsTest : ClientTransactionBaseTest
  {
    [Test]
    public void Relation_WithMoreThan2100Objects ()
    {
      SetDatabaseModifyable();

      var insertedIDs = Enumerable.Range (0, 4000).Select (x => Guid.NewGuid()).ToArray();
      var insertStatements = insertedIDs.Select (
          id => string.Format (
              "insert into [OrderItem] (ID, ClassID, OrderID, [Position], [Product]) values ('{0}', 'OrderItem', '{1}', 1, 'Test2100')",
              id,
              DomainObjectIDs.Order1.Value));

      var script = string.Join (Environment.NewLine, insertStatements);
      DatabaseAgent.ExecuteCommand (script);

      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      var orderItems = order.OrderItems;

      Assert.That (orderItems.Count, Is.EqualTo (4002));

      var loadedIDs = orderItems.Select (oi => (Guid) oi.ID.Value);
      var expectedIDs = insertedIDs.Concat (new[] { (Guid) DomainObjectIDs.OrderItem1.Value, (Guid) DomainObjectIDs.OrderItem2.Value });
      Assert.That (loadedIDs.SetEquals (expectedIDs), Is.True);
    }

    [Test]
    public void Relation_WithMoreThan2100Objects_WithTableInheritance ()
    {
      SetDatabaseModifyable();

      var domainObjectIDs = new TableInheritanceDomainObjectIDs (Configuration);

      var insertedIDs = Enumerable.Range (0, 4000).Select (x => Guid.NewGuid ()).ToArray ();
      var insertStatements = insertedIDs.Select (
          id => string.Format (
              "insert into [TableInheritance_File] (ID, ClassID, [Name], [ParentFolderID], [ParentFolderIDClassID], [Size], [FileCreatedAt]) "
              + "values ('{0}', 'TI_File', 'Test', '{1}', 'TI_Folder', 42, '2006/02/03')",
              id,
              domainObjectIDs.Folder1.Value));

      var script = string.Join (Environment.NewLine, insertStatements);
      DatabaseAgent.ExecuteCommand (script);

      var folder = domainObjectIDs.Folder1.GetObject<TIFolder> ();
      var fileSystemItems = folder.FileSystemItems;

      Assert.That (fileSystemItems.Count, Is.EqualTo (4001));
      var loadedIDs = fileSystemItems.Select (oi => (Guid) oi.ID.Value);
      var expectedIDs = insertedIDs.Concat (new[] { (Guid) domainObjectIDs.File1.Value });
      Assert.That (loadedIDs.SetEquals (expectedIDs), Is.True);
    }
  }
}