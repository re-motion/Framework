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
using System.CodeDom;
using System.Web.Compilation;
using System.Web.UI;
using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Utilities;
using ResourceExpressionBuilder = Remotion.Web.Compilation.ResourceExpressionBuilder;

namespace Remotion.Web.UnitTests.Core.Compilation
{
  [TestFixture]
  public class ResourceExpressionBuilderTest
  {
    [Test]
    public void ParseExpression_ReturnsInputExpression ()
    {
      var resourceExpressionBuilder = new ResourceExpressionBuilder();
      var parsedExpression = resourceExpressionBuilder.ParseExpression (
          "the expression",
          typeof (WebString),
          new ExpressionBuilderContext ("theControl.ascx"));

      Assert.That (parsedExpression, Is.EqualTo ("the expression"));
    }

    [Test]
    public void GetCodeExpression_ForStringProperty ()
    {
      var resourceExpressionBuilder = new ResourceExpressionBuilder();
      var boundPropertyEntry = (BoundPropertyEntry) Activator.CreateInstance(typeof (BoundPropertyEntry), nonPublic:true);
      boundPropertyEntry.PropertyInfo = MemberInfoFromExpressionUtility.GetProperty (() => StringProperty);
      object parsedData = "the expression";
      var expressionBuilderContext = new ExpressionBuilderContext("theControl.ascx");
      var codeExpression = resourceExpressionBuilder.GetCodeExpression (boundPropertyEntry, parsedData, expressionBuilderContext);

      Assert.That (codeExpression, Is.InstanceOf<CodeMethodInvokeExpression>());
      var codeMethodInvokeExpression = (CodeMethodInvokeExpression) codeExpression;
      Assert.That (codeMethodInvokeExpression.Method, Is.Not.Null);
      Assert.That (codeMethodInvokeExpression.Method.MethodName, Is.EqualTo (nameof (ResourceExpressionBuilder.GetStringForResourceID)));
      Assert.That (codeMethodInvokeExpression.Method.TargetObject, Is.InstanceOf<CodeTypeReferenceExpression>());
      Assert.That (
          ((CodeTypeReferenceExpression) codeMethodInvokeExpression.Method.TargetObject).Type.BaseType,
          Is.EqualTo (typeof (ResourceExpressionBuilder).FullName));
      Assert.That (
          codeMethodInvokeExpression.Parameters.Count,
          Is.EqualTo (
              MemberInfoFromExpressionUtility.GetMethod (
                      () => ResourceExpressionBuilder.GetStringForResourceID (null, null)).GetParameters()
                  .Length));
      Assert.That (codeMethodInvokeExpression.Parameters[0], Is.InstanceOf<CodeThisReferenceExpression>());
      Assert.That (codeMethodInvokeExpression.Parameters[1], Is.InstanceOf<CodePrimitiveExpression>());
      Assert.That (((CodePrimitiveExpression) codeMethodInvokeExpression.Parameters[1]).Value, Is.EqualTo ("the expression"));
    }

