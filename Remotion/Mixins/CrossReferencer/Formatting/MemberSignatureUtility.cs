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
using System.Reflection;
using System.Xml.Linq;
using Remotion.Utilities;

namespace Remotion.Mixins.CrossReferencer.Formatting
{
  public class MemberSignatureUtility
  {
    private readonly IOutputFormatter _outputFormatter;

    public MemberSignatureUtility (IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull("outputFormatter", outputFormatter);

      _outputFormatter = outputFormatter;
    }

    public XElement? GetMemberSignature (MemberInfo memberInfo)
    {
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      switch (memberInfo.MemberType)
      {
        case MemberTypes.Method:
          var methodInfo = (MethodInfo)memberInfo;
          return _outputFormatter.CreateMethodMarkup(methodInfo.Name, methodInfo.ReturnType, methodInfo.GetParameters(), methodInfo.GetGenericArguments());

        case MemberTypes.Constructor:
          var constructorInfo = (ConstructorInfo)memberInfo;
          var constructorInfoDeclaringType = Assertion.IsNotNull(memberInfo.DeclaringType, "memberInfo.DeclaringType != null");
          return _outputFormatter.CreateConstructorMarkup(_outputFormatter.GetConstructorName(constructorInfoDeclaringType), constructorInfo.GetParameters());

        case MemberTypes.Event:
          var eventInfo = (EventInfo)memberInfo;
          var eventInfoEventHandlerType = Assertion.IsNotNull(eventInfo.EventHandlerType, "eventInfo.EventHandlerType != null");
          return _outputFormatter.CreateEventMarkup(eventInfo.Name, eventInfoEventHandlerType);

        case MemberTypes.Field:
          var fieldInfo = (FieldInfo)memberInfo;
          return _outputFormatter.CreateFieldMarkup(fieldInfo.Name, fieldInfo.FieldType);

        case MemberTypes.Property:
          var propertyInfo = (PropertyInfo)memberInfo;
          return _outputFormatter.CreatePropertyMarkup(propertyInfo.Name, propertyInfo.PropertyType);

        case MemberTypes.NestedType:
          var nestedType = (Type)memberInfo;
          return _outputFormatter.CreateNestedTypeMarkup(nestedType);

        case MemberTypes.Custom:
        case MemberTypes.TypeInfo:
          return null;

        default:
          throw new Exception("unknown member type");
      }
    }
  }
}
