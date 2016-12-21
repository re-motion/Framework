﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.IntegrationTests;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class CompoundPersistableDataValidatorTest : StandardMappingTest
  {
    [Test]
    public void Initialize ()
    {
      var validators = new[]
                       {
                           MockRepository.GenerateStub<IPersistableDataValidator>(),
                           MockRepository.GenerateStub<IPersistableDataValidator>()
                       };
      var compoundValidator = new CompoundPersistableDataValidator (validators);

      Assert.That (compoundValidator.Validators, Is.EqualTo (validators));
    }

    [Test]
    public void Validate ()
    {
      var validators = new[]
                       {
                           MockRepository.GenerateStub<IPersistableDataValidator>(),
                           MockRepository.GenerateStub<IPersistableDataValidator>()
                       };

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);
        var dataContainer = DataContainer.CreateNew (domainObject.ID);
        var persistableData = new PersistableData (domainObject, StateType.Changed, dataContainer, Enumerable.Empty<IRelationEndPoint>());
        var compoundValidator = new CompoundPersistableDataValidator (validators);

        compoundValidator.Validate (ClientTransaction.Current, persistableData);

        validators[0].AssertWasCalled (_ => _.Validate (ClientTransaction.Current, persistableData));
        validators[1].AssertWasCalled (_ => _.Validate (ClientTransaction.Current, persistableData));
      }
    }
  }
}