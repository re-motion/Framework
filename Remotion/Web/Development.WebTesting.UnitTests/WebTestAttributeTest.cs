using NUnit.Framework;
using Remotion.Web.Development.WebTesting.UnitTests;

[assembly:TestWebTestAttribute("AssemblyKey1", "Assembly Value I")]
[assembly:TestWebTestAttribute("AssemblyKey2", "Assembly Value II")]
[assembly:TestWebTestAttribute("ClassKey3", "for override test")]
[assembly:TestWebTestAttribute("MethodKey4", "for override test")]

namespace Remotion.Web.Development.WebTesting.UnitTests;

[TestFixture]
[TestWebTestAttribute("ClassKey1", "Class Value I")]
[TestWebTestAttribute("ClassKey2", "Class Value II")]
[TestWebTestAttribute("ClassKey3", "Class Value III")]
[TestWebTestAttribute("MethodKey3", "for override test")]
public class WebTestAttributeTest
{
  [Test]
  [TestWebTestAttribute("MethodKey1", "Method Value I")]
  [TestWebTestAttribute("MethodKey2", "Method Value II")]
  public void CreatePropertiesFromAttributes_AppliesMethodAttributes ()
  {
    var properties = WebTestAttribute.CreatePropertiesFromAttributes(TestContext.CurrentContext.Test.Method!.MethodInfo);
    Assert.That(properties, Is.Not.Null);
    Assert.That(properties.ContainsKey("MethodKey1"));
    Assert.That(properties["MethodKey1"], Is.EqualTo("Method Value I"));
    Assert.That(properties.ContainsKey("MethodKey2"));
    Assert.That(properties["MethodKey2"], Is.EqualTo("Method Value II"));
  }

  [Test]
  [TestWebTestAttribute("MethodKey4", "Method Value IV")]
  public void CreatePropertiesFromAttributes_MethodAttributeCanOverrideAssemblyAttribute ()
  {
    var properties = WebTestAttribute.CreatePropertiesFromAttributes(TestContext.CurrentContext.Test.Method!.MethodInfo);
    Assert.That(properties, Is.Not.Null);
    Assert.That(properties.ContainsKey("MethodKey4"));
    Assert.That(properties["MethodKey4"], Is.EqualTo("Method Value IV"));
  }

  [Test]
  [TestWebTestAttribute("MethodKey3", "Method Value III")]
  public void CreatePropertiesFromAttributes_MethodAttributeCanOverrideClassAttribute ()
  {
    var properties = WebTestAttribute.CreatePropertiesFromAttributes(TestContext.CurrentContext.Test.Method!.MethodInfo);
    Assert.That(properties, Is.Not.Null);
    Assert.That(properties.ContainsKey("MethodKey3"));
    Assert.That(properties["MethodKey3"], Is.EqualTo("Method Value III"));
  }

  [Test]
  public void CreatePropertiesFromAttributes_AppliesClassAttributes ()
  {
    var properties = WebTestAttribute.CreatePropertiesFromAttributes(TestContext.CurrentContext.Test.Method!.MethodInfo);
    Assert.That(properties, Is.Not.Null);
    Assert.That(properties.ContainsKey("ClassKey1"));
    Assert.That(properties["ClassKey1"], Is.EqualTo("Class Value I"));
    Assert.That(properties.ContainsKey("ClassKey2"));
    Assert.That(properties["ClassKey2"], Is.EqualTo("Class Value II"));
  }

  [Test]
  public void CreatePropertiesFromAttributes_ClassAttributeCanOverrideAssemblyAttribute ()
  {
    var properties = WebTestAttribute.CreatePropertiesFromAttributes(TestContext.CurrentContext.Test.Method!.MethodInfo);
    Assert.That(properties, Is.Not.Null);
    Assert.That(properties.ContainsKey("ClassKey3"));
    Assert.That(properties["ClassKey3"], Is.EqualTo("Class Value III"));
  }

  [Test]
  public void CreatePropertiesFromAttributes_AppliesAssemblyAttributes ()
  {
    var properties = WebTestAttribute.CreatePropertiesFromAttributes(TestContext.CurrentContext.Test.Method!.MethodInfo);
    Assert.That(properties, Is.Not.Null);
    Assert.That(properties.ContainsKey("AssemblyKey1"));
    Assert.That(properties["AssemblyKey1"], Is.EqualTo("Assembly Value I"));
    Assert.That(properties.ContainsKey("ClassKey2"));
    Assert.That(properties["AssemblyKey2"], Is.EqualTo("Assembly Value II"));
  }

}