    [Test]
    public void GetCodeExpression_ForWebStringProperty ()
    {
      var resourceExpressionBuilder = new ResourceExpressionBuilder();
      var boundPropertyEntry = (BoundPropertyEntry) Activator.CreateInstance(typeof (BoundPropertyEntry), nonPublic:true);
      boundPropertyEntry.PropertyInfo = MemberInfoFromExpressionUtility.GetProperty (() => WebStringProperty);
      object parsedData = "the expression";
      var expressionBuilderContext = new ExpressionBuilderContext("theControl.ascx");
      var codeExpression = resourceExpressionBuilder.GetCodeExpression (boundPropertyEntry, parsedData, expressionBuilderContext);

      Assert.That (codeExpression, Is.InstanceOf<CodeMethodInvokeExpression>());
      var codeMethodInvokeExpression = (CodeMethodInvokeExpression) codeExpression;
      Assert.That (codeMethodInvokeExpression.Method, Is.Not.Null);
      Assert.That (codeMethodInvokeExpression.Method.MethodName, Is.EqualTo (nameof (ResourceExpressionBuilder.GetWebStringForResourceID)));
      Assert.That (codeMethodInvokeExpression.Method.TargetObject, Is.InstanceOf<CodeTypeReferenceExpression>());
      Assert.That (
          ((CodeTypeReferenceExpression) codeMethodInvokeExpression.Method.TargetObject).Type.BaseType,
          Is.EqualTo (typeof (ResourceExpressionBuilder).FullName));
      Assert.That (
          codeMethodInvokeExpression.Parameters.Count,
          Is.EqualTo (
              MemberInfoFromExpressionUtility.GetMethod (
                      () => ResourceExpressionBuilder.GetStringForResourceID (null, null)).GetParameters()
                  .Length));
      Assert.That (codeMethodInvokeExpression.Parameters[0], Is.InstanceOf<CodeThisReferenceExpression>());
      Assert.That (codeMethodInvokeExpression.Parameters[1], Is.InstanceOf<CodePrimitiveExpression>());
      Assert.That (((CodePrimitiveExpression) codeMethodInvokeExpression.Parameters[1]).Value, Is.EqualTo ("the expression"));
    }

    [Test]
    public void GetCodeExpression_ForPlainTextStringProperty ()
    {
      var resourceExpressionBuilder = new ResourceExpressionBuilder();
      var boundPropertyEntry = (BoundPropertyEntry) Activator.CreateInstance(typeof (BoundPropertyEntry), nonPublic:true);
      boundPropertyEntry.PropertyInfo = MemberInfoFromExpressionUtility.GetProperty (() => PlainTextStringProperty);
      object parsedData = "the expression";
      var expressionBuilderContext = new ExpressionBuilderContext("theControl.ascx");
      var codeExpression = resourceExpressionBuilder.GetCodeExpression (boundPropertyEntry, parsedData, expressionBuilderContext);

      Assert.That (codeExpression, Is.InstanceOf<CodeMethodInvokeExpression>());
      var codeMethodInvokeExpression = (CodeMethodInvokeExpression) codeExpression;
      Assert.That (codeMethodInvokeExpression.Method, Is.Not.Null);
      Assert.That (codeMethodInvokeExpression.Method.MethodName, Is.EqualTo (nameof (ResourceExpressionBuilder.GetPlainTextStringForResourceID)));
      Assert.That (codeMethodInvokeExpression.Method.TargetObject, Is.InstanceOf<CodeTypeReferenceExpression>());
      Assert.That (
          ((CodeTypeReferenceExpression) codeMethodInvokeExpression.Method.TargetObject).Type.BaseType,
          Is.EqualTo (typeof (ResourceExpressionBuilder).FullName));
      Assert.That (
          codeMethodInvokeExpression.Parameters.Count,
          Is.EqualTo (
              MemberInfoFromExpressionUtility.GetMethod (
                      () => ResourceExpressionBuilder.GetStringForResourceID (null, null)).GetParameters()
                  .Length));
      Assert.That (codeMethodInvokeExpression.Parameters[0], Is.InstanceOf<CodeThisReferenceExpression>());
      Assert.That (codeMethodInvokeExpression.Parameters[1], Is.InstanceOf<CodePrimitiveExpression>());
      Assert.That (((CodePrimitiveExpression) codeMethodInvokeExpression.Parameters[1]).Value, Is.EqualTo ("the expression"));
    }

    [Test]
    public void GetStringForResourceID_WithControlImplementingIObjectWithResources_ReturnsResourceEntry ()
    {
      var resourceValue = "resourceValue";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup (_ => _.TryGetString ("resourceID", out resourceValue)).Returns (true);

      var controlStub = new Mock<Control>();
      var controlWithIObjectWithResources = controlStub.As<IObjectWithResources>();
      controlWithIObjectWithResources.Setup (_ => _.GetResourceManager()).Returns (resourceManagerStub.Object);

      var result = ResourceExpressionBuilder.GetStringForResourceID (controlStub.Object, "resourceID");

      Assert.That (result, Is.EqualTo (resourceValue));
    }

