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
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class GetPropertyString : TestBase
  {
    private IBusinessObject _businessObject;
    private Mock<IBusinessObjectStringFormatterService> _mockStringFormatterService;
    private IBusinessObjectProperty _property;

    public override void SetUp ()
    {
      base.SetUp();

      _mockStringFormatterService = new Mock<IBusinessObjectStringFormatterService>(MockBehavior.Strict);
      BindableObjectProvider provider = new BindableObjectProvider();
      provider.AddService(typeof(IBusinessObjectStringFormatterService), _mockStringFormatterService.Object);
      BusinessObjectProvider.SetProvider(typeof(BindableObjectProviderAttribute), provider);

      _businessObject = (IBusinessObject)ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty);

      _property = _businessObject.BusinessObjectClass.GetPropertyDefinition("String");
      Assert.That(
          _property,
          Is.Not.Null,
          $"Property 'String' was not found on BusinessObjectClass '{_businessObject.BusinessObjectClass.Identifier}'");

      BusinessObjectProvider.SetProvider(typeof(BindableObjectProviderAttribute), new BindableObjectProvider());
    }

    [Test]
    public void FromProperty ()
    {
      _mockStringFormatterService.Setup(_ => _.GetPropertyString(_businessObject, _property, "TheFormatString")).Returns("TheStringValue").Verifiable();

      string actual = _businessObject.GetPropertyString(_property, "TheFormatString");

      _mockStringFormatterService.Verify();
      Assert.That(actual, Is.EqualTo("TheStringValue"));
    }

    [Test]
    public void FromIdentifier ()
    {
      _mockStringFormatterService.Setup(_ => _.GetPropertyString(_businessObject, _property, null)).Returns("TheStringValue").Verifiable();

      string actual = _businessObject.GetPropertyString("String");

      _mockStringFormatterService.Verify();
      Assert.That(actual, Is.EqualTo("TheStringValue"));
    }
  }
}
