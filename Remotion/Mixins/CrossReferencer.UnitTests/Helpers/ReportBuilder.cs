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
using System.Linq;
using System.Reflection;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Report;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Helpers
{
  internal static class ReportBuilder
  {
    public static AssemblyReportGenerator CreateAssemblyReportGenerator (params InvolvedType[] types)
    {
      return CreateAssemblyReportGenerator(new IdentifierGenerator<Assembly>(), types);
    }

    public static AssemblyReportGenerator CreateAssemblyReportGenerator (IIdentifierGenerator<Assembly> identifierGenerator, params InvolvedType[] types)
    {
      return new AssemblyReportGenerator(types, identifierGenerator, new IdentifierGenerator<Type>());
    }

    public static InterfaceReportGenerator CreateInterfaceReportGenerator (IOutputFormatter outputFormatter, params InvolvedType[] types)
    {
      return new InterfaceReportGenerator(
          types,
          new IdentifierGenerator<Assembly>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<MemberInfo>(),
          new IdentifierGenerator<Type>(),
          outputFormatter);
    }

    public static MemberReportGenerator CreateMemberReportGenerator (Type type, IOutputFormatter outputFormatter)
    {
      return new MemberReportGenerator(type, new InvolvedType(type), new IdentifierGenerator<Type>(), new IdentifierGenerator<MemberInfo>(), outputFormatter);
    }

    public static InvolvedTypeReportGenerator CreateInvolvedTypeReportGenerator (
        IOutputFormatter outputFormatter,
        params InvolvedType[] involvedTypes)
    {
      var assemblyIdentifierGenerator = StubFactory.CreateIdentifierGeneratorStub(new Assembly[0]);
      var involvedTypeIdentifierGenerator = StubFactory.CreateIdentifierGeneratorStub(involvedTypes.Select(t => t.Type));

      return new InvolvedTypeReportGenerator(
          involvedTypes,
          assemblyIdentifierGenerator,
          involvedTypeIdentifierGenerator,
          new IdentifierGenerator<MemberInfo>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          outputFormatter);
    }
  }
}
