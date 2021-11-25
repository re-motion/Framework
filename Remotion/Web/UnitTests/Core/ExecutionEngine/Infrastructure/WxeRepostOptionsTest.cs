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
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class WxeRepostOptionsTest
  {
    [Test]
    public void SuppressRepost_UsesEventTargetTrue ()
    {
      var sender = new Control();
      var options = WxeRepostOptions.SuppressRepost(sender, true);

      Assert.That(options.Sender, Is.SameAs(sender));
      Assert.That(options.SuppressesRepost, Is.True);
      Assert.That(options.UsesEventTarget, Is.True);
    }

    [Test]
    public void SuppressRepost_SenderImplementsIPostBackEventHandler ()
    {
      var sender = new Mock<Control>();
      var options = WxeRepostOptions.SuppressRepost((Control)sender.As<IPostBackEventHandler>().Object, false);

      Assert.That(options.Sender, Is.SameAs(sender.Object));
      Assert.That(options.SuppressesRepost, Is.True);
      Assert.That(options.UsesEventTarget, Is.False);
    }

    [Test]
    public void SuppressRepost_SenderImplementsIPostBackDataHandler ()
    {
      var sender = new Mock<Control>();
      var options = WxeRepostOptions.SuppressRepost((Control)sender.As<IPostBackDataHandler>().Object, false);

      Assert.That(options.Sender, Is.SameAs(sender.Object));
      Assert.That(options.SuppressesRepost, Is.True);
      Assert.That(options.UsesEventTarget, Is.False);
    }

    [Test]
    public void SuppressRepost_SenderNotIPostBackDataHandler_And_SenderNotIPostBackDataHandler_ThrowsArgumentException ()
    {
      Assert.That(
          () => WxeRepostOptions.SuppressRepost(new Control(), false),
          Throws.ArgumentException
              .With.Message.EqualTo(
                  "The 'sender' must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event."));
    }

    [Test]
    public void SuppressRepost_NoSender_ThrowsArgumentNullException ()
    {
      Assert.That(
          () => WxeRepostOptions.SuppressRepost(null, false),
          Throws.InstanceOf<ArgumentNullException>()
              .With.Message.Contains("sender"));
      // ReSharper restore AssignNullToNotNullAttribute
    }

    [Test]
    public void DoRepost_NoSender ()
    {
      var options = WxeRepostOptions.DoRepost(null);

      Assert.That(options.Sender, Is.Null);
      Assert.That(options.SuppressesRepost, Is.False);
      Assert.That(options.UsesEventTarget, Is.False);
    }

    [Test]
    public void DoRepost_HasSender ()
    {
      var sender = new Control();
      var options = WxeRepostOptions.DoRepost(sender);

      Assert.That(options.Sender, Is.SameAs(sender));
      Assert.That(options.SuppressesRepost, Is.False);
      Assert.That(options.UsesEventTarget, Is.False);
    }
  }
}
