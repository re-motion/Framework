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
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectDefaultValueStrategyTest : TestBase
  {
    [Test]
    public void IsDefaultValue ()
    {
      var businessObjectStub = new Mock<IBusinessObject>();
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof (bool));
      propertyInformationStub.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);
      var property = CreateProperty(propertyInformationStub.Object);
      var strategy = (IDefaultValueStrategy) new BindableObjectDefaultValueStrategy();

      Assert.That(strategy.IsDefaultValue(businessObjectStub.Object, property), Is.False);
    }

    private BooleanProperty CreateProperty (IPropertyInformation propertyInformation)
    {
      var businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      return new BooleanProperty(GetPropertyParameters(propertyInformation, businessObjectProvider));
    }
  }
}