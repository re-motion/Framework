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
using System.Collections.Generic;
using System.IO;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schema;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schemas;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public abstract class TemplateControlFunctionGeneratorBase
  {
    private readonly CodeCompileUnit _codeCompileUnit;
    private readonly FunctionDeclaration _functionDeclaration;
    private readonly List<string> _importNamespaces;
    private readonly FileInfo _file;
    private readonly int _line;

    protected TemplateControlFunctionGeneratorBase (
        CodeCompileUnit codeCompileUnit, FunctionDeclaration functionDeclaration, List<string> importNamespaces, FileInfo file, int line)
    {
      ArgumentUtility.CheckNotNull ("codeCompileUnit", codeCompileUnit);
      ArgumentUtility.CheckNotNull ("functionDeclaration", functionDeclaration);
      ArgumentUtility.CheckNotNull ("importNamespaces", importNamespaces);
      ArgumentUtility.CheckNotNull ("file", file);

      _codeCompileUnit = codeCompileUnit;
      _line = line;
      _file = file;
      _importNamespaces = importNamespaces;
      _functionDeclaration = functionDeclaration;
    }

    // generate output classes for [WxePageFunction] page class
    public void GenerateClass ()
    {
      if (string.IsNullOrEmpty (_functionDeclaration.TemplateControlCodeBehindType))
        throw new InputException (
            InputError.ClassNotFound,
            _file.FullName,
            _line,
            1,
            new ApplicationException ("Could not detect class declaration."));

      string nameSpace;
      string typeName;
      SplitTypeName (_functionDeclaration.TemplateControlCodeBehindType, out nameSpace, out typeName);

      string functionName = _functionDeclaration.FunctionName;
      if (functionName == null)
        functionName = typeName + "Function";

      CodeNamespace ns = new CodeNamespace (nameSpace);
      _codeCompileUnit.Namespaces.Add (ns);

      foreach (string importNamespace in _importNamespaces)
        ns.Imports.Add (new CodeNamespaceImport (importNamespace));
      //ns.Imports.Add (new CodeNamespaceImport ("System"));
      //ns.Imports.Add (new CodeNamespaceImport ("Remotion.Web.ExecutionEngine"));

      // generate a partial class for the page that allows access to parameters and
      // local variables from page code
      CodeTypeDeclaration partialTemplateControlClass = GenerateWxeTemplateControlClassDecalation (typeName, ns);

      // add Return() method as alias for ExecuteNextStep()
      GenerateWxeTemplateControlReturnMethod (partialTemplateControlClass);

      //// add Return (outPar1, outPar2, ...) method 
      //// -- removed (unneccessary, possibly confusing)
      //GenerateWxePageReturnMethodWithOutParameters();

      // generate a WxeFunction class
      CodeTypeDeclaration functionClass = GenerateWxeFunctionClassDeclaration (functionName, ns);

      // generate a strongly typed CurrentFunction property for the page class
      GenerateWxeTemplateControlCurrentFunctionProperty (partialTemplateControlClass, functionClass);

      GenerateWxeFunctionVariablesAndWxePageVariables (partialTemplateControlClass, functionClass);

      // add PageStep to WXE function
      GenerateWxeFunctionTemplateControlStep (functionClass, _functionDeclaration);

      // add constructors to WXE function

      // ctor () : base () {}
      GenerateWxeFunctionDefaultConstructor (functionClass);

      // ctor (params object[] args): base (args) {}
      // GenerateWxeFunctionConstructorWithParamsArray();

      // ctor (<type1> inarg1, <type2> inarg2, ...): base (inarg1, inarg2, ...) {}
      GenerateWxeFunctionContructorWithTypesParameters (functionClass);

      // <returnType> Call (IWxePage page, WxeExecuteFunctionOptions options, <type> [ref|out] param1, <type> [ref|out] param2, ...)
      CodeMemberMethod callMethod = GenerateWxeTemplateControlCallMethod (_functionDeclaration, partialTemplateControlClass, functionClass);

      // <returnType> Call (IWxePage page, <type> [ref|out] param1, <type> [ref|out] param2, ...)
      GenerateWxeTemplateControlCallMethodOverloadWithoutCallArguments (partialTemplateControlClass, callMethod);
    }

    public FunctionDeclaration FunctionDeclaration
    {
      get { return _functionDeclaration; }
    }

    private void SplitTypeName (string fullTypeName, out string nameSpace, out string typeName)
    {
      int pos = fullTypeName.LastIndexOf ('.');
      if (pos < 0)
      {
        nameSpace = null;
        typeName = fullTypeName;
      }
      else
      {
        nameSpace = fullTypeName.Substring (0, pos);
        typeName = fullTypeName.Substring (pos + 1);
      }
    }

    /// <summary>
    /// generate a partial class for the page that allows access to parameters and local variables from page code
    /// </summary>
    private CodeTypeDeclaration GenerateWxeTemplateControlClassDecalation (string typeName, CodeNamespace ns)
    {
      CodeTypeDeclaration partialTemplateControlClass = new CodeTypeDeclaration (typeName);
      ns.Types.Add (partialTemplateControlClass);
      partialTemplateControlClass.IsPartial = true;
      partialTemplateControlClass.Attributes = MemberAttributes.Public;
      return partialTemplateControlClass;
    }

    /// <summary>
    /// add Return() method as alias for ExecuteNextStep()
    /// </summary>
    private void GenerateWxeTemplateControlReturnMethod (CodeTypeDeclaration partialTemplateControlClass)
    {
      CodeMemberMethod returnMethod = new CodeMemberMethod();
      partialTemplateControlClass.Members.Add (returnMethod);
      returnMethod.Name = "Return";
      returnMethod.Attributes = MemberAttributes.Family | MemberAttributes.Final;
      CodeExpression executeNextStep = new CodeMethodInvokeExpression (
          new CodeThisReferenceExpression(),
          "ExecuteNextStep",
          new CodeExpression[0]);
      returnMethod.Statements.Add (executeNextStep);
    }

    /// <summary>
    /// add Return (outPar1, outPar2, ...) method 
    /// </summary>
    [Obsolete ("removed (unneccessary, possibly confusing)", true)]
    private void GenerateWxePageReturnMethodWithOutParameters ()
    {
      //CodeMemberMethod returnParametersMethod = new CodeMemberMethod ();
      //foreach (WxePageParameterAttribute parameterDeclaration in GetPageParameterAttributesOrdered (type))
      //{
      //  if (parameterDeclaration.Direction != WxeParameterDirection.In)
      //  {
      //    returnParametersMethod.Parameters.Add (new CodeParameterDeclarationExpression (
      //        new CodeTypeReference (parameterDeclaration.Type),
      //        parameterDeclaration.Name));
      //    returnParametersMethod.Statements.Add (new CodeAssignStatement (
      //        new CodePropertyReferenceExpression (new CodeThisReferenceExpression (), parameterDeclaration.Name),
      //        new CodeArgumentReferenceExpression (parameterDeclaration.Name)));
      //  }
      //}
      //if (returnParametersMethod.Parameters.Count > 0)
      //{
      //  partialPageClass.Members.Add (returnParametersMethod);
      //  returnParametersMethod.Name = "Return";
      //  returnParametersMethod.Attributes = MemberAttributes.Family | MemberAttributes.Final;
      //  returnParametersMethod.Statements.Add (executeNextStep);
      //}
    }

    /// <summary>
    /// &lt;returnType&gt; Call (IWxePage page, IWxeCallArguments arguments, &lt;type&gt; [ref|out] param1, &lt;type&gt; [ref|out] param2, ...)
    /// </summary>
    protected abstract CodeMemberMethod GenerateWxeTemplateControlCallMethod (FunctionDeclaration functionDeclaration, CodeTypeDeclaration partialTemplateControlClass, CodeTypeDeclaration functionClass);

    /// <summary>
    /// &lt;returnType&gt; Call (IWxePage page, &lt;type&gt; [ref|out] param1, &lt;type&gt; [ref|out] param2, ...)
    /// </summary>
    protected abstract void GenerateWxeTemplateControlCallMethodOverloadWithoutCallArguments (CodeTypeDeclaration partialTemplateControlClass, CodeMemberMethod callMethod);

    private CodeTypeDeclaration GenerateWxeFunctionClassDeclaration (string functionName, CodeNamespace ns)
    {
      CodeTypeDeclaration functionClass = new CodeTypeDeclaration (functionName);
      ns.Types.Add (functionClass);
      functionClass.Attributes = MemberAttributes.Public;
      functionClass.BaseTypes.Add (new CodeTypeReference (_functionDeclaration.FunctionBaseType));
      functionClass.CustomAttributes.Add (new CodeAttributeDeclaration (new CodeTypeReference (typeof (SerializableAttribute))));
      return functionClass;
    }

    private void GenerateWxeTemplateControlCurrentFunctionProperty (CodeTypeDeclaration partialTemplateControlClass, CodeTypeDeclaration functionClass)
    {
      CodeMemberProperty currentFunctionProperty = new CodeMemberProperty();
      currentFunctionProperty.Name = "CurrentFunction";
      currentFunctionProperty.Type = new CodeTypeReference (functionClass.Name);
      currentFunctionProperty.Attributes = MemberAttributes.New | MemberAttributes.Family | MemberAttributes.Final;
      currentFunctionProperty.GetStatements.Add (
          new CodeMethodReturnStatement (
              new CodeCastExpression (
                  new CodeTypeReference (functionClass.Name),
                  new CodePropertyReferenceExpression (
                      new CodeBaseReferenceExpression(),
                      "CurrentFunction"))));
      partialTemplateControlClass.Members.Add (currentFunctionProperty);
    }

    protected abstract void GenerateWxeFunctionTemplateControlStep (CodeTypeDeclaration functionClass, FunctionDeclaration templateControlFile);

    private void GenerateWxeFunctionVariablesAndWxePageVariables (CodeTypeDeclaration partialPageClass, CodeTypeDeclaration functionClass)
    {
      int parameterIndex = 0;

      // generate local variables in partial/variables class, and
      // generate function parameters in partial/variables class and function class
      foreach (VariableDeclaration variableDeclaration in _functionDeclaration.ParametersAndVariables)
      {
        CodeMemberProperty localProperty = GenerateWxeTemplateControlVariableDeclaration (variableDeclaration, partialPageClass);

        ParameterDeclaration parameterDeclaration = variableDeclaration as ParameterDeclaration;
        CodeMemberProperty functionProperty = null;
        if (parameterDeclaration != null)
        {
          functionProperty = GenerateWxeFunctionParameterDeclaration (parameterDeclaration, functionClass);
          // add attribute [WxeParameter (parameterIndex, [Required,] Direction)
          GenerateWxeFunctionDecoratePropertyWithWxeParameterAttribute (parameterDeclaration, functionProperty, parameterIndex);
        }

        // <variable> := Variables["<parameterName>"]
        CodeStatement getStatement;
        CodeStatement setStatement;
        GenerateWxeFunctionAndWxeTemplateControlPropertyAccessors (variableDeclaration, out getStatement, out setStatement);

        if (parameterDeclaration != null)
        {
          // add get/set accessors according to parameter direction
          if (parameterDeclaration.Direction != WxeParameterDirection.Out)
          {
            // In or InOut: get from page, set from function
            localProperty.GetStatements.Add (getStatement);
            functionProperty.SetStatements.Add (setStatement);
          }

          if (parameterDeclaration.Direction != WxeParameterDirection.In)
          {
            // Out or InOut: get from function
            functionProperty.GetStatements.Add (getStatement);
          }

          // all directions: set from page
          localProperty.SetStatements.Add (setStatement);
        }
        else
        {
          // variables always have get and set, and are only added to the local variable collection
          localProperty.GetStatements.Add (getStatement);
          localProperty.SetStatements.Add (setStatement);
        }

        if (parameterDeclaration != null)
          ++ parameterIndex;
      }
    }

    private CodeMemberProperty GenerateWxeFunctionParameterDeclaration (ParameterDeclaration parameterDeclaration, CodeTypeDeclaration functionClass)
    {
      CodeMemberProperty functionProperty;
      functionProperty = new CodeMemberProperty();
      functionClass.Members.Add (functionProperty);
      functionProperty.Name = parameterDeclaration.Name;
      functionProperty.Type = new CodeTypeReference (parameterDeclaration.TypeName);
      functionProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
      return functionProperty;
    }

    private CodeMemberProperty GenerateWxeTemplateControlVariableDeclaration (VariableDeclaration variableDeclaration, CodeTypeDeclaration partialTemplateControlClass)
    {
      CodeMemberProperty localProperty = new CodeMemberProperty();
      localProperty.Name = variableDeclaration.Name;
      localProperty.Type = new CodeTypeReference (variableDeclaration.TypeName);
      // localProperty.Type = new LiteralTypeReference (variableDeclaration.TypeName);

      partialTemplateControlClass.Members.Add (localProperty);
      localProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
      return localProperty;
      // TODO: can get-accessor alone be set to protected via CodeDOM?
    }

    /// <summary>
    /// &lt;variable&gt; := Variables["&lt;parameterName&gt;"]
    /// </summary>
    private void GenerateWxeFunctionAndWxeTemplateControlPropertyAccessors (
        VariableDeclaration variableDeclaration, out CodeStatement getStatement, out CodeStatement setStatement)
    {
      CodeExpression variable = new CodeIndexerExpression (
          new CodePropertyReferenceExpression (new CodeThisReferenceExpression(), "Variables"),
          new CodePrimitiveExpression (variableDeclaration.Name));

      // <getStatement> := get { return (<type>) <variable>; }
      getStatement = new CodeMethodReturnStatement (new CodeCastExpression (new CodeTypeReference (variableDeclaration.TypeName), variable));

      // <setStatement> := set { <variable> = value; }
      setStatement = new CodeAssignStatement (variable, new CodePropertySetValueReferenceExpression());
    }

    /// <summary>
    /// add attribute [WxeParameter (parameterIndex, [Required,] Direction)
    /// </summary>
    private void GenerateWxeFunctionDecoratePropertyWithWxeParameterAttribute (
        ParameterDeclaration parameterDeclaration, CodeMemberProperty functionProperty, int parameterIndex)
    {
      CodeAttributeDeclaration wxeParameterAttribute = new CodeAttributeDeclaration (new CodeTypeReference (typeof (WxeParameterAttribute)));
      functionProperty.CustomAttributes.Add (wxeParameterAttribute);
      wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (new CodePrimitiveExpression (parameterIndex)));
      if (parameterDeclaration.IsRequired.HasValue)
        wxeParameterAttribute.Arguments.Add (new CodeAttributeArgument (new CodePrimitiveExpression (parameterDeclaration.IsRequired.Value)));
      wxeParameterAttribute.Arguments.Add (
          new CodeAttributeArgument (
              new CodeFieldReferenceExpression (
                  new CodeTypeReferenceExpression (typeof (WxeParameterDirection)),
                  parameterDeclaration.Direction.ToString())));
    }

    /// <summary>
    /// ctor () : base (new object[]{}) {}
    /// <para>- or -</para>
    /// ctor () : base (new NoneTransactionMode(), new object[]{}) {}
    /// </summary>
    private void GenerateWxeFunctionDefaultConstructor (CodeTypeDeclaration functionClass)
    {
      CodeConstructor defaultCtor = new CodeConstructor();
      functionClass.Members.Add (defaultCtor);
      defaultCtor.Attributes = MemberAttributes.Public;
      if (IsWxeFunctionType (_functionDeclaration.FunctionBaseType))
        defaultCtor.BaseConstructorArgs.Add (new CodeObjectCreateExpression (new CodeTypeReference (typeof (NoneTransactionMode))));
      defaultCtor.BaseConstructorArgs.Add (new CodeArrayCreateExpression (new CodeTypeReference (typeof (object[])), 0));
    }

    /// <summary>
    /// ctor (params object[] args): base (args) {}
    /// </summary>
    [Obsolete ("replace by (VarRef<type1> arg1, VarRef<type2> arg2, ...)", true)]
    private void GenerateWxeFunctionConstructorWithParamsArray ()
    {
      // replace by (VarRef<type1> arg1, VarRef<type2> arg2, ...)
      //CodeConstructor untypedCtor = new CodeConstructor ();
      //functionClass.Members.Add (untypedCtor);
      //untypedCtor.Attributes = MemberAttributes.Public;
      //CodeParameterDeclarationExpression untypedParameters = new CodeParameterDeclarationExpression (
      //    new CodeTypeReference (typeof (object[])),
      //    "args");
      //untypedParameters.CustomAttributes.Add (new CodeAttributeDeclaration ("System.ParamArrayAttribute"));
      //untypedCtor.Parameters.Add (untypedParameters);
      //untypedCtor.BaseConstructorArgs.Add (new CodeArgumentReferenceExpression ("args"));
    }

    /// <summary>
    /// ctor (&lt;type1&gt; inarg1, &lt;type2&gt; inarg2, ...): base (new object[] {inarg1, inarg2, ...}) {}
    /// <para>- or -</para>
    /// ctor (&lt;type1&gt; inarg1, &lt;type2&gt; inarg2, ...): base (new NoneTransactionMode(), new object[] {inarg1, inarg2, ...}) {}
    /// </summary>
    private void GenerateWxeFunctionContructorWithTypesParameters (CodeTypeDeclaration functionClass)
    {
      CodeConstructor typedCtor = new CodeConstructor();
      typedCtor.Attributes = MemberAttributes.Public;
      if (IsWxeFunctionType(_functionDeclaration.FunctionBaseType))
        typedCtor.BaseConstructorArgs.Add (new CodeObjectCreateExpression (new CodeTypeReference (typeof (NoneTransactionMode))));
      List<CodeExpression> parameters = new List<CodeExpression>();
      foreach (ParameterDeclaration parameterDeclaration in _functionDeclaration.Parameters)
      {
        if (parameterDeclaration.Direction != WxeParameterDirection.Out)
        {
          typedCtor.Parameters.Add (
              new CodeParameterDeclarationExpression (new CodeTypeReference (parameterDeclaration.TypeName), parameterDeclaration.Name));

          parameters.Add (new CodeArgumentReferenceExpression (parameterDeclaration.Name));
        }
      }
      typedCtor.BaseConstructorArgs.Add (new CodeArrayCreateExpression (new CodeTypeReference (typeof (object[])), parameters.ToArray()));
      if (typedCtor.Parameters.Count > 0)
        functionClass.Members.Add (typedCtor);
    }

    private bool IsWxeFunctionType (string functionBaseType)
    {
      return functionBaseType == typeof (WxeFunction).Name || functionBaseType == typeof (WxeFunction).FullName;
    }
  }
}
