using System;
using System.Reflection;

// We leave this in Remotion.Web as it makes sharing the file between multiple projects easier
// ReSharper disable once CheckNamespace
namespace Remotion.Web
{
  internal class WebDiagnosticIDs
  {
    /// <summary>
    /// Diagnostic ID reported when a WebString or PlainTextString are used incorrectly.
    /// When using those types, implicitly using .ToString() might lead to incorrectly encoded values.
    /// </summary>
    public const string RMWEB0001_WrongWebStringUsage = "RMWEB0001";

    /// <summary>
    /// Diagnostic ID reported when a constructor of WxeResourcePageStep is used without a type/assembly argument is used.
    /// This can lead to problems as the target assembly is determined using <see cref="Assembly"/>.<see cref="Assembly.GetCallingAssembly"/>,
    /// which can return incorrect results when method are inlined.
    /// </summary>
    /// <example>
    /// <code>
    /// new WxeResourcePageStep("Test.aspx"); // &lt;-- RMWEB0002 emitted
    /// new WxeResourcePageStep(new WxeVariableReference("test")); // &lt;-- RMWEB0002 emitted
    /// // vs.
    /// new WxeResourcePageStep(typeof(MyClass), "Test.aspx"); // &lt;-- OK
    /// new WxeResourcePageStep(typeof(MyClass), new WxeVariableReference("test")); // &lt;-- OK
    /// </code>
    /// </example>
    public const string RMWEB0002_ObsoleteWxeResourcePageStepConstructor = "RMWEB0002";

    /// <summary>
    /// Diagnostic ID reported when a type is passed in the constructor of WxeResourcePageStep that does not belong to the calling assembly.
    /// This is most likely a (copy &amp; paste)-mistake as the passed type usually belongs to the same assembly.
    /// </summary>
    /// <example>
    /// <code>
    /// new WxeResourcePageStep(typeof(int), "Test.aspx"); // &lt;-- RMWEB0003 emitted
    /// new WxeResourcePageStep(typeof(string), new WxeVariableReference("test")); // &lt;-- RMWEB0003 emitted
    /// vs.
    /// new WxeResourcePageStep(typeof(MyClass), "Test.aspx"); // &lt;-- OK (assuming MyClass is the declaring class, or it is in part of the same assembly)
    /// new WxeResourcePageStep(typeof(MyClass), new WxeVariableReference("test")); // &lt;-- OK (see comment above)
    /// </code>
    /// </example>
    public const string RMWEB0003_PossiblyInvalidTypeForWxeResourcePageStepConstructor = "RMWEB0003";
  }
}
