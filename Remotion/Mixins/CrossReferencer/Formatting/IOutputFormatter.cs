// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Reflection;
using System.Xml.Linq;

namespace MixinXRef.Formatting
{
  public interface IOutputFormatter
  {
    string GetShortFormattedTypeName (Type type);
    string GetFullFormattedTypeName (Type type);
    string GetConstructorName (Type type);

    XElement CreateModifierMarkup (string attributes, string keywords);

    XElement CreateConstructorMarkup (string name, ParameterInfo[] parameterInfos);
    XElement CreateMethodMarkup (string methodName, Type returnType, ParameterInfo[] parameterInfos, Type[] genericArguments = null);
    XElement CreateEventMarkup (string eventName, Type handlerType);
    XElement CreateFieldMarkup (string fieldName, Type fieldType);
    XElement CreatePropertyMarkup (string propertyName, Type propertyType);
    XElement CreateNestedTypeMarkup (Type nestedType);
  }
}