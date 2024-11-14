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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Web.Resources;

namespace Remotion.Web.UnitTests.Core.Resources;

[TestFixture]
public class RemotionStaticFilesCacheKeyRemovalRewriteRuleTest
{
  [Test]
  public void Constructor_EmptyRequestPath_Throws ()
  {
    Assert.That(
        () => new RemotionStaticFilesCacheKeyRemovalRewriteRule(PathString.Empty, false),
        Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
            "Request path must have a value.",
            "requestPath"));
  }

  [Test]
  public void Constructor_RequestPathWithNoTrailingSlash_Throws ()
  {
    Assert.That(
        () => new RemotionStaticFilesCacheKeyRemovalRewriteRule("/abc/", false),
        Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
            "Request path must not end in a slash.",
            "requestPath"));
  }

  [Test]
  [TestCase("/test")]
  [TestCase("/test.txt")]
  [TestCase("/sub/a")]
  [TestCase("/other/cache_aaaa/test")]
  [TestCase("/static")]
  [TestCase("/static/")]
  [TestCase("/static/test")]
  [TestCase("/static/sub/test.txt")]
  [TestCase("/static/cacheabc/test.txt")]
  [TestCase("/static/cache_")]
  [TestCase("/static/cache_abc")]
  public void ApplyRule_WithNormalUrl_DoesNotChangeRequest (string path)
  {
    var middleware = new RemotionStaticFilesCacheKeyRemovalRewriteRule("/static", false);

    var httpContext = new DefaultHttpContext();
    httpContext.Request.Path = path;

    var rewriteContext = new RewriteContext();
    rewriteContext.HttpContext = httpContext;
    middleware.ApplyRule(rewriteContext);

    Assert.That(rewriteContext.Result, Is.EqualTo(RuleResult.ContinueRules));
    Assert.That(httpContext.Request.Path, Is.EqualTo((PathString)path));
    Assert.That(httpContext.Items.Count, Is.EqualTo(0));

    var hasCacheKey = RemotionStaticFilesCacheKeyRemovalRewriteRule.TryGetRemovedCacheKey(httpContext, out _);
    Assert.That(hasCacheKey, Is.False);
  }

  [Test]
  [TestCase("/static/cache_/", "/static/", "")]
  [TestCase("/static/cache_abc/", "/static/", "abc")]
  [TestCase("/static/cache_$$$/test/abc.txt", "/static/test/abc.txt", "$$$")]
  public void ApplyRule_WithCachingUrl_ChangesRequestPathAndAddsCacheKeyToItems (string path, string newPath, string cacheKey)
  {
    var middleware = new RemotionStaticFilesCacheKeyRemovalRewriteRule("/static", true);

    var httpContext = new DefaultHttpContext();
    httpContext.Request.Path = path;

    var rewriteContext = new RewriteContext();
    rewriteContext.HttpContext = httpContext;
    middleware.ApplyRule(rewriteContext);

    Assert.That(rewriteContext.Result, Is.EqualTo(RuleResult.SkipRemainingRules));
    Assert.That(httpContext.Request.Path, Is.EqualTo((PathString)newPath));
    Assert.That(httpContext.Items.Count, Is.EqualTo(1));

    var hasCacheKey = RemotionStaticFilesCacheKeyRemovalRewriteRule.TryGetRemovedCacheKey(httpContext, out var actualCacheKey);
    Assert.That(hasCacheKey, Is.True);
    Assert.That(actualCacheKey, Is.EqualTo(cacheKey));
  }
}
