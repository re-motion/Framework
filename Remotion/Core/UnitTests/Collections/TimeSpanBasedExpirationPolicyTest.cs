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

using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class TimeSpanBasedExpirationPolicyTest
  {
    private IUtcNowProvider _utcProviderStub;
    private TimeSpanBasedExpirationPolicy<string> _policy;

    [SetUp]
    public void SetUp ()
    {
      _utcProviderStub = MockRepository.GenerateStub<IUtcNowProvider>();

      SetCurrentTime (new DateTime (5));
      _policy = new TimeSpanBasedExpirationPolicy<string> (TimeSpan.FromTicks (1), _utcProviderStub);
    }

    [Test]
    public void GetNextScanInfo ()
    {
      Assert.That (_policy.GetNextScanInfo(), Is.EqualTo (new DateTime (6)));
    }

    [Test]
    public void GetExpirationInfo ()
    {
      var result = _policy.GetExpirationInfo ("Test");

      Assert.That (result, Is.EqualTo (new DateTime (6)));
    }

    [Test]
    public void IsExpired_True ()
    {
      var result = _policy.IsExpired ("Test", new DateTime(0));

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsExpired_True_ExactSameTime ()
    {
      var result = _policy.IsExpired ("Test", new DateTime (5));

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsExpired_False ()
    {
      var result = _policy.IsExpired ("Test", new DateTime (6));

      Assert.That (result, Is.False);
    }

    [Test]
    public void ShouldScanForExpiredItems_False ()
    {
      Assert.That (_policy.ShouldScanForExpiredItems (new DateTime (6)), Is.False);
    }

    [Test]
    public void ShouldScanForExpiredItems_True ()
    {
      SetCurrentTime (new DateTime (7));

      Assert.That (_policy.ShouldScanForExpiredItems (new DateTime (6)), Is.True);
    }

    [Test]
    public void ShouldScanForExpiredItems_True_ExactSameTime ()
    {
      SetCurrentTime (new DateTime (6));

      Assert.That (_policy.ShouldScanForExpiredItems (new DateTime (6)), Is.True);
    }

    private void SetCurrentTime (DateTime time)
    {
      _utcProviderStub.BackToRecord ();
      _utcProviderStub.Stub (stub => stub.UtcNow).Return (time);
      _utcProviderStub.Replay();
      return;
    }
  }
}