    [Test]
    public void GetStringForResourceID_WithoutControlImplementingIObjectWithResources_ThrowsInvalidOperationException ()
    {
      Assert.That (
          () => ResourceExpressionBuilder.GetStringForResourceID (new Control(), "resourceID"),
          Throws.InvalidOperationException
              .With.Message.EqualTo (
                  "Remotion.Web.Compilation.ResourceExpressionBuilder can only be used on controls embedded within a parent implementing IObjectWithResources."));
    }

    [Test]
    public void GetWebStringForResourceID_WithControlImplementingIObjectWithResources_ReturnsResourceEntryAText ()
    {
      var resourceValue = "resourceValue";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup (_ => _.TryGetString ("resourceID", out resourceValue)).Returns (true);

      var controlStub = new Mock<Control>();
      var controlWithIObjectWithResources = controlStub.As<IObjectWithResources>();
      controlWithIObjectWithResources.Setup (_ => _.GetResourceManager()).Returns (resourceManagerStub.Object);

      var result = ResourceExpressionBuilder.GetWebStringForResourceID (controlStub.Object, "resourceID");

      Assert.That (result, Is.EqualTo (WebString.CreateFromText (resourceValue)));
    }

    [Test]
    public void GetWebStringForResourceID_WithControlImplementingIObjectWithResources_AndResourceIDIdentifiesHtml_ReturnsResourceEntryAsHtml ()
    {
      var resourceValue = "resourceValue";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup (_ => _.TryGetString ("resourceID", out resourceValue)).Returns (true);

      var controlStub = new Mock<Control>();
      var controlWithIObjectWithResources = controlStub.As<IObjectWithResources>();
      controlWithIObjectWithResources.Setup (_ => _.GetResourceManager()).Returns (resourceManagerStub.Object);

      var result = ResourceExpressionBuilder.GetWebStringForResourceID (controlStub.Object, "(html)resourceID");

      Assert.That (result, Is.EqualTo (WebString.CreateFromHtml (resourceValue)));
    }

    [Test]
    public void GetWebStringForResourceID_WithoutControlImplementingIObjectWithResources_ThrowsInvalidOperationException ()
    {
      Assert.That (
          () => ResourceExpressionBuilder.GetWebStringForResourceID (new Control(), "resourceID"),
          Throws.InvalidOperationException
              .With.Message.EqualTo (
                  "Remotion.Web.Compilation.ResourceExpressionBuilder can only be used on controls embedded within a parent implementing IObjectWithResources."));
    }

    [Test]
    public void GetPlainTextStringForResourceID_WithControlImplementingIObjectWithResources_ReturnsResourceEntry ()
    {
      var resourceValue = "resourceValue";
      var resourceManagerStub = new Mock<IResourceManager>();
      resourceManagerStub.Setup (_ => _.TryGetString ("resourceID", out resourceValue)).Returns (true);

      var controlStub = new Mock<Control>();
      var controlWithIObjectWithResources = controlStub.As<IObjectWithResources>();
      controlWithIObjectWithResources.Setup (_ => _.GetResourceManager()).Returns (resourceManagerStub.Object);

      var result = ResourceExpressionBuilder.GetPlainTextStringForResourceID (controlStub.Object, "resourceID");

      Assert.That (result, Is.EqualTo (PlainTextString.CreateFromText (resourceValue)));
    }

    [Test]
    public void GetPlainTextStringForResourceID_WithoutControlImplementingIObjectWithResources_ThrowsInvalidOperationException ()
    {
      Assert.That (
          () => ResourceExpressionBuilder.GetPlainTextStringForResourceID (new Control(), "resourceID"),
          Throws.InvalidOperationException
              .With.Message.EqualTo (
                  "Remotion.Web.Compilation.ResourceExpressionBuilder can only be used on controls embedded within a parent implementing IObjectWithResources."));
    }

    private static string StringProperty { get; set; }
    private static WebString WebStringProperty { get; set; }
    private static PlainTextString PlainTextStringProperty { get; set; }
  }
}