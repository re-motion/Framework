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

<%@ Page Language="c#" CodeBehind="TestForm.aspx.cs" AutoEventWireup="false" Inherits="OBWTest.TestForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>Test Form</title>
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
  <script type="text/javascript">
    function DoAspNetAjaxCall()
    {
      $("#Result").text("");

      var intValueAsString = $("#IntField").val();
      intValueAsString = intValueAsString == '' ? 0 : intValueAsString;
      var params = {
        stringValue: $("#StringField").val(),
        intValue: parseInt(intValueAsString)
      };
      executingRequest = Sys.Net.WebServiceProxy.invoke("TestService.asmx", "DoStuff", false, params,
                                          function (result, context, methodName)
                                          {
                                            executingRequest = null;
                                            $("#Result").text(result);
                                          },
                                          function (err, context, methodName)
                                          {
                                            executingRequest = null;
                                            $("#Result").text(err.get_message());
                                          });

    }
    function DoJQueryAjaxCall()
    {
      $("#Result").text("");

      var intValueAsString = $("#IntField").val();
      intValueAsString = intValueAsString == '' ? 0 : intValueAsString;
      var params = {
        stringValue: $("#StringField").val(),
        intValue: parseInt(intValueAsString)
      };
      $.ajax({
        type: "POST",
        data: "{ stringValue: '" + params.stringValue + "', intValue: " + params.intValue + "}",
        dataType: "json",
        url: "TestService.asmx/DoStuff",
        contentType: "application/json; charset=utf-8",
        async: false,
        success: function (result)
        {
          $("#Result").text(result.d);
        },
        error: function (err)
        {
          $("#Result").text(err.responseText);
        }
      });
    }

    $(document).ready(function ()
    {
      $("#StringField").bind("keydown", function (event)
      {
        // re-motion: block event bubbling
        event.stopPropagation();
        if (event.keyCode == 9) // TAB
        {
          DoJQueryAjaxCall();
        } 
      });
    });
    
  </script>
</head>
<body>
  <form id="form1" runat="server">
  <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true"
    AsyncPostBackTimeout="3600" />
  <input id="StringField" type="text" />
  <input id="IntField" type="text" />
  <input type="button" value="ASP.NET Ajax" onclick="DoAspNetAjaxCall()" />
  <input type="button" value="jQuery Ajax" onclick="DoJQueryAjaxCall()" />
  <div id="Result">
  </div>
  </form>
</body>
</html>
