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
<%@ Page language="c#" Codebehind="DesignTestCheckBoxForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.Design.DesignTestCheckBoxForm" %>

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
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox1 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox2 runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox3 runat="server" datasourcecontrol="CurrentObject" Width="300px" propertyidentifier="Deceased" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox4 runat="server" datasourcecontrol="CurrentObject" Width="150px" propertyidentifier="Deceased" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox17 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" showdescription="True">
    </remotion:boccheckbox><remotion:boccheckbox id=BocCheckBox18 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox5 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" readonly="True" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox6 runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" readonly="True" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox7 runat="server" datasourcecontrol="CurrentObject" readonly="True" Width="300px" propertyidentifier="Deceased" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox8 runat="server" datasourcecontrol="CurrentObject" readonly="True" Width="33%" propertyidentifier="Deceased" showdescription="True">
    </remotion:boccheckbox><remotion:boccheckbox id=BocCheckBox19 runat="server" datasourcecontrol="CurrentObject" readonly="True" Width="33%" propertyidentifier="Deceased" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Edit Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox9 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox10 runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox11 runat="server" datasourcecontrol="CurrentObject" Width="300px" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox12 runat="server" datasourcecontrol="CurrentObject" Width="150px" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox22 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox><remotion:boccheckbox id=BocCheckBox23 runat="server" datasourcecontrol="CurrentObject" Width="33%" propertyidentifier="Deceased" cssclass="bocCheckBox right" showdescription="True">
<LabelStyle cssclass="label">
</LabelStyle>
</remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode right</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id=BocCheckBox13 runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocCheckBox right" showdescription="True">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox14" cssclass="bocCheckBox right" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="50%" readonly="True" showdescription="True">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox15" cssclass="bocCheckBox right" runat="server" Width="300px" datasourcecontrol="CurrentObject" readonly="True" showdescription="True">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox16" cssclass="bocCheckBox right" runat="server" Width="150px" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox20" cssclass="bocCheckBox right" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased">
<LabelStyle cssclass="label">
</LabelStyle></remotion:boccheckbox>
<remotion:boccheckbox id="BocCheckBox21" cssclass="bocCheckBox right" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased">
<LabelStyle cssclass="label">
</LabelStyle>
</remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>
  <tr>
    <td colSpan=2>Edit Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox24" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" cssclass="bocCheckBox block" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox25" runat="server" datasourcecontrol="CurrentObject" width="50%" propertyidentifier="Deceased" cssclass="bocCheckBox block" showdescription="True"></remotion:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox26" runat="server" datasourcecontrol="CurrentObject" Width="300px" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></remotion:boccheckbox></td>
    <td width="50%" class="cell">300px</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox27" runat="server" datasourcecontrol="CurrentObject" Width="150px" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox28" runat="server" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></remotion:boccheckbox>
    <remotion:boccheckbox id="BocCheckBox29" runat="server" datasourcecontrol="CurrentObject" Width="33%" cssclass="bocCheckBox block" showdescription="True" propertyidentifier="Deceased"></remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td></tr>
  <tr>
    <td colSpan=2>Read-Only Mode block</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox30" runat="server" datasourcecontrol="CurrentObject" width="100%" propertyidentifier="Deceased" readonly="True" cssclass="bocCheckBox block" showdescription="True">
</remotion:boccheckbox></td>
    <td width="50%" class="cell">100%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox31" cssclass="bocCheckBox block" runat="server" datasourcecontrol="CurrentObject" propertyidentifier="Deceased" width="50%" readonly="True" showdescription="True">
</remotion:boccheckbox></td>
    <td width="50%" class="cell">50%</td></tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox32" cssclass="bocCheckBox block" runat="server" Width="300px" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased">
</remotion:boccheckbox></td>
    <td width="50%" class="cell">300px</td>
  </tr>
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox33" cssclass="bocCheckBox block" runat="server" Width="150px" datasourcecontrol="CurrentObject" readonly="True" showdescription="True">
</remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>150px</p></td>
  </tr>  
  <tr>
    <td width="50%" class="cell"><remotion:boccheckbox id="BocCheckBox34" cssclass="bocCheckBox block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased"></remotion:boccheckbox>
<remotion:boccheckbox id="BocCheckBox35" cssclass="bocCheckBox block" runat="server" Width="33%" datasourcecontrol="CurrentObject" readonly="True" showdescription="True" propertyidentifier="Deceased"></remotion:boccheckbox></td>
    <td width="50%" class="cell">
      <p>2x 33%</p></td>
  </tr>  </table><remotion:BindableObjectDataSourceControl id="CurrentObject" runat="server" Type="Remotion.ObjectBinding.Sample::Person"></remotion:BindableObjectDataSourceControl></form>
  </body>
</html>
