<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Data.DomainObjects.Web.Test.DefaultPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <HEAD>
    <title>default</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </HEAD>
  <body >
    <form id="Form1" method="post" runat="server">
      
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.EditObjectFunction, Remotion.Data.DomainObjects.Web.Test">Bestehendes Objekt editieren</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.NewObjectFunction, Remotion.Data.DomainObjects.Web.Test">Neues Objekt editieren</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.SearchFunction, Remotion.Data.DomainObjects.Web.Test" target="_blank">Objekt suchen</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.WxeTestPageFunction,Remotion.Data.DomainObjects.Web.Test&amp;ClassWithAllDataTypesID=ClassWithAllDataTypes|3f647d79-0caf-4a53-baa7-a56831f8ce2d|System.Guid" target="_blank">WxeFunctionTest</a><br>
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.UndefinedEnumTestFunction,Remotion.Data.DomainObjects.Web.Test">UndefinedEnumTest</a><br> 
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.WxeUserControlTestPageFunction,Remotion.Data.DomainObjects.Web.Test">WxeUserControlTest</a><br> 
      <a href="WxeHandler.ashx?WxeFunctionType=Remotion.Data.DomainObjects.Web.Test.WxeFunctions.PerformanceFunction,Remotion.Data.DomainObjects.Web.Test&ItemCount=1100">Performance Test</a><br> 

    </form>
  </body>
</HTML>
