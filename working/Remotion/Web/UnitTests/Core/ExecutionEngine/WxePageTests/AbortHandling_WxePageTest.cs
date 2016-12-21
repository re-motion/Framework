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
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.WxePageTests
{
  [TestFixture]
  public class AbortHandling_WxePageTest : WxeTest
  {
    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
    }

    [Test]
    public void TestIsAbortEnabled ()
    {
      WxePage page = new WxePage();

      page.EnableAbort = null;
      Assert.That (
          ((IWxePage) page).IsAbortEnabled,
          Is.True,
          "Abort disabled with EnableAbort=Undefined.");

      page.EnableAbort = true;
      Assert.That (
          ((IWxePage) page).IsAbortEnabled,
          Is.True,
          "Abort disabled with EnableAbort=True.");

      page.EnableAbort = false;
      Assert.That (
          ((IWxePage) page).IsAbortEnabled,
          Is.False,
          "Abort enabled with EnableAbort=False.");
    }


    [Test]
    public void TestIsAbortConfimationEnabledWithAbortEnabledAndPageNotDirty ()
    {
      WxePage page = new WxePage();
      page.EnableAbort = true;
      page.IsDirty = false;

      page.ShowAbortConfirmation = ShowAbortConfirmation.Always;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.True,
          "Abort confirmation disabled with ShowAbortConfirmation=Always.");

      page.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.True,
          "Abort confirmation disabled with ShowAbortConfirmation=OnlyIfDirty.");

      page.ShowAbortConfirmation = ShowAbortConfirmation.Never;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.False,
          "Abort confirmation enabled with ShowAbortConfirmation=Never.");
    }

    [Test]
    public void TestIsAbortConfimationEnabledWithAbortEnabledTrueAndPageDirty ()
    {
      WxePage page = new WxePage();
      page.EnableAbort = true;
      page.IsDirty = true;

      page.ShowAbortConfirmation = ShowAbortConfirmation.Always;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.True,
          "Abort confirmation disabled with ShowAbortConfirmation=Always.");

      page.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.True,
          "Abort confirmation disabledwith ShowAbortConfirmation=OnlyIfDirty.");

      page.ShowAbortConfirmation = ShowAbortConfirmation.Never;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.False,
          "Abort confirmation enabled with ShowAbortConfirmation=Never.");
    }

    [Test]
    public void TestIsAbortConfimationEnabledWithAbortDisabledFalseAndPageNotDirty ()
    {
      WxePage page = new WxePage();
      page.EnableAbort = false;
      page.IsDirty = false;

      page.ShowAbortConfirmation = ShowAbortConfirmation.Always;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.False,
          "Abort confirmation enabled with ShowAbortConfirmation=Always.");

      page.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.False,
          "Abort confirmation enabled with ShowAbortConfirmation=OnlyIfDirty.");

      page.ShowAbortConfirmation = ShowAbortConfirmation.Never;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.False,
          "Abort confirmation enabled with ShowAbortConfirmation=Never.");
    }

    [Test]
    public void TestIsAbortConfimationEnabledWithAbortDisabledFalseAndPageDirty ()
    {
      WxePage page = new WxePage();
      page.EnableAbort = false;
      page.IsDirty = true;

      page.ShowAbortConfirmation = ShowAbortConfirmation.Always;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.False,
          "Abort confirmation enabled with ShowAbortConfirmation=Always.");

      page.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.False,
          "Abort confirmation enabled with ShowAbortConfirmation=OnlyIfDirty.");

      page.ShowAbortConfirmation = ShowAbortConfirmation.Never;
      Assert.That (
          ((IWxePage) page).IsAbortConfirmationEnabled,
          Is.False,
          "Abort confirmation enabled with ShowAbortConfirmation=Never.");
    }


    [Test]
    public void TestAreOutOfSequencePostBacksEnabledWithAbortEnabled ()
    {
      WxePage page = new WxePage();
      page.EnableAbort = true;

      page.EnableOutOfSequencePostBacks = null;
      Assert.That (
          ((IWxePage) page).AreOutOfSequencePostBacksEnabled,
          Is.False,
          "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=Undefined.");

      page.EnableOutOfSequencePostBacks = true;
      Assert.That (
          ((IWxePage) page).AreOutOfSequencePostBacksEnabled,
          Is.False,
          "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=True.");

      page.EnableOutOfSequencePostBacks = false;
      Assert.That (
          ((IWxePage) page).AreOutOfSequencePostBacksEnabled,
          Is.False,
          "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=False.");
    }

    [Test]
    public void TestAreOutOfSequencePostBacksEnabledWithAbortDisabled ()
    {
      WxePage page = new WxePage();
      page.EnableAbort = false;

      page.EnableOutOfSequencePostBacks = null;
      Assert.That (
          ((IWxePage) page).AreOutOfSequencePostBacksEnabled,
          Is.False,
          "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=Undefined.");

      page.EnableOutOfSequencePostBacks = true;
      Assert.That (
          ((IWxePage) page).AreOutOfSequencePostBacksEnabled,
          Is.True,
          "Out-of-sequence postbacks disabled with EnableOutOfSequencePostBacks=True.");

      page.EnableOutOfSequencePostBacks = false;
      Assert.That (
          ((IWxePage) page).AreOutOfSequencePostBacksEnabled,
          Is.False,
          "Out-of-sequence postbacks enabled with EnableOutOfSequencePostBacks=False.");
    }
  }
}