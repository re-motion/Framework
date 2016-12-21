// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.TestDomain;
using Remotion.ServiceLocation;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;
using PrincipalTestHelper = Remotion.SecurityManager.UnitTests.Domain.AccessControl.PrincipalTestHelper;
using SecurityContext = Remotion.Security.SecurityContext;

namespace Remotion.SecurityManager.UnitTests
{
  [TestFixture]
  public class SecurityServiceTest : DomainTest
  {
    private MockRepository _mocks;
    private IAccessControlListFinder _mockAclFinder;
    private ISecurityTokenBuilder _mockTokenBuilder;
    private IAccessResolver _mockAccessResolver;

    private SecurityService _service;
    private SecurityContext _context;
    private Tenant _tenant;
    private ISecurityPrincipal _principalStub;

    private MemoryAppender _memoryAppender;
    private ClientTransaction _clientTransaction;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mocks = new MockRepository();
      _mockAclFinder = _mocks.StrictMock<IAccessControlListFinder>();
      _mockTokenBuilder = _mocks.StrictMock<ISecurityTokenBuilder>();
      _mockAccessResolver = _mocks.StrictMock<IAccessResolver>();

      _service = new SecurityService (_mockAclFinder, _mockTokenBuilder, _mockAccessResolver);
      _context = SecurityContext.Create (typeof (Order), "Owner", "UID: OwnerGroup", "OwnerTenant", new Dictionary<string, Enum>(), new Enum[0]);

      _clientTransaction = ClientTransaction.CreateRootTransaction();
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        OrganizationalStructureFactory organizationalStructureFactory = new OrganizationalStructureFactory();
        _tenant = organizationalStructureFactory.CreateTenant();
      }

      _principalStub = _mocks.Stub<ISecurityPrincipal>();
      SetupResult.For (_principalStub.User).Return ("group0/user1");

      _memoryAppender = new MemoryAppender();

      LoggerMatchFilter acceptFilter = new LoggerMatchFilter();
      acceptFilter.LoggerToMatch = "Remotion.SecurityManager";
      acceptFilter.AcceptOnMatch = true;
      _memoryAppender.AddFilter (acceptFilter);

      DenyAllFilter denyFilter = new DenyAllFilter();
      _memoryAppender.AddFilter (denyFilter);

      BasicConfigurator.Configure (_memoryAppender);
    }

    public override void TearDown()
    {
      base.TearDown();
      LogManager.ResetConfiguration();
    }

    [Test]
    public void GetAccess_ReturnsAccessTypes ()
    {
      AccessType[] expectedAccessTypes = new AccessType[1];

      SecurityToken token = SecurityToken.Create (
          PrincipalTestHelper.Create (_tenant, null, new Role[0]),
          null,
          null,
          null,
          Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());

      var aclHandle = CreateAccessControlListHandle();
      Expect.Call (_mockAclFinder.Find (_context)).Return (aclHandle);
      Expect.Call (_mockTokenBuilder.CreateToken (_principalStub, _context)).Return (token);
      Expect.Call (_mockAccessResolver.GetAccessTypes (aclHandle, token)).Return (expectedAccessTypes);
      _mocks.ReplayAll();

      AccessType[] actualAccessTypes = _service.GetAccess (_context, _principalStub);

      Assert.That (actualAccessTypes, Is.SameAs (expectedAccessTypes));
    }
    

    [Test]
    public void GetAccess_ContextDoesNotMatchAcl_ReturnsEmptyAccessTypes ()
    {
      SecurityToken token = SecurityToken.Create (
          PrincipalTestHelper.Create (_tenant, null, new Role[0]),
          null,
          null,
          null,
          Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());

      Expect.Call (_mockAclFinder.Find (_context)).Return (null);
      Expect.Call (_mockTokenBuilder.CreateToken (_principalStub, _context)).Return (token);
      _mocks.ReplayAll();

      AccessType[] actualAccessTypes = _service.GetAccess (_context, _principalStub);

      Assert.That (actualAccessTypes, Is.Empty);
    }

    [Test]
    public void GetAccess_WithAccessControlExcptionFromAccessControlListFinder ()
    {
      AccessControlException expectedException = new AccessControlException();
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        Expect.Call (_mockAclFinder.Find (_context)).Throw (expectedException);
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_context, _principalStub);

      _mocks.VerifyAll ();
      Assert.That (accessTypes.Length, Is.EqualTo (0));
      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].ExceptionObject, Is.SameAs (expectedException));
      Assert.That (events[0].Level, Is.EqualTo (Level.Error));
    }

    [Test]
    public void GetAccess_WithAccessControlExcptionFromSecurityTokenBuilder ()
    {
      AccessControlException expectedException = new AccessControlException();
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var aclHandle = CreateAccessControlListHandle();
        Expect.Call (_mockAclFinder.Find (_context)).Return (aclHandle);
        Expect.Call (_mockTokenBuilder.CreateToken (_principalStub, _context)).Throw (expectedException);
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_context, _principalStub);

      _mocks.VerifyAll ();
      Assert.That (accessTypes.Length, Is.EqualTo (0));
      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].ExceptionObject, Is.SameAs (expectedException));
      Assert.That (events[0].Level, Is.EqualTo (Level.Error));
    }

    [Test]
    [Ignore ("RM-5633: Temporarily disabled")]
    public void GetAccess_UsesSecurityFreeSection ()
    {
      ClientTransaction subTransaction;
      using (_clientTransaction.EnterNonDiscardingScope())
      {
        var abstractRoles = new List<IDomainObjectHandle<AbstractRoleDefinition>>();
        //abstractRoles.Add (_ace.SpecificAbstractRole.GetHandle());

        //_ace.GroupCondition = GroupCondition.AnyGroupWithSpecificGroupType;
        //_ace.SpecificGroupType = GroupType.NewObject();
        OrganizationalStructureFactory organizationalStructureFactory = new OrganizationalStructureFactory();
        var role = Role.NewObject();
        role.Group = organizationalStructureFactory.CreateGroup();
        role.Group.Tenant = _tenant;
        role.Position = organizationalStructureFactory.CreatePosition();

        var token = SecurityToken.Create (PrincipalTestHelper.Create (_tenant, null, new[] { role }), null, null, null, abstractRoles);

        subTransaction = _clientTransaction.CreateSubTransaction();
        using (subTransaction.EnterNonDiscardingScope())
        {
          var aclHandle = CreateAccessControlListHandle();
          Expect.Call (_mockAclFinder.Find (_context))
              .WhenCalled (invocation => Assert.That (SecurityFreeSection.IsActive, Is.True))
              .Return (aclHandle);
          Expect.Call (_mockTokenBuilder.CreateToken (_principalStub, _context))
              .WhenCalled (invocation => Assert.That (SecurityFreeSection.IsActive, Is.True))
              .Return (token);
        }
      }

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => MockRepository.GenerateStub<ISecurityProvider>());
      using (new ServiceLocatorScope (serviceLocator))
      {
        _clientTransaction.Extensions.Add (new SecurityClientTransactionExtension());

        _mocks.ReplayAll();

        _service.GetAccess (_context, _principalStub);

        _mocks.VerifyAll();
      }
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.That (((ISecurityProvider) _service).IsNull, Is.False);
    }

    private IDomainObjectHandle<AccessControlList> CreateAccessControlListHandle ()
    {
      return new DomainObjectHandle<StatefulAccessControlList> (new ObjectID (typeof (StatefulAccessControlList), Guid.NewGuid()));
    }
  }
}
