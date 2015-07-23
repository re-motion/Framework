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
using Remotion.Validation.Mixins.Implementation;
using Remotion.Validation.Mixins.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Validation.Mixins.UnitTests.Implementation
{
  [TestFixture]
  public class CheckNoMixinCollectorValidatorTest
  {
    private CheckNoMixinCollectorValidator _validator;
    private IComponentValidationCollector _collectorStub;

    [SetUp]
    public void SetUp ()
    {
      _collectorStub = MockRepository.GenerateStub<IComponentValidationCollector>();

      _validator = new CheckNoMixinCollectorValidator();
    }

    [Test]
    public void CheckValid_MixinType_ExceptionIsThrown ()
    {
      _collectorStub.Stub (stub => stub.ValidatedType).Return (typeof (CustomerMixin));

      Assert.That (
          () => _validator.CheckValid (_collectorStub),
          Throws.TypeOf<NotSupportedException> ().And.Message.EqualTo (
              "Validation rules for type 'Remotion.Validation.Mixins.UnitTests.TestDomain.CustomerMixin' are not supported. "
              + "If validation rules should be defined for mixins, please ensure to apply the rules to 'ITargetInterface' or 'IIntroducedInterface' instead."
              ));
    }

    [Test]
    public void CheckValid_NoMixinType_NoExceptionIsThrown ()
    {
      _collectorStub.Stub (stub => stub.ValidatedType).Return (typeof (Customer));

      _validator.CheckValid (_collectorStub);
    }
  }
}