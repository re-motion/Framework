// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class DefaultBocTextValueLineEndingServiceTest
  {
    [Test]
    public void GetLineEnding_ReturnsWindowsStyle ()
    {
      var service = new DefaultBocTextValueLineEndingService();

      Assert.That(service.GetLineEnding(new BocTextValue()), Is.EqualTo("\r\n"));
    }
  }
}
