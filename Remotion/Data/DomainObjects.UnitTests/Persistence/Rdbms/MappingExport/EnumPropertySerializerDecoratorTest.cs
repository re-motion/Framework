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
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.MappingExport;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.MappingExport
{
  [TestFixture]
  public class EnumPropertySerializerDecoratorTest : SchemaGenerationTestBase
  {

    [Test]
    public void Serialize_DelegatesToPropertySerializer ()
    {
      var sampleProperty = GetPropertyDefinition ((ClassWithAllDataTypes _) => _.ExtensibleEnumProperty);
      var propertySerializerMock = MockRepository.GenerateMock<IPropertySerializer>();
      var enumPropertySerializerDecorator = new EnumPropertySerializerDecorator (MockRepository.GenerateStub<IEnumSerializer>(), propertySerializerMock);

      var expected = new XElement ("expected");
      propertySerializerMock.Expect (
          _ => _.Serialize (Arg.Is (sampleProperty), Arg<IRdbmsPersistenceModelProvider>.Is.Anything))
          .Return (expected);

      propertySerializerMock.Replay();

      var actual = enumPropertySerializerDecorator.Serialize (
          sampleProperty,
          MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>());

      propertySerializerMock.VerifyAllExpectations();

      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void Serialize_PassesPropertyToEnumSerializer ()
    {
      var sampleProperty = GetPropertyDefinition ((ClassWithAllDataTypes _) => _.EnumProperty);
      var enumSerializerMock = MockRepository.GenerateMock<IEnumSerializer>();
      var enumPropertySerializerDecorator = new EnumPropertySerializerDecorator (enumSerializerMock, MockRepository.GenerateStub<IPropertySerializer>());

      enumSerializerMock.Expect (_ => _.CollectPropertyType (sampleProperty));
      enumSerializerMock.Replay();

      enumPropertySerializerDecorator.Serialize (
          sampleProperty,
          MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>());

      enumSerializerMock.VerifyAllExpectations();
    }


  }
}