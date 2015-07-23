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
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schema;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schemas;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public class FileProcessor
  {
    private readonly LanguageProvider _languageProvider;
    private readonly string _functionBaseType;

    public FileProcessor (LanguageProvider languageProvider, string functionBaseType)
    {
      ArgumentUtility.CheckNotNull ("languageProvider", languageProvider);
      _languageProvider = languageProvider;
      _functionBaseType = functionBaseType;
    }

    public void ProcessFile (FileInfo file, char[] whitespace, CodeCompileUnit unit)
    {
      CommentLineContext commentLineContext = new CommentLineContext (null, null, false);
      List<string> importNamespaces = new List<string>(); // namespaces to import
      StringBuilder currentNamespaceBuilder = new StringBuilder();
      Action<string> currentNamespaceAppender = s =>
      {
        if (currentNamespaceBuilder.Length > 0)
          currentNamespaceBuilder.Append (".");
        currentNamespaceBuilder.Append (s);
      };

      StreamReader reader = new StreamReader (file.FullName, true);
      int lineNumber = 1;

      for (string line = reader.ReadLine(); line != null; line = reader.ReadLine(), lineNumber++)
      {
        string lineArgument;
        CodeLineType lineType = _languageProvider.ParseLine (line, out lineArgument);

        if (lineType == CodeLineType.NamespaceImport)
          ProcessNamespaceImportLine (importNamespaces, lineArgument);
        else if (lineType == CodeLineType.NamespaceDeclaration)
          ProcessNamespaceDeclarationLine (lineArgument, currentNamespaceAppender);
        else if (lineType == CodeLineType.LineComment)
        {
          commentLineContext = ProcessCommentLine (file, line, lineArgument, commentLineContext, whitespace, lineNumber);
          if (commentLineContext.IsXmlFragmentComplete)
            continue;
        }
        else if (lineType == CodeLineType.ClassDeclaration)
        {
          if (!commentLineContext.IsXmlFragmentComplete && commentLineContext.XmlFragmentContext != null)
            ProcessXmlFragment (commentLineContext.XmlFragmentContext, file);

          ProcessClassDeclarationLine (lineArgument, currentNamespaceBuilder.ToString(), commentLineContext.FunctionDeclaration);
          break;
        }
      }

      if (commentLineContext.IsXmlFragmentComplete && commentLineContext.FunctionDeclaration != null)
        GenerateClass (file, importNamespaces, commentLineContext.FunctionDeclaration, unit, commentLineContext.XmlFragmentContext.FirstLineNumber);
    }

    private void ProcessNamespaceImportLine (List<string> importNamespaces, string lineArgument)
    {
      if (!importNamespaces.Contains (lineArgument))
        importNamespaces.Add (lineArgument);
    }

    private void ProcessNamespaceDeclarationLine (string lineArgument, Action<string> currentNamespaceAppender)
    {
      currentNamespaceAppender (lineArgument);
    }

    private CommentLineContext ProcessCommentLine (
        FileInfo file, string line, string lineArgument, CommentLineContext commentLineContext, char[] whitespace, int lineNumber)
    {
      if (commentLineContext.XmlFragmentContext == null)
      {
        if (lineArgument.TrimStart (whitespace).StartsWith ("<" + FunctionDeclaration.ElementName))
          return new CommentLineContext (ProcessXmlFragmentOpeningTagLine (line, lineArgument, whitespace, lineNumber), null, false);

        return commentLineContext;
      }
      else
      {
        commentLineContext.XmlFragmentContext.XmlFragment.AppendLine();
        commentLineContext.XmlFragmentContext.XmlFragment.Append (lineArgument);
        commentLineContext.XmlFragmentContext.Indents.Add (line.IndexOf (lineArgument));
        if (lineArgument.TrimEnd (whitespace).EndsWith ("</" + FunctionDeclaration.ElementName + ">"))
          return ProcessXmlFragmentClosingTagLine (commentLineContext.XmlFragmentContext, file);
        else
          return commentLineContext;
      }
    }

    private XmlFragmentContext ProcessXmlFragmentOpeningTagLine (string line, string lineArgument, char[] whitespace, int lineNumber)
    {
      StringBuilder xmlFragment = new StringBuilder (1000);
      xmlFragment.AppendFormat ("<{0} xmlns=\"{1}\"", FunctionDeclaration.ElementName, FunctionDeclaration.SchemaUri);
      lineArgument = lineArgument.TrimStart (whitespace).Substring (FunctionDeclaration.ElementName.Length + 1);
      xmlFragment.Append (lineArgument);
      var indents = new List<int>();
      indents.Add (line.IndexOf (lineArgument));

      return new XmlFragmentContext (xmlFragment, lineNumber, indents);
    }

    private CommentLineContext ProcessXmlFragmentClosingTagLine (XmlFragmentContext xmlFragmentContext, FileInfo file)
    {
      FunctionDeclaration declaration = ProcessXmlFragment (xmlFragmentContext, file);
      if (declaration == null)
        return new CommentLineContext (xmlFragmentContext, null, true);

      if (string.IsNullOrEmpty (declaration.TemplateControlMarkupFile))
      {
        string cd = Environment.CurrentDirectory;
        string path = file.FullName;

        // TODO: geht nicht wenn parameter filemask ein ..\ o.ä. enthält
        Assertion.IsTrue (path.StartsWith (cd));
        path = path.Substring (cd.Length + 1);
        string ext = file.Extension;
        if (!string.IsNullOrEmpty (ext))
          path = path.Substring (0, path.Length - ext.Length);
        declaration.TemplateControlMarkupFile = path;
      }

      if (declaration.TemplateControlMode == TemplateMode.AutoDetect)
      {
        bool isPageFile = file.Name.EndsWith (".aspx" + file.Extension);
        bool isUserControlFile = file.Name.EndsWith (".ascx" + file.Extension);
        Assertion.IsTrue (isPageFile || isUserControlFile);
        declaration.TemplateControlMode = isPageFile ? TemplateMode.Page : TemplateMode.UserControl;
      }

      // replace built-in types
      foreach (VariableDeclaration var in declaration.ParametersAndVariables)
        var.TypeName = _languageProvider.ConvertTypeName (var.TypeName);

      if (string.IsNullOrEmpty (declaration.FunctionBaseType))
        declaration.FunctionBaseType = _functionBaseType;
      return new CommentLineContext (xmlFragmentContext, declaration, true);
    }

    private FunctionDeclaration ProcessXmlFragment (XmlFragmentContext xmlFragmentContext, FileInfo file)
    {
      // fragment complete, process it
      StringReader stringReader = new StringReader (xmlFragmentContext.XmlFragment.ToString());

      XmlSchemaSet schemas = new XmlSchemaSet();
      schemas.Add (FunctionDeclaration.SchemaUri, FunctionDeclaration.GetSchemaReader());

      XmlReaderSettings settings = new XmlReaderSettings();
      settings.Schemas = schemas;
      settings.ValidationType = ValidationType.Schema;
      settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

      var schemaValidationObject = new SchemaValidationObject (file, xmlFragmentContext.FirstLineNumber, xmlFragmentContext.Indents);
      settings.ValidationEventHandler += schemaValidationObject.CreateValidationHandler();

      XmlReader xmlReader = XmlReader.Create (stringReader, settings, file.FullName);
      XmlSerializer serializer = new XmlSerializer (typeof (FunctionDeclaration), FunctionDeclaration.SchemaUri);

      try
      {
        FunctionDeclaration declaration = (FunctionDeclaration) serializer.Deserialize (xmlReader);

        if (schemaValidationObject.HasFailed)
          return null;
        else
          return declaration;
      }
      catch (InvalidOperationException e)
      {
        XmlException xmlException = e.InnerException as XmlException;
        if (xmlException != null)
        {
          throw new InputException (
              InputError.XmlError,
              file.FullName,
              xmlException.LineNumber + xmlFragmentContext.FirstLineNumber - 1,
              xmlException.LinePosition + xmlFragmentContext.Indents[xmlException.LineNumber - 1],
              xmlException);
        }
        else
          throw;
      }
    }

    private void ProcessClassDeclarationLine (string lineArgument, string currentNamespace, FunctionDeclaration declaration)
    {
      if (declaration != null && string.IsNullOrEmpty (declaration.TemplateControlCodeBehindType))
      {
        string type = lineArgument;
        if (currentNamespace.Length > 0)
          type = currentNamespace + "." + type;
        declaration.TemplateControlCodeBehindType = type;
      }
    }

    private void GenerateClass (
        FileInfo file, List<string> importNamespaces, FunctionDeclaration declaration, CodeCompileUnit unit, int firstLineNumber)
    {
      TemplateControlFunctionGeneratorBase generator;
      switch (declaration.TemplateControlMode)
      {
        case TemplateMode.Page:
          generator = new PageFunctionGenerator (unit, declaration, importNamespaces, file, firstLineNumber);
          break;
        case TemplateMode.UserControl:
          generator = new UserControlFunctionGenerator (unit, declaration, importNamespaces, file, firstLineNumber);
          break;
        default:
          throw new InvalidOperationException (
              string.Format ("Value '{0}' is not supported for generating the function and page classes.", declaration.TemplateControlMode));
      }
      generator.GenerateClass();
    }
  }
}
