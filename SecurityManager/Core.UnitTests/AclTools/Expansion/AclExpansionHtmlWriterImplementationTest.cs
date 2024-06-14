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
using System.IO;
using NUnit.Framework;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionHtmlWriterImplementationTest : AclToolsTestBase
  {
    [Test]
    public void WriteTableDataForBooleanConditionTest ()
    {
      AssertAclExpansionHtmlWriterImplementationResult(x => x.WriteTableDataForBooleanCondition(true), "<td>X</td>");
      AssertAclExpansionHtmlWriterImplementationResult(x => x.WriteTableDataForBooleanCondition(false), "<td></td>");
    }


    public void AssertAclExpansionHtmlWriterImplementationResult (Action<AclExpansionHtmlWriterImplementation> testFunc, string resultingXmlExpected)
    {
      var stringWriter = new StringWriter();
      var implementation = new AclExpansionHtmlWriterImplementation(stringWriter, false, new AclExpansionHtmlWriterSettings());
      testFunc(implementation);
      implementation.HtmlTagWriter.Close();
      var result = stringWriter.ToString();
      Assert.That(result, Is.EqualTo(resultingXmlExpected));
    }

    [Test]
    public void WriteTableDataForAbstractRoleConditionNoAbstractRoleTest ()
    {
      AclExpansionAccessConditions accessConditions = new AclExpansionAccessConditions { AbstractRole = null };
      AssertAclExpansionHtmlWriterImplementationResult(x => x.WriteTableDataForAbstractRoleCondition(accessConditions), "<td></td>");
    }

    [Test]
    public void WriteTableDataForAbstractRoleConditionAbstractRoleTest ()
    {
      var abstractRole = TestHelper.CreateTestAbstractRole();
      AclExpansionAccessConditions accessConditions = new AclExpansionAccessConditions { AbstractRole = abstractRole };
      AssertAclExpansionHtmlWriterImplementationResult(x => x.WriteTableDataForAbstractRoleCondition(accessConditions), "<td>Test</td>");
    }
  }
}
