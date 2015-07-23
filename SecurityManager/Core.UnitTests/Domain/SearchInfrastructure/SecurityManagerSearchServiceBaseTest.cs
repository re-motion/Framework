// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure
{
  [TestFixture]
  public class SecurityManagerSearchServiceBaseTest : SearchServiceTestBase
  {
    private IBusinessObjectReferenceProperty _property;

    public override void SetUp ()
    {
      base.SetUp();

      _property = MockRepository.GenerateStub<IBusinessObjectReferenceProperty>();
    }

    [Test]
    public void Search_WithResultSizeConstraint ()
    {
      var searchService = new TestableSecurityManagerSearchServiceBase (QueryFactory.CreateLinqQuery<User>());
      var actual = searchService.Search (null, _property, CreateSecurityManagerSearchArguments (3));

      Assert.That (actual.Length, Is.EqualTo (3));
    }

    [Test]
    public void Search_WithResultSizeConstrant_AndWhereConstraint ()
    {
      var searchService = new TestableSecurityManagerSearchServiceBase (QueryFactory.CreateLinqQuery<User>().Where (u=>u.LastName.Contains ("user")));
      var actual = searchService.Search (null, _property, CreateSecurityManagerSearchArguments (1)).ToArray();

      Assert.That (actual.Length, Is.EqualTo (1));
      Assert.That (((User) actual[0]).LastName, Is.StringContaining ("user"));
    }

    private SecurityManagerSearchArguments CreateSecurityManagerSearchArguments (int? resultSize)
    {
      return new SecurityManagerSearchArguments (
          null,
          resultSize.HasValue ? new ResultSizeConstraint (resultSize.Value) : null,
          null);
    }
  }
}