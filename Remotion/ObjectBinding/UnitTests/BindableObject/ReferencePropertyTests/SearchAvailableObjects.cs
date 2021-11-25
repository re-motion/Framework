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
using Moq;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SearchAvailableObjects : TestBase
  {
    private BindableObjectProvider _bindableObjectProviderForDeclaringType;
    private BindableObjectProvider _bindableObjectProviderForPropertyType;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProviderForDeclaringType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _bindableObjectProviderForPropertyType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute>(_bindableObjectProviderForDeclaringType);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute>(_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void Search_WithSearchSupported ()
    {
      var stubBusinessObject = new Mock<IBusinessObject>();
      var mockService = new Mock<ISearchServiceOnProperty>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreateProperty("SearchServiceFromPropertyDeclaration");
      IBusinessObject[] expected = new IBusinessObject[0];
      var searchArgumentsStub = new Mock<ISearchAvailableObjectsArguments>();

      var sequence = new MockSequence();
      mockService.InSequence(sequence).Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      mockService.InSequence(sequence).Setup(_ => _.Search(stubBusinessObject.Object, property, searchArgumentsStub.Object)).Returns(expected).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(mockService.Object);
      IBusinessObject[] actual = property.SearchAvailableObjects(stubBusinessObject.Object, searchArgumentsStub.Object);

      mockService.Verify();
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void Search_WithSearchSupportedAndReferencingObjectNull ()
    {
      var mockService = new Mock<ISearchServiceOnType>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreateProperty("SearchServiceFromPropertyType");
      IBusinessObject[] expected = new IBusinessObject[0];
      var searchArgumentsStub = new Mock<ISearchAvailableObjectsArguments>();

      var sequence = new MockSequence();
      mockService.InSequence(sequence).Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      mockService.InSequence(sequence).Setup(_ => _.Search(null, property, searchArgumentsStub.Object)).Returns(expected).Verifiable();

      _bindableObjectProviderForPropertyType.AddService(mockService.Object);
      IBusinessObject[] actual = property.SearchAvailableObjects(null, searchArgumentsStub.Object);

      mockService.Verify();
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void Search_WithSearchNotSupported ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithBusinessObjectProperties>(ParamList.Empty);
      var mockService = new Mock<ISearchServiceOnProperty>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreateProperty("SearchServiceFromPropertyDeclaration");
      var searchArgumentsStubb = new Mock<ISearchAvailableObjectsArguments>();

      mockService.Setup(_ => _.SupportsProperty(property)).Returns(false).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(mockService.Object);

      Assert.That(
          () => property.SearchAvailableObjects(businessObject, searchArgumentsStubb.Object),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Searching is not supported for reference property 'SearchServiceFromPropertyDeclaration' of business object class "
                  + "'Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain.ClassWithBusinessObjectProperties, "
                  + "Remotion.ObjectBinding.UnitTests'."));

      mockService.Verify();
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters =
          GetPropertyParameters(GetPropertyInfo(typeof(ClassWithBusinessObjectProperties), propertyName), _bindableObjectProviderForDeclaringType);
      ReferenceProperty property = new ReferenceProperty(propertyParameters);
      property.SetReflectedClass(BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(ClassWithBusinessObjectProperties)));

      return property;
    }
  }
}
