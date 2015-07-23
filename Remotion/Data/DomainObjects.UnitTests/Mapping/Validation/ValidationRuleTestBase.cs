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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation
{
  [TestFixture]
  public class ValidationRuleTestBase
  {
    protected void AssertMappingValidationResult (MappingValidationResult validationResult, bool expectedIsValid, string expectedMessage)
    {
      Assert.That (validationResult.IsValid, Is.EqualTo (expectedIsValid));
      Assert.That (validationResult.Message, Is.EqualTo (expectedMessage));
    }

    protected void AssertMappingValidationResult (IEnumerable<MappingValidationResult> validationResult, bool expectedIsValid, string expectedMessage)
    {
      var invalidResults = validationResult.Where (r => !r.IsValid).ToArray ();
      if (expectedIsValid)
      {
        Assert.That (invalidResults, Is.Empty);
      }
      else
      {
        Assert.That (invalidResults.Length, Is.EqualTo (1));
        Assert.That (invalidResults[0].IsValid, Is.EqualTo (expectedIsValid));
        Assert.That (invalidResults[0].Message, Is.EqualTo (expectedMessage));
      }
    }

    protected StorageProviderDefinition StorageProviderDefinition
    {
      get { return new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider"); }
    }

    protected RelationDefinition CreateRelationDefinitionAndSetBackReferences (string id, IRelationEndPointDefinition endPointDefinition1, IRelationEndPointDefinition endPointDefinition2)
    {
      var relationDefinition = new RelationDefinition (id, endPointDefinition1, endPointDefinition2);
      endPointDefinition1.SetRelationDefinition (relationDefinition);
      endPointDefinition2.SetRelationDefinition (relationDefinition);
      return relationDefinition;
    }
  }
}