// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
namespace Remotion.Web.ContentSecurityPolicy;

/// <summary>
/// Represents a directive in a Content Security Policy (CSP) header.
/// </summary>
/// <remarks>
/// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
/// </remarks>
public enum CspDirective
{
  /// <summary>
  /// The 'base-uri' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/base-uri
  /// </remarks>
  BaseUri,

  /// <summary>
  /// The 'child-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/child-src
  /// </remarks>
  ChildSrc,

  /// <summary>
  /// The 'connect-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/connect-src
  /// </remarks>
  ConnectSrc,

  /// <summary>
  /// The 'default-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/default-src
  /// </remarks>
  DefaultSrc,

  /// <summary>
  /// The 'font-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/font-src
  /// </remarks>
  FontSrc,

  /// <summary>
  /// The 'form-action' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/form-action
  /// </remarks>
  FormAction,

  /// <summary>
  /// The 'frame-ancestors' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/frame-ancestors
  /// </remarks>
  FrameAncestors,

  /// <summary>
  /// The 'frame-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/frame-src
  /// </remarks>
  FrameSrc,

  /// <summary>
  /// The 'img-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/img-src
  /// </remarks>
  ImgSrc,

  /// <summary>
  /// The 'manifest-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/manifest-src
  /// </remarks>
  ManifestSrc,

  /// <summary>
  /// The 'media-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/media-src
  /// </remarks>
  MediaSrc,

  /// <summary>
  /// The 'object-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/object-src
  /// </remarks>
  ObjectSrc,

  /// <summary>
  /// The 'report-to' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/report-to
  /// </remarks>
  ReportTo,

  /// <summary>
  /// The 'sandbox' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/sandbox
  /// </remarks>
  Sandbox,

  /// <summary>
  /// The 'script-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/script-src
  /// </remarks>
  ScriptSrc,

  /// <summary>
  /// The 'script-src-attr' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/script-src-attr
  /// </remarks>
  ScriptSrcAttr,

  /// <summary>
  /// The 'script-src-elem' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/script-src-elem
  /// </remarks>
  ScriptSrcElem,

  /// <summary>
  /// The 'style-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/style-src
  /// </remarks>
  StyleSrc,

  /// <summary>
  /// The 'style-src-elem' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/style-src-elem
  /// </remarks>
  StyleSrcElem,

  /// <summary>
  /// The 'worker-src' CSP directive.
  /// </summary>
  /// <remarks>
  /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/worker-src
  /// </remarks>
  WorkerSrc,
}
