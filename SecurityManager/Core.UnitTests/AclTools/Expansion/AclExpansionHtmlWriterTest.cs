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
using System.IO;
using System.Linq;
using System.Xml;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Globalization.AclTools.Expansion;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionHtmlWriterTest : AclToolsTestBase
  {
    [Test]
    public void FullNameTest ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        var users = ListObjectMother.New(User);
        var acls = ListObjectMother.New<AccessControlList>(Acl);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList(users, acls, false);

        var stringWriter = new StringWriter();
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(stringWriter, true,
          new AclExpansionHtmlWriterSettings { ShortenNames = false });
        aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
        string result = stringWriter.ToString();
        Assert.That(result, Does.Contain("Dhl|Remotion.SecurityManager.UnitTests.TestDomain.Delivery, Remotion.SecurityManager.UnitTests"));
        Assert.That(result, Does.Contain("Remotion.SecurityManager.UnitTests.TestDomain.Order"));
      }
    }

    [Test]
    public void ShortNameTest ()
    {
      using (CultureScope.CreateInvariantCultureScope())
      {
        var users = ListObjectMother.New(User);
        var acls = ListObjectMother.New<AccessControlList>(Acl);
        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList(users, acls, false);

        var stringWriter = new StringWriter();
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(stringWriter, true,
          new AclExpansionHtmlWriterSettings { ShortenNames = true });
        aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
        string result = stringWriter.ToString();
        Assert.That(result, Does.Contain("Dhl"));
        Assert.That(
            result,
            Does.Not.Contains(
                "Remotion.SecurityManager.UnitTests.TestDomain.Delivery, Remotion.SecurityManager.UnitTests"));
        Assert.That(result, Does.Contain("Order"));
        Assert.That(result, Does.Not.Contains("Remotion.SecurityManager.UnitTests.TestDomain.Order"));
      }
    }


    [Test]
    public void OutputRowCountTest ()
    {
      var users = ListObjectMother.New(User);
      var acls = ListObjectMother.New<AccessControlList>(Acl);
      List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList(users, acls, false);

      var stringWriter = new StringWriter();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(stringWriter, true,
        new AclExpansionHtmlWriterSettings { OutputRowCount = true });
      aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      string result = stringWriter.ToString();
      Assert.That(result, Does.Contain("Usa Da, Dr. (2)"));
      Assert.That(result, Does.Contain("Da Group, Supreme Being (2)"));
      Assert.That(result, Does.Contain("Order (2)"));
    }

    [Test]
    public void DontOutputRowCountTest ()
    {
      var users = ListObjectMother.New(User);
      var acls = ListObjectMother.New<AccessControlList>(Acl);

      List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList(users, acls, false);

      var stringWriter = new StringWriter();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(stringWriter, true,
        new AclExpansionHtmlWriterSettings { OutputRowCount = false });
      aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      string result = stringWriter.ToString();
      Assert.That(result, Does.Not.Contains("(2)"));
    }


    [Test]
    public void UserSortOrderTest ()
    {
      var users = ListObjectMother.New(User, User2, User3);
      var acls = ListObjectMother.New<AccessControlList>(Acl, Acl2);

      List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList(users, acls, false);

      var stringWriter = new StringWriter();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(stringWriter, true,
        new AclExpansionHtmlWriterSettings { ShortenNames = true });
      aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      string result = stringWriter.ToString();

      Assert.That(result, Does.Contain("James Ryan"));
      Assert.That(result.IndexOf("James Ryan"), Is.LessThan(result.IndexOf("Smith, Mr.")));
    }

    [Test]
    public void SortOrderTest ()
    {
      var users = ListObjectMother.New(User3);
      var acls = ListObjectMother.New<AccessControlList>(Acl);

      List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList(users, acls, false);

      var stringWriter = new StringWriter();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(stringWriter, true,
          new AclExpansionHtmlWriterSettings { ShortenNames = true });
      aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      string result = stringWriter.ToString();

      // Roles
      const string firstRoleText = "Anotha Group, Supreme Being";
      const string secondRoleText = "Anotha Group, Working Drone";
      const string thirdRoleText = "Da 3rd Group, Combatant";
      Assert.That(result, Does.Contain(firstRoleText));
      Assert.That(result.IndexOf(firstRoleText), Is.LessThan(result.IndexOf(secondRoleText)));
      Assert.That(result.IndexOf(secondRoleText), Is.LessThan(result.IndexOf(thirdRoleText)));

      Assert.That(result, Does.Contain("Dhl, None, Received"));
      Assert.That(result, Does.Contain("Delete, FirstAccessType, Read, Write"));
    }


    [Test]
    public void ResultTest ()
    {
      using (CultureScope_de_DE())
      {
        var users = ListObjectMother.New(User, User2, User3);
        var acls = ListObjectMother.New<AccessControlList>(Acl2, Acl);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList(users, acls, false);

        using (var textWriter = new StringWriter())
        {
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(textWriter, true, new AclExpansionHtmlWriterSettings());
          aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
          string result = textWriter.ToString();

          //Clipboard.SetText (CreateLiteralResultExpectedString(result));

          const string resultExpected =
          #region
 @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"" """">
<html>
  <head>
    <title>re-motion ACL Expansion</title>
    <style>@import ""AclExpansion.css"";</style>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
  </head>
  <body>
    <table style=""width: 100%;"" class=""aclExpansionTable"" id=""remotion-ACL-expansion-table"">
      <tr>
        <th class=""header"">User</th>
        <th class=""header"">Role</th>
        <th class=""header"">Class</th>
        <th class=""header"">Status</th>
        <th class=""header"">User must be owner</th>
        <th class=""header"">Object group is set to</th>
        <th class=""header"">Object tenant is set to</th>
        <th class=""header"">User must have abstract role</th>
        <th class=""header"">Permissions granted</th>
      </tr>
      <tr>
        <td rowspan=""15"">James Ryan</td>
        <td rowspan=""2"">Anotha Group, Supreme Being</td>
        <td rowspan=""2"">Bestellung</td>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""2"">Anotha Group, Working Drone</td>
        <td rowspan=""2"">Bestellung</td>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Da 3rd Group, Combatant</td>
        <td rowspan=""3"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Da 3rd Group, Combatant</td>
        <td rowspan=""3"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""3"">Da Group, Combatant</td>
        <td rowspan=""3"">Bestellung</td>
        <td>Bezahlt, DHL, Erhalten</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Read, Write</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""2"">Da Group, Supreme Being</td>
        <td rowspan=""2"">Bestellung</td>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""8"">Smith, Mr.</td>
        <td rowspan=""2"">Anotha Group, Supreme Being</td>
        <td rowspan=""2"">Bestellung</td>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""2"">Anotha Group, Working Drone</td>
        <td rowspan=""2"">Bestellung</td>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""2"">Da 3rd Group, Working Drone</td>
        <td rowspan=""2"">Bestellung</td>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""2"">Da Group, Working Drone</td>
        <td rowspan=""2"">Bestellung</td>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
      <tr>
        <td rowspan=""2"">Usa Da, Dr.</td>
        <td rowspan=""2"">Da Group, Supreme Being</td>
        <td rowspan=""2"">Bestellung</td>
        <td>DHL, Erhalten, Offen; DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete, FirstAccessType, Read</td>
      </tr>
      <tr>
        <td>DHL, Erhalten, Offen</td>
        <td></td>
        <td></td>
        <td>Da Tenant</td>
        <td></td>
        <td>Delete, FirstAccessType, Read, Write</td>
      </tr>
    </table>
  </body>
</html>";
          #endregion

          Assert.That(result, Is.EqualTo(resultExpected));
        }
      }
    }


    [Test]
    public void StatelessAclTest ()
    {
      using (CultureScope_de_DE())
      {
        var users = ListObjectMother.New(User);

        // Create stateless-only ACL
        SecurableClassDefinition classDefinition = TestHelper.CreateOrderClassDefinition();
        var statlessAcl = TestHelper.CreateStatelessAcl(classDefinition);
        TestHelper.AttachAces(statlessAcl, Ace);
        var acls = ListObjectMother.New<AccessControlList>(statlessAcl);
        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList(users, acls, false);

        using (var textWriter = new StringWriter())
        {
          const string stateLessAclStateHtmlText = "@*?stateless state?!?!?";
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(textWriter, true, new AclExpansionHtmlWriterSettings());
          aclExpansionHtmlWriter.Implementation.StatelessAclStateHtmlText = stateLessAclStateHtmlText;
          aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
          string result = textWriter.ToString();
          Assert.That(result, Does.Contain("<td>"+ stateLessAclStateHtmlText + @"</td>"));
        }
      }
    }


    [Test]
    public void AclWithNoAssociatedStatesHtmlTest ()
    {
      using (CultureScope_de_DE())
      {
        var users = ListObjectMother.New(User);

        // Create an ACL with no states
        var orderClassDefinition = TestHelper.CreateOrderClassDefinition();
        var acl = TestHelper.CreateStatefulAcl(orderClassDefinition, new StateDefinition[0]);

        TestHelper.AttachAces(acl, Ace);

        // Assert that the ACL ctually contains no states (note: this is not the same as an ACL being a stateless ACL).
        var stateDefinitions = acl.StateCombinations.SelectMany(x => x.GetStates());
        Assert.That(stateDefinitions.Any(), Is.False);

        var acls = ListObjectMother.New<AccessControlList>(acl);

        List<AclExpansionEntry> aclExpansion = GetAclExpansionEntryList(users, acls, false);

        using (var textWriter = new StringWriter())
        {
          const string aclWithNoAssociatedStatesHtmlText = "No sports... - I mean states";
          var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(textWriter, true, new AclExpansionHtmlWriterSettings());
          aclExpansionHtmlWriter.Implementation.AclWithNoAssociatedStatesHtmlText = aclWithNoAssociatedStatesHtmlText;
          aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
          string result = textWriter.ToString();
          Assert.That(result, Does.Contain("<td>" + aclWithNoAssociatedStatesHtmlText + @"</td>"));
        }
      }
    }



    [Test]
    public void EmptyAclExpansionListResultTest ()
    {
      var textWriter = new StringWriter();
      new AclExpansionHtmlWriter(textWriter, true, new AclExpansionHtmlWriterSettings()).WriteAclExpansion(new List<AclExpansionEntry>());
      string result = textWriter.ToString();

      const string resultExpected =
      #region
 @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"" """">
<html>
  <head>
    <title>re-motion ACL Expansion</title>
    <style>@import ""AclExpansion.css"";</style>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
  </head>
  <body>
    <table style=""width: 100%;"" class=""aclExpansionTable"" id=""remotion-ACL-expansion-table"">
      <tr>
        <th class=""header"">User</th>
        <th class=""header"">Role</th>
        <th class=""header"">Class</th>
        <th class=""header"">Status</th>
        <th class=""header"">User must be owner</th>
        <th class=""header"">Object group is set to</th>
        <th class=""header"">Object tenant is set to</th>
        <th class=""header"">User must have abstract role</th>
        <th class=""header"">Permissions granted</th>
      </tr>
    </table>
  </body>
</html>";
      #endregion
      Assert.That(result, Is.EqualTo(resultExpected));
    }



    [Test]
    public void DeniedRightsTest ()
    {
      var aclExpansionEntry = new AclExpansionEntry(
          User, Role, Acl, new AclExpansionAccessConditions(), new[] { DeleteAccessType }, new[] { ReadAccessType, WriteAccessType });
      var aclExpansionEntryList = ListObjectMother.New(aclExpansionEntry);

      using (var textWriter = new StringWriter())
      {
        var aclExpansionWriter = new AclExpansionHtmlWriter(textWriter, true,
        new AclExpansionHtmlWriterSettings { OutputDeniedRights = true });
        aclExpansionWriter.WriteAclExpansion(aclExpansionEntryList);


        string result = textWriter.ToString();
        //Clipboard.SetText (result);

        // Detail tests 
        Assert.That(result, Does.Contain("Permissions denied</th>")); // Denied rights header
        Assert.That(result, Does.Contain("<td>Delete</td>")); // allowed rights
        Assert.That(result, Does.Contain("<td>Read, Write</td>")); // denied rights

        const string resultExpected =
        #region
 @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"" """">
<html>
  <head>
    <title>re-motion ACL Expansion</title>
    <style>@import ""AclExpansion.css"";</style>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
  </head>
  <body>
    <table style=""width: 100%;"" class=""aclExpansionTable"" id=""remotion-ACL-expansion-table"">
      <tr>
        <th class=""header"">User</th>
        <th class=""header"">Role</th>
        <th class=""header"">Class</th>
        <th class=""header"">Status</th>
        <th class=""header"">User must be owner</th>
        <th class=""header"">Object group is set to</th>
        <th class=""header"">Object tenant is set to</th>
        <th class=""header"">User must have abstract role</th>
        <th class=""header"">Permissions granted</th>
        <th class=""header"">Permissions denied</th>
      </tr>
      <tr>
        <td rowspan=""1"">Usa Da, Dr.</td>
        <td rowspan=""1"">Da Group, Supreme Being</td>
        <td rowspan=""1"">Order</td>
        <td>DHL, None, Received</td>
        <td></td>
        <td></td>
        <td></td>
        <td></td>
        <td>Delete</td>
        <td>Read, Write</td>
      </tr>
    </table>
  </body>
</html>";
        #endregion

        // Full test
        Assert.That(result, Is.EqualTo(resultExpected));
      }
    }



    [Test]
    public void GroupHierarchyConditionTest ()
    {
      var group = Group;
      const string orItsChildrenText = "or its children";
      const string orItsParentsText = "or its parents";
      AssertGroupHierarchyCondition(group, GroupHierarchyCondition.This, group.DisplayName, new[] { orItsChildrenText, orItsParentsText });
      AssertGroupHierarchyCondition(group, GroupHierarchyCondition.ThisAndParent, group.DisplayName + "<br />" + orItsParentsText, new[] { orItsChildrenText });
      AssertGroupHierarchyCondition(group, GroupHierarchyCondition.ThisAndChildren, group.DisplayName + "<br />" + orItsChildrenText, new[] { orItsParentsText });
      AssertGroupHierarchyCondition(
          group,
          GroupHierarchyCondition.ThisAndParentAndChildren,
          group.DisplayName + "<br />" + orItsParentsText + "<br />" + orItsChildrenText,
          new string[0]);
    }

    public void AssertGroupHierarchyCondition (Group owningGroup, GroupHierarchyCondition groupHierarchyCondition, string inResultingHtmlString,
      string[] notInResultingHtmlStrings)
    {
      var accessConditions = new AclExpansionAccessConditions { OwningGroup = owningGroup, GroupHierarchyCondition = groupHierarchyCondition };
      var aclExpansionEntry = new AclExpansionEntry(
          User, Role, Acl, accessConditions, new AccessTypeDefinition[0], new AccessTypeDefinition[0]);
      List<AclExpansionEntry> aclExpansion = ListObjectMother.New(aclExpansionEntry);

      var stringWriter = new StringWriter();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(stringWriter, true, new AclExpansionHtmlWriterSettings());
      aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      string result = stringWriter.ToString();
      //Clipboard.SetText (result);

      Assert.That(result, Does.Contain(inResultingHtmlString));
      foreach (string notInResultingHtml in notInResultingHtmlStrings)
      {
        Assert.That(result, Does.Not.Contains(notInResultingHtml));
      }
    }



    [Test]
    public void TenantHierarchyConditionTest ()
    {
      var tenant = Tenant;
      const string orItsParentsText = "or its parents";
      AssertTenantHierarchyCondition(tenant, TenantHierarchyCondition.This, tenant.DisplayName, new[] { orItsParentsText });
      AssertTenantHierarchyCondition(tenant, TenantHierarchyCondition.ThisAndParent, tenant.DisplayName + "<br />" + orItsParentsText, new string[0]);
    }

    public void AssertTenantHierarchyCondition (Tenant owningTenant, TenantHierarchyCondition tenantHierarchyCondition, string inResultingHtmlString,
      string[] notInResultingHtmlStrings)
    {
      var accessConditions = new AclExpansionAccessConditions { OwningTenant = owningTenant, TenantHierarchyCondition = tenantHierarchyCondition };
      var aclExpansionEntry = new AclExpansionEntry( User, Role, Acl, accessConditions, new AccessTypeDefinition[0], new AccessTypeDefinition[0]);
      List<AclExpansionEntry> aclExpansion = ListObjectMother.New(aclExpansionEntry);

      var stringWriter = new StringWriter();
      var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(stringWriter, true, new AclExpansionHtmlWriterSettings());
      aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      string result = stringWriter.ToString();
      //Clipboard.SetText (result);

      Assert.That(result, Does.Contain(inResultingHtmlString));
      foreach (string notInResultingHtml in notInResultingHtmlStrings)
      {
        Assert.That(result, Does.Not.Contains(notInResultingHtml));
      }
    }



    [Test]
    public void WriteTableBody_ProcessClassClassNodeKeyIsNullTest ()
    {
      const int nrLeafNodes = 33;

      var aclExpansionTreeChildrenMock = new Mock<List<AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>();

      var aclExpansionTreeNodeMock = new Mock<AclExpansionTreeNode<SecurableClassDefinition,
        AclExpansionTreeNode<AclExpansionEntry, AclExpansionEntry>>>(null, nrLeafNodes, aclExpansionTreeChildrenMock.Object);
      aclExpansionTreeNodeMock.Setup(x => x.NumberLeafNodes).Returns(nrLeafNodes).Verifiable();
      aclExpansionTreeNodeMock.Setup(x => x.Children).Returns(aclExpansionTreeChildrenMock.Object).Verifiable();

      var aclExpansionHtmlWriterImplementation = new Mock<AclExpansionHtmlWriterImplementation>(TextWriter.Null, true, new AclExpansionHtmlWriterSettings());
      aclExpansionHtmlWriterImplementation.Setup(x => x.WriteTableDataWithRowCount(AclToolsExpansion.NO_CLASSES_DEFINED_Text, nrLeafNodes)).Verifiable();

      var aclExpansionHtmlWriterMock =
          new Mock<AclExpansionHtmlWriter>(
              aclExpansionHtmlWriterImplementation.Object,
              new AclExpansionHtmlWriterSettings())
          { CallBase = true };
      aclExpansionHtmlWriterMock.Setup(x => x.WriteTableBody_ProcessStates(aclExpansionTreeChildrenMock.Object)).Verifiable();

      aclExpansionHtmlWriterMock.Object.WriteTableBody_ProcessClass(aclExpansionTreeNodeMock.Object);

      aclExpansionTreeChildrenMock.Verify();
      aclExpansionTreeNodeMock.Verify();
      aclExpansionHtmlWriterImplementation.Verify();
      aclExpansionHtmlWriterMock.Verify();
    }


    [Test]
    [Explicit]
    public void WriteAclExpansionAsHtmlTest ()
    {
      using (new CultureScope("de-AT", "de-AT"))
      {
        var aclExpander = new AclExpander();
        var aclExpansion = aclExpander.GetAclExpansionEntryList();
        var stringWriter = new StringWriter();
        var aclExpansionHtmlWriter = new AclExpansionHtmlWriter(stringWriter, true, new AclExpansionHtmlWriterSettings());
        aclExpansionHtmlWriter.WriteAclExpansion(aclExpansion);
      }
    }


    // Note: Code is kept for comparison of readability and XML validity safety with HtmlTagWriter
    [Test]
    [Explicit]
    public void XmlWriterSpikeTest ()
    {
      var stringWriter = new StringWriter();
      var xmlWriter = CreateXmlWriter(stringWriter, false);

      //xmlWriter.WriteStartDocument();
      // DOCTYPE
      xmlWriter.WriteDocType("HTML", "-//W3C//DTD HTML 4.0 Transitional//EN", null, null);
      // HTML
      xmlWriter.WriteStartElement("html");
      // HEAD
      xmlWriter.WriteStartElement("head");
      // TITLE
      xmlWriter.WriteStartElement("title");
      xmlWriter.WriteValue("re-motion ACL Expansion");
      xmlWriter.WriteEndElement(); // title

      // STYLE
      xmlWriter.WriteStartElement("style");
      xmlWriter.WriteValue("@import \"AclExpansion.css\";");
      xmlWriter.WriteEndElement(); // style
      xmlWriter.WriteEndElement(); // head

      // BODY
      xmlWriter.WriteStartElement("body");
      xmlWriter.WriteValue("re-motion ACL Expansion body");

      // TABLE
      xmlWriter.WriteStartElement("table");
      xmlWriter.WriteAttributeString("style", "width: 100%;");
      xmlWriter.WriteAttributeString("class", "aclExpansionTable");
      xmlWriter.WriteAttributeString("id", "remotion-ACL-expansion-table");

      // TR
      xmlWriter.WriteStartElement("tr");
      // TD
      xmlWriter.WriteStartElement("td");
      xmlWriter.WriteAttributeString("class", "header");
      xmlWriter.WriteValue("User");
      xmlWriter.WriteEndElement(); // td
      xmlWriter.WriteEndElement(); // tr

      xmlWriter.WriteEndElement(); // table
      xmlWriter.WriteEndElement(); // body
      xmlWriter.WriteEndElement(); // html

      xmlWriter.Close();
    }


    //public static string CreateLiteralResultExpectedString (string result)
    //{
    //  var resultDoubleQuoted = result.Replace ("\"", "\"\"");
    //  return "\nconst string resultExpected =\n#region\n@\"" + resultDoubleQuoted + "\";\n#endregion\n";
    //}


    private CultureScope CultureScope_de_DE ()
    {
      return new CultureScope("de-DE");
    }


    private static XmlWriter CreateXmlWriter (TextWriter textWriter, bool indent)
    {
      XmlWriterSettings settings = new XmlWriterSettings();

      settings.OmitXmlDeclaration = true;
      settings.Indent = indent;
      settings.NewLineOnAttributes = false;
      //settings.ConformanceLevel = ConformanceLevel.Fragment;

      return XmlWriter.Create(textWriter, settings);
    }
  }


}
