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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schema;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schemas;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public class UserControlFunctionGenerator : TemplateControlFunctionGeneratorBase
  {
    public UserControlFunctionGenerator (CodeCompileUnit codeCompileUnit, FunctionDeclaration functionDeclaration, List<string> importNamespaces, FileInfo file, int line)
        : base(codeCompileUnit, functionDeclaration, importNamespaces, file, line)
    {
    }

    protected override void GenerateWxeFunctionTemplateControlStep (CodeTypeDeclaration functionClass, FunctionDeclaration templateControlFile)
    {
      ArgumentUtility.CheckNotNull ("functionClass", functionClass);
      ArgumentUtility.CheckNotNull ("templateControlFile", templateControlFile);

      CodeMemberField step1 = new CodeMemberField (typeof (WxeUserControlStep), "Step1");
      functionClass.Members.Add (step1);
      step1.InitExpression = new CodeObjectCreateExpression (
          new CodeTypeReference (typeof (WxeUserControlStep)),
          new CodePrimitiveExpression (templateControlFile.TemplateControlMarkupFile));
    }

    /// <summary>
    /// &lt;returnType&gt; Call (IWxePage page, WxeUserControl userControl, Control sender, &lt;type&gt; [ref|out] param1, &lt;type&gt; [ref|out] param2, ...)
    /// </summary>
    protected override CodeMemberMethod GenerateWxeTemplateControlCallMethod (FunctionDeclaration functionDeclaration, CodeTypeDeclaration partialTemplateControlClass, CodeTypeDeclaration functionClass)
    {
      CodeMemberMethod callMethod = new CodeMemberMethod();
      partialTemplateControlClass.Members.Add (callMethod);
      callMethod.Name = "Call";
      callMethod.Attributes = MemberAttributes.Static | MemberAttributes.Public;
      callMethod.Parameters.Add (
          new CodeParameterDeclarationExpression (
              new CodeTypeReference (typeof (IWxePage)), "currentPage"));
      callMethod.Parameters.Add (
          new CodeParameterDeclarationExpression (
              new CodeTypeReference (typeof (WxeUserControl)), "currentUserControl"));
      callMethod.Parameters.Add (
          new CodeParameterDeclarationExpression (
              new CodeTypeReference (typeof (Control)), "sender"));
      foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.IsReturnValue)
          callMethod.ReturnType = new CodeTypeReference (parameterDeclaration.TypeName);
        else
        {
          CodeParameterDeclarationExpression parameter = new CodeParameterDeclarationExpression (
              new CodeTypeReference (parameterDeclaration.TypeName),
              parameterDeclaration.Name);
          callMethod.Parameters.Add (parameter);
          if (parameterDeclaration.Direction == WxeParameterDirection.InOut)
            parameter.Direction = FieldDirection.Ref;
          else if (parameterDeclaration.Direction == WxeParameterDirection.Out)
            parameter.Direction = FieldDirection.Out;
        }
      }

      //  ArgumentUtility.CheckNotNull ("page", page);
      //  ArgumentUtility.CheckNotNull ("userControl", userControl);
      //  ArgumentUtility.CheckNotNull ("sender", sender);

      // <class>Function function;
      CodeVariableDeclarationStatement functionVariable = new CodeVariableDeclarationStatement (
          new CodeTypeReference (functionClass.Name), "function");
      callMethod.Statements.Add (functionVariable);
      // common variables
      CodeArgumentReferenceExpression currentPage = new CodeArgumentReferenceExpression ("currentPage");
      CodeArgumentReferenceExpression currentUserControl = new CodeArgumentReferenceExpression ("currentUserControl");
      CodeVariableReferenceExpression function = new CodeVariableReferenceExpression ("function");
      CodePropertyReferenceExpression exceptionHandler = new CodePropertyReferenceExpression (function, "ExceptionHandler");
      // if (! currentPage.IsReturningPostBack)
      CodeConditionStatement ifNotIsReturningPostBack = new CodeConditionStatement();
      callMethod.Statements.Add (ifNotIsReturningPostBack);
      ifNotIsReturningPostBack.Condition = new CodeBinaryOperatorExpression (
          new CodePropertyReferenceExpression (currentPage, "IsReturningPostBack"),
          CodeBinaryOperatorType.ValueEquality,
          new CodePrimitiveExpression (false));
      // { 
      //   function = new <class>Function(inarg1, inarg2, ...);
      CodeObjectCreateExpression functionInitialization = new CodeObjectCreateExpression (new CodeTypeReference (functionClass.Name));
      foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.Direction != WxeParameterDirection.Out)
          functionInitialization.Parameters.Add (new CodeArgumentReferenceExpression (parameterDeclaration.Name));
      }
      ifNotIsReturningPostBack.TrueStatements.Add (new CodeAssignStatement (function, functionInitialization));
      //   function.ExceptionHandler.SetCatchExceptionTypes (typeof (Exception));
      ifNotIsReturningPostBack.TrueStatements.Add (
          new CodeMethodInvokeExpression (
              exceptionHandler,
              "SetCatchExceptionTypes",
              new CodeTypeOfExpression (typeof (Exception))));

      //    WxeUserControl actualUserControl;
      CodeVariableDeclarationStatement actualUserControlVariable = new CodeVariableDeclarationStatement (
          new CodeTypeReference (typeof (WxeUserControl)), "actualUserControl");
      ifNotIsReturningPostBack.TrueStatements.Add (actualUserControlVariable);
      CodeVariableReferenceExpression actualUserControl = new CodeVariableReferenceExpression (actualUserControlVariable.Name);

      //    actualUserControl = (WxeUserControl) page.FindControl (userControl.PermanentUniqueID);
      ifNotIsReturningPostBack.TrueStatements.Add (
          new CodeAssignStatement (
              actualUserControl,
              new CodeCastExpression (
                  typeof (WxeUserControl),
                  new CodeMethodInvokeExpression (
                      currentPage,
                      "FindControl",
                      new CodePropertyReferenceExpression (currentUserControl, "PermanentUniqueID")))));

      //    Assertion.IsNotNull (actualUserControl);

      //  actualUserControl.ExecuteFunction (function, sender, null);
      ifNotIsReturningPostBack.TrueStatements.Add (
          new CodeMethodInvokeExpression (
              actualUserControl,
              "ExecuteFunction",
              function,
              new CodeVariableReferenceExpression ("sender"),
              new CodePrimitiveExpression (null)));

      //    throw new Exception ("(Unreachable code)"); 
      ifNotIsReturningPostBack.TrueStatements.Add (
          new CodeThrowExceptionStatement (
              new CodeObjectCreateExpression (
                  new CodeTypeReference (typeof (Exception)),
                  new CodeExpression[]
                  {
                      new CodePrimitiveExpression ("(Unreachable code)")
                  })));
      // } else {
      //   function = (<class>Function) currentPage.ReturningFunction;
      ifNotIsReturningPostBack.FalseStatements.Add (
          new CodeAssignStatement (
              function,
              new CodeCastExpression (
                  new CodeTypeReference (functionClass.Name),
                  new CodePropertyReferenceExpression (currentPage, "ReturningFunction"))));
      //    if (function.Exception != null)
      CodeConditionStatement ifException = new CodeConditionStatement();
      ifNotIsReturningPostBack.FalseStatements.Add (ifException);
      ifException.Condition = new CodeBinaryOperatorExpression (
          new CodePropertyReferenceExpression (
              exceptionHandler,
              "Exception"),
          CodeBinaryOperatorType.IdentityInequality,
          new CodePrimitiveExpression (null));
      //      throw function.Exception;
      ifException.TrueStatements.Add (
          new CodeThrowExceptionStatement (
              new CodePropertyReferenceExpression (
                  exceptionHandler,
                  "Exception")
              ));
      //   ParamN = function.ParamN;
      foreach (ParameterDeclaration parameterDeclaration in functionDeclaration.Parameters)
      {
        if (parameterDeclaration.Direction != WxeParameterDirection.In && !parameterDeclaration.IsReturnValue)
        {
          ifNotIsReturningPostBack.FalseStatements.Add (
              new CodeAssignStatement (
                  new CodeArgumentReferenceExpression (parameterDeclaration.Name),
                  new CodePropertyReferenceExpression (function, parameterDeclaration.Name)));
        }
        else if (parameterDeclaration.IsReturnValue)
        {
          // TODO: Throw Exception if any but last parameter has return value flag!
          ifNotIsReturningPostBack.FalseStatements.Add (
              new CodeMethodReturnStatement (
                  new CodePropertyReferenceExpression (function, parameterDeclaration.Name)));
        }
      }
      return callMethod;
    }

    protected override void GenerateWxeTemplateControlCallMethodOverloadWithoutCallArguments (CodeTypeDeclaration partialTemplateControlClass, CodeMemberMethod callMethod)
    {
      //NOP
    }
  }
}
