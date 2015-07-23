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
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Permissions;
using System.Web;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Sandboxing;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  [TestFixture]
  public class PermissionSetsTest
  {
    [Test]
    public void GetMediumTrust ()
    {
      var tempDirectory = Environment.GetEnvironmentVariable ("TEMP");
      var mediumTrustPermissions = PermissionSets.GetMediumTrust (tempDirectory, Environment.MachineName);
      var directoryName = Path.GetDirectoryName (tempDirectory);

      Assert.That (mediumTrustPermissions.Length, Is.EqualTo (11));
      Assert.That (((AspNetHostingPermission) mediumTrustPermissions[0]).Level, Is.EqualTo (AspNetHostingPermissionLevel.Medium));
      Assert.That (((DnsPermission) mediumTrustPermissions[1]).IsUnrestricted(), Is.True);
      Assert.That (
          ((EnvironmentPermission) mediumTrustPermissions[2]).GetPathList (EnvironmentPermissionAccess.Read),
          Is.EqualTo ("TEMP;TMP;USERNAME;OS;COMPUTERNAME"));
      Assert.That (
          Path.GetDirectoryName (((FileIOPermission) mediumTrustPermissions[3]).GetPathList (FileIOPermissionAccess.Read)[0]),
          Is.EqualTo (directoryName));
      Assert.That (
          Path.GetDirectoryName (((FileIOPermission) mediumTrustPermissions[3]).GetPathList (FileIOPermissionAccess.Write)[0]),
          Is.EqualTo (directoryName));
      Assert.That (
          Path.GetDirectoryName (((FileIOPermission) mediumTrustPermissions[3]).GetPathList (FileIOPermissionAccess.Append)[0]),
          Is.EqualTo (directoryName));
      Assert.That (
          Path.GetDirectoryName (((FileIOPermission) mediumTrustPermissions[3]).GetPathList (FileIOPermissionAccess.PathDiscovery)[0]),
          Is.EqualTo (directoryName));
      Assert.That (
          ((IsolatedStorageFilePermission) mediumTrustPermissions[4]).UsageAllowed, Is.EqualTo (IsolatedStorageContainment.AssemblyIsolationByUser));
      Assert.That (((IsolatedStorageFilePermission) mediumTrustPermissions[4]).UserQuota, Is.EqualTo (9223372036854775807L));
      Assert.That (((PrintingPermission) mediumTrustPermissions[5]).Level, Is.EqualTo (PrintingPermissionLevel.DefaultPrinting));
      Assert.That (
          ((SecurityPermission) mediumTrustPermissions[6]).Flags,
          Is.EqualTo (
              SecurityPermissionFlag.Assertion
              | SecurityPermissionFlag.Execution
              | SecurityPermissionFlag.ControlThread
              | SecurityPermissionFlag.ControlPrincipal
              | SecurityPermissionFlag.RemotingConfiguration));
      Assert.That (((SmtpPermission) mediumTrustPermissions[7]).Access, Is.EqualTo (SmtpAccess.Connect));
      Assert.That (((SqlClientPermission) mediumTrustPermissions[8]).IsUnrestricted(), Is.True);
      Assert.That (mediumTrustPermissions[9], Is.TypeOf (typeof (WebPermission)));
      Assert.That (((ReflectionPermission) mediumTrustPermissions[10]).Flags, Is.EqualTo (ReflectionPermissionFlag.RestrictedMemberAccess));
    }
  }
}