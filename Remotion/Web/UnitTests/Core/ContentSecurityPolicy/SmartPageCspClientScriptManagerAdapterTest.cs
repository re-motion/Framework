// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using Moq;
using NUnit.Framework;
using Remotion.Web.ContentSecurityPolicy;
using Remotion.Web.UI;

namespace Remotion.Web.UnitTests.Core.ContentSecurityPolicy;

[TestFixture]
public class SmartPageCspClientScriptManagerAdapterTest
{
  [Test]
  public void RegisterScript ()
  {
    var smartPageStub = new Mock<ISmartPage>(MockBehavior.Strict);
    var clientScriptManagerMock = new Mock<ISmartPageClientScriptManager>(MockBehavior.Strict);

    smartPageStub.Setup(e => e.ClientScript).Returns(clientScriptManagerMock.Object);

    clientScriptManagerMock.Setup(e =>
            e.RegisterStartupScriptBlock(
                smartPageStub.Object,
                typeof(CspEnabledHtmlTextWriter),
                "key",
                "script"))
        .Verifiable();

    var adapter = new SmartPageCspClientScriptManagerAdapter(smartPageStub.Object);
    adapter.RegisterScript("key", "script");

    clientScriptManagerMock.Verify();
  }
}
