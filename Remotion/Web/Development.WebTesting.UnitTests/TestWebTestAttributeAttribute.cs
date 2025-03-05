using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.UnitTests;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class TestWebTestAttributeAttribute : WebTestAttribute
{
  public TestWebTestAttributeAttribute ([NotNull] string propertyKey, [NotNull] object propertyValue)
  {
    PropertyKey = propertyKey;
    PropertyValue = propertyValue;
  }

  [NotNull]
  public string PropertyKey { get; }

  [NotNull]
  public object PropertyValue { get; }

  public override void ApplyValue (IDictionary<string, object> properties)
  {
    properties[PropertyKey] = PropertyValue;
  }
}
