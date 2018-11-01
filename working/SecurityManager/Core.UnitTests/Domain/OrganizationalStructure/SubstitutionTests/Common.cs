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
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.SubstitutionTests
{
  [TestFixture]
  public class Common : SubstitutionTestBase
  {
    [Test]
    public void Initialize ()
    {
      Substitution substitution = Substitution.NewObject();

      Assert.That (substitution.IsEnabled, Is.True);
      Assert.That (substitution.BeginDate, Is.Null);
      Assert.That (substitution.EndDate, Is.Null);
    }

    [Test]
    public void DoesNotImplementISecurableObject ()
    {
      Substitution substitution = Substitution.NewObject();
      Assert.That (substitution, Is.Not.InstanceOf<ISecurableObject>());
    }
  }
}
