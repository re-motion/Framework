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
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.Globalization;
using Remotion.Web.UI.Globalization;

namespace Remotion.Web.Compilation
{
  //TODO: Check if this is realy the optimal solution
  [ExpressionPrefix("res")]
  public class ResourceExpressionBuilder : ExpressionBuilder
  {
    // constants

    // types

    // static members

    private static readonly WebStringConverter s_webStringConverter = new();

    public static string GetStringForResourceID (Control parent, string resourceID)
    {
      ArgumentUtility.CheckNotNull("parent", parent);
      ArgumentUtility.CheckNotNullOrEmpty("resourceID", resourceID);

      IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager(parent, true);

      return resourceManager.GetString(resourceID);
    }

    public static WebString GetWebStringForResourceID (Control parent, string resourceID)
    {
      ArgumentUtility.CheckNotNull("parent", parent);
      ArgumentUtility.CheckNotNullOrEmpty("resourceID", resourceID);

      var resourceIDAsWebString = (WebString?)s_webStringConverter.ConvertFromString(resourceID) ?? WebString.Empty;

      IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager(parent, true);

      return resourceManager.GetWebString(resourceIDAsWebString.GetValue(), resourceIDAsWebString.Type);
    }

    public static PlainTextString GetPlainTextStringForResourceID (Control parent, string resourceID)
    {
      ArgumentUtility.CheckNotNull("parent", parent);
      ArgumentUtility.CheckNotNullOrEmpty("resourceID", resourceID);

      IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager(parent, true);

      return resourceManager.GetText(resourceID);
    }

    // member fields

    // construction and disposing

    public ResourceExpressionBuilder ()
    {
    }

    // methods and properties

    public override CodeExpression GetCodeExpression (BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
    {
      CodeMethodInvokeExpression expression = new CodeMethodInvokeExpression();
      expression.Method.TargetObject = new CodeTypeReferenceExpression(GetType());
      expression.Method.MethodName = entry.PropertyInfo.PropertyType switch
      {
          { } type when type == typeof(WebString) => nameof(GetWebStringForResourceID),
          { } type when type == typeof(PlainTextString) => nameof(GetPlainTextStringForResourceID),
          _ => nameof(GetStringForResourceID),
      };
      expression.Parameters.Add(new CodeThisReferenceExpression());
      expression.Parameters.Add(new CodePrimitiveExpression((string)parsedData));

      return expression;
    }

    public override object ParseExpression (string expression, Type propertyType, ExpressionBuilderContext context)
    {
      return expression;
    }
  }
}
