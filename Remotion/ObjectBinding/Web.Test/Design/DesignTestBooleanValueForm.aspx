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
<%@ Page language="c#" Codebehind="DesignTestBooleanValueForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestBooleanValueForm" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
    <title>DesignTest: BooleanValue Form</title>
<meta content="Microsoft Visual Studio .NET 7.1" name=GENERATOR>
<meta content=C# name=CODE_LANGUAGE>
<meta content=JavaScript name=vs_defaultClientScript>
<meta content=http://schemas.microsoft.com/intellisense/ie5 name=vs_targetSchema><remotion:htmlheadcontents id=HtmlHeadContents runat="server"></remotion:htmlheadcontents>
  </head>
<body>
<form id=Form method=post runat="server"><remotion:webbutton id=PostBackButton runat="server" Text="PostBack"></remotion:webbutton>
<h1>DesignTest: BooleanValue Form</h1>
<table width="100%">
  <tr>
    <td colSpan=2>Edit Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue1 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue2 runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue3 runat="server" datasourcecontrol="CurrentObject" Width="300px" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue4 runat="server" datasourcecontrol="CurrentObject" Width="150px" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue17 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased">
    </remotion:bocbooleanvalue><remotion:bocbooleanvalue id=BocBooleanValue18 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue5 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" readonly="True"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue6 runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" readonly="True"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue7 runat="server" datasourcecontrol="CurrentObject" readonly="True" Width="300px" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue8 runat="server" datasourcecontrol="CurrentObject" readonly="True" Width="33%" propertyidentifier="Deceased">
    </remotion:bocbooleanvalue><remotion:bocbooleanvalue id=BocBooleanValue19 runat="server" datasourcecontrol="CurrentObject" readonly="True" Width="33%" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue9 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue10 runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue11 runat="server" datasourcecontrol="CurrentObject" Width="300px" cssclass="bocBooleanValue right" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue12 runat="server" datasourcecontrol="CurrentObject" Width="150px" cssclass="bocBooleanValue right" propertyidentifier="Deceased">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue22 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue><remotion:bocbooleanvalue id=BocBooleanValue23 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id=BocBooleanValue13 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocBooleanValue right">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue14" cssclass="bocBooleanValue right" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="50%" readonly="True">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue15" cssclass="bocBooleanValue right" runat="server" Width="300px" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" readonly="True">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue16" cssclass="bocBooleanValue right" runat="server" Width="150px" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" readonly="True">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue20" cssclass="bocBooleanValue right" runat="server" Width="33%" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" readonly="True">
<labelstyle cssclass="label">
</LabelStyle></remotion:bocbooleanvalue>
<remotion:bocbooleanvalue id="BocBooleanValue21" cssclass="bocBooleanValue right" runat="server" Width="33%" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" readonly="True">
<labelstyle cssclass="label">
</LabelStyle>
</remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue24" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" cssclass="bocBooleanValue block"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue25" runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" cssclass="bocBooleanValue block"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue26" runat="server" datasourcecontrol="CurrentObject" Width="300px" cssclass="bocBooleanValue block" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue27" runat="server" datasourcecontrol="CurrentObject" Width="150px" cssclass="bocBooleanValue block" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue28" runat="server" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocBooleanValue block" propertyidentifier="Deceased"></remotion:bocbooleanvalue>
    <remotion:bocbooleanvalue id="BocBooleanValue29" runat="server" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocBooleanValue block" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue30" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocBooleanValue block">
</remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue31" cssclass="bocBooleanValue block" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="50%" readonly="True">
</remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue32" cssclass="bocBooleanValue block" runat="server" Width="300px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="Deceased">
</remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue33" cssclass="bocBooleanValue block" runat="server" Width="150px" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="Deceased">
</remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><remotion:bocbooleanvalue id="BocBooleanValue34" cssclass="bocBooleanValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="Deceased"></remotion:bocbooleanvalue>
<remotion:bocbooleanvalue id="BocBooleanValue35" cssclass="bocBooleanValue block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" propertyidentifier="Deceased"></remotion:bocbooleanvalue></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>  </table><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person"></remotion:BindableObjectDataSourceControl></form>
  </body>
</html>
