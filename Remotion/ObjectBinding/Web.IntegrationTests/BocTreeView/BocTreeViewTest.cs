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
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.IntegrationTests.BocTreeView;

[TestFixture]
public class BocTreeViewTest : IntegrationTest
{
  [Test]
  [Category("LongRunning")]
  public void WebTreeView_ExpansionOfEmptyEmitsNoItemFoundText ()
  {
    var logger = Helper.LoggerFactory.CreateLogger<BocTreeViewTest>();

    var home = Start();
    var webTreeView = home.TreeViews().GetByLocalID("NoLookAheadEvaluation");

    var firstNode = webTreeView.GetNode(1).Expand();
    var notExpandableNode = firstNode.GetNode(2);

    var ariaTreeAlertSpan = webTreeView.Scope.FindCss(".bocTreeView span[aria-live=assertive] span");

    Assert.That(ariaTreeAlertSpan.InnerHTML, Is.Empty);
    Assert.That(ariaTreeAlertSpan.GetAttribute("aria-hidden", logger), Is.EqualTo("true"));

    notExpandableNode.Expand();

    Assert.That(ariaTreeAlertSpan.InnerHTML, Is.EqualTo("B, D Does not contain any items."));
    Assert.That(ariaTreeAlertSpan.GetAttribute("aria-hidden", logger), Is.EqualTo("false"));

    Assert.That(() => ariaTreeAlertSpan.GetAttribute("aria-hidden", logger), Is.EqualTo("true").After(7).Seconds.PollEvery(200).MilliSeconds);
  }

  private WxePageObject Start ()
  {
    return Start("BocTreeView");
  }
}
