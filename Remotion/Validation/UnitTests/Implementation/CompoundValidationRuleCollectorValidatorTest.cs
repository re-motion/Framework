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
using Remotion.Validation.Implementation;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundValidationRuleCollectorValidatorTest
  {
    private Mock<IValidationRuleCollectorValidator> _collectorValidatorMock1;
    private Mock<IValidationRuleCollectorValidator> _collectorValidatorMock2;
    private CompoundValidationRuleCollectorValidator _compoundValidationRuleCollectorValidator;
    private Mock<IValidationRuleCollector> _collector;

    [SetUp]
    public void SetUp ()
    {
      _collectorValidatorMock1 = new Mock<IValidationRuleCollectorValidator> (MockBehavior.Strict);
      _collectorValidatorMock2 = new Mock<IValidationRuleCollectorValidator> (MockBehavior.Strict);

      _collector = new Mock<IValidationRuleCollector>();

      _compoundValidationRuleCollectorValidator = new CompoundValidationRuleCollectorValidator (new[] { _collectorValidatorMock1.Object, _collectorValidatorMock2.Object });
    }

    [Test]
    public void CheckValid ()
    {
      _collectorValidatorMock1.Setup (stub => stub.CheckValid (_collector.Object)).Verifiable();
      _collectorValidatorMock2.Setup (stub => stub.CheckValid (_collector.Object)).Verifiable();

      _compoundValidationRuleCollectorValidator.CheckValid (_collector.Object);

      _collectorValidatorMock1.Verify();
      _collectorValidatorMock2.Verify();
    }
    
  }
}