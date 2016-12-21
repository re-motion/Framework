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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class GetPropertyString : TestBase
  {
    private IBusinessObject _businessObject;
    private MockRepository _mockRepository;
    private IBusinessObjectStringFormatterService _mockStringFormatterService;
    private IBusinessObjectProperty _property;

    public override void SetUp ()
    {
      base.SetUp ();

      _mockRepository = new MockRepository();
      _mockStringFormatterService = _mockRepository.StrictMock<IBusinessObjectStringFormatterService>();
      BindableObjectProvider provider = new BindableObjectProvider();
      provider.AddService (typeof (IBusinessObjectStringFormatterService), _mockStringFormatterService);
      BusinessObjectProvider.SetProvider(typeof (BindableObjectProviderAttribute), provider);

      _businessObject = (IBusinessObject) ObjectFactory.Create<SimpleBusinessObjectClass> (ParamList.Empty);

      _property = _businessObject.BusinessObjectClass.GetPropertyDefinition ("String");
      Assert.That (
          _property, Is.Not.Null, "Property 'String' was not found on BusinessObjectClass '{0}'", _businessObject.BusinessObjectClass.Identifier);

      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), new BindableObjectProvider ());
    }

    [Test]
    public void FromProperty ()
    {
      Expect.Call (_mockStringFormatterService.GetPropertyString (_businessObject, _property, "TheFormatString")).Return ("TheStringValue");
      _mockRepository.ReplayAll();

      string actual = _businessObject.GetPropertyString (_property, "TheFormatString");
      
      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("TheStringValue"));
    }

    [Test]
    public void FromIdentifier ()
    {
      Expect.Call (_mockStringFormatterService.GetPropertyString (_businessObject, _property, null)).Return ("TheStringValue");
      _mockRepository.ReplayAll ();

      string actual = _businessObject.GetPropertyString ("String");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("TheStringValue"));
    }
  }
}
