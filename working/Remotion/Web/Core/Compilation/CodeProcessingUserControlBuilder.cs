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
using System.Web.UI;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.Compilation
{
  /// <summary>
  /// Extends the <see cref="FileLevelUserControlBuilder"/> with the <see cref="IControlBuilderCodeProcessor"/> extension point.
  /// </summary>
  /// <remarks>
  /// Use the <see cref="FileLevelControlBuilderAttribute"/> to apply the extension point to a class derived from <see cref="UserControl"/>.
  /// </remarks>
  public class CodeProcessingUserControlBuilder : FileLevelUserControlBuilder 
  {
    public override void ProcessGeneratedCode (
        CodeCompileUnit codeCompileUnit,
        CodeTypeDeclaration baseType,
        CodeTypeDeclaration derivedType,
        CodeMemberMethod buildMethod,
        CodeMemberMethod dataBindingMethod)
    {
      ArgumentUtility.CheckNotNull ("codeCompileUnit", codeCompileUnit);
      ArgumentUtility.CheckNotNull ("baseType", baseType);
      ArgumentUtility.CheckNotNull ("derivedType", derivedType);
      ArgumentUtility.CheckNotNull ("buildMethod", buildMethod);

      var processor = SafeServiceLocator.Current.GetInstance<IControlBuilderCodeProcessor>();
      processor.ProcessGeneratedCode (
          codeCompileUnit,
          baseType,
          derivedType,
          buildMethod,
          dataBindingMethod,
          BaseProcessGeneratedCode);
    }

    private void BaseProcessGeneratedCode (
        CodeCompileUnit codeCompileUnit,
        CodeTypeDeclaration baseType,
        CodeTypeDeclaration derivedType,
        CodeMemberMethod buildMethod,
        CodeMemberMethod dataBindingMethod)
    {
      base.ProcessGeneratedCode (codeCompileUnit, baseType, derivedType, buildMethod, dataBindingMethod);
    }
  }
}