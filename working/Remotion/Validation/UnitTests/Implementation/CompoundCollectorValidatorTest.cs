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
using Remotion.Validation.Implementation;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundCollectorValidatorTest
  {
    private ICollectorValidator _collectorValidatorMock1;
    private ICollectorValidator _collectorValidatorMock2;
    private CompoundCollectorValidator _compoundCollectorValidator;
    private IComponentValidationCollector _collector;

    [SetUp]
    public void SetUp ()
    {
      _collectorValidatorMock1 = MockRepository.GenerateStrictMock<ICollectorValidator>();
      _collectorValidatorMock2 = MockRepository.GenerateStrictMock<ICollectorValidator> ();

      _collector = MockRepository.GenerateStub<IComponentValidationCollector>();

      _compoundCollectorValidator = new CompoundCollectorValidator (new[] { _collectorValidatorMock1, _collectorValidatorMock2 });
    }

    [Test]
    public void CheckValid ()
    {
      _collectorValidatorMock1.Expect (stub => stub.CheckValid (_collector));
      _collectorValidatorMock2.Expect (stub => stub.CheckValid (_collector));

      _compoundCollectorValidator.CheckValid (_collector);

      _collectorValidatorMock1.VerifyAllExpectations();
      _collectorValidatorMock2.VerifyAllExpectations();
    }
    
  }
}