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
using Remotion.Xml;

namespace Remotion.Data.DomainObjects.Schemas
{
  public sealed class TypesSchemaLoader : SchemaLoaderBase
  {
    // types

    // static members and constants

    public static readonly TypesSchemaLoader Instance = new TypesSchemaLoader();

    // member fields

    private readonly string _schemaFile = "Types.xsd";
    private readonly string _schemaUri = "http://www.re-motion.org/Data/DomainObjects/Types";

    // construction and disposing

    private TypesSchemaLoader ()
    {
    }

    // methods and properties

    public override string SchemaUri
    {
      get { return _schemaUri; }
    }

    protected override string SchemaFile
    {
      get { return _schemaFile; }
    }
  }
}
