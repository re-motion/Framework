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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private DatabaseFixtures _dbFixtures;

    [SetUp]
    public void SetUp ()
    {
      try
      {
        BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
        
        _dbFixtures = new DatabaseFixtures ();
        _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
      }
      catch (Exception e)
      {
        Console.WriteLine (e);
        throw;
      }
    }

    [TearDown]
    public void TearDown ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }
  }
}
