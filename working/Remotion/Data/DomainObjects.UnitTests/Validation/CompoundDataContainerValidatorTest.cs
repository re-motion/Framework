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
using Remotion.Data.DomainObjects.Validation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class CompoundDataContainerValidatorTest : StandardMappingTest
  {
    [Test]
    public void Initialize ()
    {
      var validators = new[]
                       {
                           MockRepository.GenerateStub<IDataContainerValidator>(),
                           MockRepository.GenerateStub<IDataContainerValidator>()
                       };
      var compoundValidator = new CompoundDataContainerValidator (validators);

      Assert.That (compoundValidator.Validators, Is.EqualTo (validators));
    }

    [Test]
    public void Validate ()
    {
      var validators = new[]
                       {
                           MockRepository.GenerateStub<IDataContainerValidator>(),
                           MockRepository.GenerateStub<IDataContainerValidator>()
                       };

      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.ClassWithAllDataTypes1);

      var compoundValidator = new CompoundDataContainerValidator (validators);

      compoundValidator.Validate (dataContainer);

      validators[0].AssertWasCalled (_ => _.Validate (dataContainer));
      validators[1].AssertWasCalled (_ => _.Validate (dataContainer));
    }
  }
}