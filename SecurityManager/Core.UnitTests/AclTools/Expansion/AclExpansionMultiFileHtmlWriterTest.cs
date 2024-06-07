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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionMultiFileHtmlWriterTest : AclToolsTestBase
  {
    [Test]
    public void TextWriterFactoryResultTest ()
    {
      using (new CultureScope("en-US"))
      {
        var aclExpander = new AclExpander();
        var aclExpansionEntryList = aclExpander.GetAclExpansionEntryList();
        var stringWriterFactory = new StringWriterFactory();

        stringWriterFactory.Directory = "";
        stringWriterFactory.Extension = "xYz";

        var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter(stringWriterFactory, false);
        aclExpansionMultiFileHtmlWriter.WriteAclExpansion(aclExpansionEntryList);

        Assert.That(stringWriterFactory.Count, Is.EqualTo(7));

        AssertTextWriterFactoryMemberEquals(
            stringWriterFactory,
            "_AclExpansionMain_",
            "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion - Users</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-user-table\"><tr><th class=\"header\">User</th><th class=\"header\">First name</th><th class=\"header\">Last name</th><th class=\"header\">Permissions granted</th></tr><tr><td>substituting.user</td><td></td><td>substitute</td><td><a href=\".\\substituting.user.xYz\" target=\"_blank\">.\\substituting.user.xYz</a></td></tr><tr><td>test.user</td><td>test</td><td>user</td><td><a href=\".\\test.user.xYz\" target=\"_blank\">.\\test.user.xYz</a></td></tr><tr><td>group0/user1</td><td></td><td>user1</td><td><a href=\".\\group0_user1.xYz\" target=\"_blank\">.\\group0_user1.xYz</a></td></tr><tr><td>group1/user1</td><td></td><td>user1</td><td><a href=\".\\group1_user1.xYz\" target=\"_blank\">.\\group1_user1.xYz</a></td></tr><tr><td>group0/user2</td><td></td><td>user2</td><td><a href=\".\\group0_user2.xYz\" target=\"_blank\">.\\group0_user2.xYz</a></td></tr><tr><td>group1/user2</td><td></td><td>user2</td><td><a href=\".\\group1_user2.xYz\" target=\"_blank\">.\\group1_user2.xYz</a></td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals(
            stringWriterFactory,
            "test.user",
            "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">Status</th><th class=\"header\">User must be owner</th><th class=\"header\">Object group is set to</th><th class=\"header\">Object tenant is set to</th><th class=\"header\">User must have abstract role</th><th class=\"header\">Permissions granted</th></tr><tr><td rowspan=\"8\">user test, Dipl.Ing.(FH)</td><td rowspan=\"2\">testGroup, Manager</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr><tr><td rowspan=\"2\">testGroup, Official</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr><tr><td rowspan=\"2\">testOwningGroup, Manager</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr><tr><td rowspan=\"2\">testRootGroup, Official</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals(
            stringWriterFactory,
            "group0_user1",
            "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">Status</th><th class=\"header\">User must be owner</th><th class=\"header\">Object group is set to</th><th class=\"header\">Object tenant is set to</th><th class=\"header\">User must have abstract role</th><th class=\"header\">Permissions granted</th></tr><tr><td rowspan=\"2\">user1</td><td rowspan=\"2\">PG0 (parentGroup0), Manager</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals(
            stringWriterFactory,
            "group1_user1",
            "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">Status</th><th class=\"header\">User must be owner</th><th class=\"header\">Object group is set to</th><th class=\"header\">Object tenant is set to</th><th class=\"header\">User must have abstract role</th><th class=\"header\">Permissions granted</th></tr><tr><td rowspan=\"2\">user1</td><td rowspan=\"2\">PG1 (parentGroup1), Manager</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals(
            stringWriterFactory,
            "group0_user2",
            "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">Status</th><th class=\"header\">User must be owner</th><th class=\"header\">Object group is set to</th><th class=\"header\">Object tenant is set to</th><th class=\"header\">User must have abstract role</th><th class=\"header\">Permissions granted</th></tr><tr><td rowspan=\"2\">user2</td><td rowspan=\"2\">PG0 (parentGroup0), Official</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
        AssertTextWriterFactoryMemberEquals(
            stringWriterFactory,
            "group1_user2",
            "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"\"><html><head><title>re-motion ACL Expansion</title><style>@import \"AclExpansion.css\";</style><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" /></head><body><table style=\"width: 100%;\" class=\"aclExpansionTable\" id=\"remotion-ACL-expansion-table\"><tr><th class=\"header\">User</th><th class=\"header\">Role</th><th class=\"header\">Class</th><th class=\"header\">Status</th><th class=\"header\">User must be owner</th><th class=\"header\">Object group is set to</th><th class=\"header\">Object tenant is set to</th><th class=\"header\">User must have abstract role</th><th class=\"header\">Permissions granted</th></tr><tr><td rowspan=\"2\">user2</td><td rowspan=\"2\">PG1 (parentGroup1), Official</td><td rowspan=\"2\">Order</td><td>DHL, None, Received</td><td></td><td></td><td></td><td></td><td>FirstAccessType</td></tr><tr><td>DHL, None, Received</td><td></td><td></td><td>TestTenant</td><td></td><td>FirstAccessType, Write</td></tr></table></body></html>");
      }
    }


    [Test]
    public void GetUsersTest ()
    {
      var aclExpansionEntryList = new List<AclExpansionEntry>();
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User));
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User3));
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User2));
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User2));
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User));
      var userList = AclExpansionMultiFileHtmlWriter.GetUsers(aclExpansionEntryList);
      Assert.That(userList, Is.EqualTo(new List<User> { User3, User2, User }));
    }

    [Test]
    public void GetAccessControlEntriesForUserTest ()
    {
      AclExpansionEntry aclExpansionEntry0 = GetAclExpansionEntryWithUser(User);
      AclExpansionEntry aclExpansionEntry1 = GetAclExpansionEntryWithUser(User);
      AclExpansionEntry aclExpansionEntry2 = GetAclExpansionEntryWithUser(User);

      var aclExpansionEntryList = new List<AclExpansionEntry>();
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User3));
      aclExpansionEntryList.Add(aclExpansionEntry0);
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User3));
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User2));
      aclExpansionEntryList.Add(aclExpansionEntry1);
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User3));
      aclExpansionEntryList.Add(aclExpansionEntry2);
      aclExpansionEntryList.Add(GetAclExpansionEntryWithUser(User2));

      var aclExpansionEntryListResult = AclExpansionMultiFileHtmlWriter.GetAccessControlEntriesForUser(aclExpansionEntryList, User);
      Assert.That(aclExpansionEntryListResult, Is.EquivalentTo(ListObjectMother.New( aclExpansionEntry0, aclExpansionEntry1, aclExpansionEntry2 )));
      Assert.That(aclExpansionEntryListResult, Is.EquivalentTo(ListObjectMother.New(aclExpansionEntry0, aclExpansionEntry1, aclExpansionEntry2)));
    }


    private AclExpansionEntry GetAclExpansionEntryWithUser (User user)
    {
      return new AclExpansionEntry(
          user, Role, Acl, new AclExpansionAccessConditions(), new AccessTypeDefinition[0], new AccessTypeDefinition[0]);
    }


    [Test]
    public void ToValidFileNameTest ()
    {
      const string unityInput = "µabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      //const string forbiddenInput =  "\"?/\\*:";
      string forbiddenInput = new string(Path.GetInvalidFileNameChars());
      string forbiddenInputResult = new String('_', forbiddenInput.Length);
      Assert.That(AclExpansionHtmlWriterImplementationBase.ToValidFileName(unityInput), Is.EqualTo(unityInput));
      Assert.That(AclExpansionHtmlWriterImplementationBase.ToValidFileName(forbiddenInput), Is.EqualTo(forbiddenInputResult));
      Assert.That(
          AclExpansionHtmlWriterImplementationBase.ToValidFileName(forbiddenInput + unityInput + forbiddenInput + unityInput),
          Is.EqualTo(forbiddenInputResult + unityInput + forbiddenInputResult + unityInput));
    }



    [Test]
    public void DetailHtmlWriterSettingsTest ()
    {
      var aclExpansionMultiFileHtmlWriter = new AclExpansionMultiFileHtmlWriter(new Mock<ITextWriterFactory>().Object, true);
      var settings = new AclExpansionHtmlWriterSettings();
      aclExpansionMultiFileHtmlWriter.DetailHtmlWriterSettings = settings;
      Assert.That(aclExpansionMultiFileHtmlWriter.DetailHtmlWriterSettings, Is.EqualTo(settings));
    }

    private void AssertTextWriterFactoryMemberEquals (StringWriterFactory stringWriterFactory, string name, string resultExpected)
    {
      var textWriterData = stringWriterFactory.GetTextWriterData(name);
      string result = textWriterData.TextWriter.ToString();
      Assert.That(result, Is.EqualTo(resultExpected));
    }
  }
}
