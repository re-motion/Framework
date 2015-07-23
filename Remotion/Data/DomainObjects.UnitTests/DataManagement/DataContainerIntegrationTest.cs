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
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  public class DataContainerIntegrationTest : ClientTransactionBaseTest
  {
    private DataContainer _existingOrderDataContainer;

    public override void SetUp ()
    {
      base.SetUp ();

      _existingOrderDataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
    }

    [Test]
    public void GetObjectID ()
    {
      var propertyDefinition = GetPropertyDefinition (typeof (Order), "Customer");
      _existingOrderDataContainer.SetValue (propertyDefinition, DomainObjectIDs.Customer1);
      var id = (ObjectID) _existingOrderDataContainer.GetValue (propertyDefinition);
      Assert.That (id, Is.EqualTo (DomainObjectIDs.Customer1));
    }

    [Test]
    public void GetNullObjectID ()
    {
      var propertyDefinition = GetPropertyDefinition (typeof (Order), "Customer");
      var id = (ObjectID) _existingOrderDataContainer.GetValue (propertyDefinition);
      Assert.That (id, Is.Null);
    }

    [Test]
    public void GetBytes ()
    {
      DataContainer dataContainer = TestDataContainerObjectMother.CreateClassWithAllDataTypes1DataContainer ();

      var propertyDefinition1 = GetPropertyDefinition (typeof (ClassWithAllDataTypes), "BinaryProperty");
      ResourceManager.IsEqualToImage1 ((byte[]) dataContainer.GetValue (propertyDefinition1));
      var propertyDefinition2 = GetPropertyDefinition (typeof (ClassWithAllDataTypes), "NullableBinaryProperty");
      Assert.That (dataContainer.GetValue (propertyDefinition2), Is.Null);
    }

    [Test]
    public void SetBytes ()
    {
      DataContainer dataContainer = TestDataContainerObjectMother.CreateClassWithAllDataTypes1DataContainer ();

      var propertyDefinition1 = GetPropertyDefinition (typeof (ClassWithAllDataTypes), "BinaryProperty");
      dataContainer.SetValue (propertyDefinition1, new byte[0]);
      ResourceManager.IsEmptyImage ((byte[]) dataContainer.GetValue (propertyDefinition1));

      var propertyDefinition2 = GetPropertyDefinition (typeof (ClassWithAllDataTypes), "NullableBinaryProperty");
      dataContainer.SetValue (propertyDefinition2, null);
      Assert.That (dataContainer.GetValue (propertyDefinition2), Is.Null);
    }
  }
}