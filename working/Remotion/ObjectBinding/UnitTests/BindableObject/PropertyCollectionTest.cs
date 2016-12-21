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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class PropertyCollectionTest : TestBase
  {
    [Test]
    public void Initialize_AndToArray ()
    {
      var properties = new PropertyBase[] { CreateProperty() };
      var propertyCollection = new PropertyCollection (properties);

      Assert.That (propertyCollection.ToArray(), Is.EqualTo (properties));
    }

    [Test]
    public void Contains_KnownProperty ()
    {
      var properties = new PropertyBase[] { CreateProperty() };
      var propertyCollection = new PropertyCollection (properties);

      Assert.That (propertyCollection.Contains ("Scalar"), Is.True);
    }

    [Test]
    public void Contains_UnkownProperty ()
    {
      var properties = new PropertyBase[] { CreateProperty() };
      var propertyCollection = new PropertyCollection (properties);

      Assert.That (propertyCollection.Contains ("Invalid"), Is.False);
    }

    [Test]
    public void GetByName ()
    {
      var property = CreateProperty();
      var propertyCollection = new PropertyCollection (new PropertyBase[] { property });

      Assert.That (propertyCollection["Scalar"], Is.SameAs (property));
    }

    private StubPropertyBase CreateProperty ()
    {
      return new StubPropertyBase (
          CreateParameters (
              new BindableObjectProvider (
                  MockRepository.GenerateStub<IMetadataFactory>(), MockRepository.GenerateStub<IBusinessObjectServiceFactory>()),
              GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"),
              typeof (SimpleReferenceType),
              typeof (SimpleReferenceType),
              null,
              false,
              false));
    }
  }
}