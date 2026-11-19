// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using NUnit.Framework;
using Remotion.Web.ContentSecurityPolicy;

namespace Remotion.Web.UnitTests.Core.ContentSecurityPolicy;

[TestFixture]
public class CollectingCspClientScriptManagerTest
{
  [Test]
  public void RegisterScript ()
  {
    var scriptManager = new CollectingCspClientScriptManager();

    scriptManager.RegisterScript("key-A", "Script1");
    scriptManager.RegisterScript("key-B", "Script2");
    scriptManager.RegisterScript("key-A", "Script3");
    scriptManager.RegisterScript("key-C", "Script4");

    Assert.That(
        scriptManager.GetCollectedScripts(),
        Is.EqualTo(
            """
            Script2
            Script3
            Script4
            """));
  }
}
