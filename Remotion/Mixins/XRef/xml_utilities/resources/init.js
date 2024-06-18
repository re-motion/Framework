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
$(window).resize(function () {
    initTableSorter();
});


$(document).ready(function () {

    initTableSorter();

    setSelectedIndexClass();
    initTreeView();

    prepareCollapsing();

    if (document.location.hash) {
        highlight(document.location.hash);
    }

    $('a[href*=#]').click(function () {
        var elemId = '#' + $(this).attr('href').split('#')[1];
        highlight(elemId);
    });
});

function getCookieName() {
    var file_name = document.location.href;
    var tmp = file_name.split("/");
    return tmp[tmp.length - 2] + "_" + (tmp[tmp.length - 1].split("."))[0];
}


function extractModifier(node) {
    if ($(node).find("span.Name").text() != "")
        return ($(node).find("span.Name").text());
    if ($(node).find("span.Keyword").text() != "")
        return ($(node).find("span.Keyword").text());
    return $(node).text();
}

function calculateModifierSorting(node) {

    var s = extractModifier(node);

    var index = -1;

    if (s.indexOf("public") != -1)
        index = 0;
    if (s.indexOf("protected internal") != -1)
        index = 10;
    if (s.indexOf("protected") != -1)
        index = 20;
    if (s.indexOf("internal") != -1)
        index = 30;
    if (s.indexOf("private") != -1)
        index = 40;

    if (s.indexOf("abstract") != -1)
        index += 1;
    if (s.indexOf("override") != -1)
        index += 2;
    if (s.indexOf("virtual") != -1)
        index += 3;

    return index;
}

jQuery.fn.dataTableExt.oSort['modifier-visibility-asc'] = function (a, b) {
    var x = calculateModifierSorting(a);
    var y = calculateModifierSorting(b);

    return ((x < y) ? -1 : ((x > y) ? 1 : 0));
};

jQuery.fn.dataTableExt.oSort['modifier-visibility-desc'] = function (a, b) {
    var x = calculateModifierSorting(a);
    var y = calculateModifierSorting(b);

    return ((x < y) ? 1 : ((x > y) ? -1 : 0));
};

jQuery.fn.dataTableExt.oSort['signature-asc'] = function (a, b) {
    var x = $(a).find("span.Name").text();
    var y = $(b).find("span.Name").text();

    return ((x < y) ? -1 : ((x > y) ? 1 : 0));
};

jQuery.fn.dataTableExt.oSort['signature-desc'] = function (a, b) {
    var x = $(a).find("span.Name").text();
    var y = $(b).find("span.Name").text();

    return ((x < y) ? 1 : ((x > y) ? -1 : 0));
};

function initTableSorter() {

    $('.assemblyDataTable').dataTable({
        "bStateSave": true,
        "bRetrieve": true,
        "iCookieDuration": 60 * 60 * 24 * 365,
        "bJQueryUI": true,
        "sPaginationType": "full_numbers",
        "aaSorting": [[0, 'asc'], [1, 'asc']],
        "aoColumns": [
            {"sType": "html"},
            null,
            null,
            null,
            null,
            null
        ]
    });

    if (location.href.indexOf("assemblies") != -1) {
        $('.indexDataTable').dataTable({
            "bStateSave": true, "bRetrieve": true,
            "iCookieDuration": 60 * 60 * 24 * 365,
            "bJQueryUI": true,
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": true,
            "bInfo": false,
            "bAutoWidth": false,
            "aaSorting": [[0, 'asc'], [1, 'asc']],
            "aoColumns": [
                null,
                {"sType": "html"},
                null,
                {"sType": "html"},
                {"sType": "html"}
            ]
        });

        $('.interfaceDataTable, .attributeDataTable').dataTable({
            "bStateSave": true,
            "bRetrieve": true,
            "iCookieDuration": 60 * 60 * 24 * 365,
            "bJQueryUI": true,
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": true,
            "bInfo": false,
            "bAutoWidth": false,
            "aaSorting": [[0, 'asc'], [1, 'asc']],
            "aoColumns": [
                null,
                {"sType": "html"},
                null,
                {"sType": "html"}
            ]
        });
    } else {
        $('.indexDataTable').dataTable({
            "bStateSave": true,
            "bRetrieve": true,
            "iCookieDuration": 60 * 60 * 24 * 365,
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "aaSorting": [[0, 'asc'], [1, 'asc']],
            "aoColumns": [
                null,
                {"sType": "html"},
                null,
                {"sType": "html"},
                {"sType": "html"}
            ]
        });

        $('.interfaceDataTable, .attributeDataTable').dataTable({
            "bStateSave": true,
            "bRetrieve": true,
            "iCookieDuration": 60 * 60 * 24 * 365,
            "bJQueryUI": true,
            "sPaginationType": "full_numbers",
            "aaSorting": [[0, 'asc'], [1, 'asc']],
            "aoColumns": [
                null,
                {"sType": "html"},
                null,
                {"sType": "html"}
            ]
        });
    }

    if (onIndexSite())
        $('.fg-toolbar').width($('.indexDataTable, .interfaceDataTable, .attributeDataTable, .assemblyDataTable').width() - 12);

    $('.mixinTable').dataTable({
        "bStateSave": true,
        "bRetrieve": true,
        "iCookieDuration": 60 * 60 * 24 * 365,
        "bJQueryUI": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": true,
        "bInfo": false,
        "bAutoWidth": false,
        "aaSorting": [[0, 'asc'], [1, 'asc']],
        "aoColumns": [
            null,
            {"sType": "html"},
            null,
            null,
            {"sType": "html"},
            {"sType": "html"},
            {"sType": "html"},
            {"sType": "html"},
            null
        ]
    });

    $('.dataTable').dataTable({
        "bStateSave": true,
        "bRetrieve": true,
        "iCookieDuration": 60 * 60 * 24 * 365,
        "bJQueryUI": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": true,
        "bInfo": false,
        "bAutoWidth": false,
        "aaSorting": [[0, 'asc'], [1, 'asc']],
        "aoColumns": [
            null,
            {"sType": "html"},
            null,
            {"sType": "html"},
            {"sType": "html"}
        ]
    });

    $('.attributeTable').dataTable({
        "bStateSave": true,
        "bRetrieve": true,
        "iCookieDuration": 60 * 60 * 24 * 365,
        "bJQueryUI": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": true,
        "bInfo": false,
        "bAutoWidth": false,
        "aaSorting": [[0, 'asc'], [1, 'asc']],
        "aoColumns": [
            null,
            {"sType": "html"},
            null,
            {"sType": "html"},
            null
        ]
    });

    $('.argumentTable').dataTable({
        "bStateSave": true,
        "bRetrieve": true,
        "iCookieDuration": 60 * 60 * 24 * 365,
        "bJQueryUI": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": true,
        "bInfo": false,
        "bAutoWidth": false
    });

    if ($('.declaredMembersDataTable').find('th').length == 5) {
        $('.declaredMembersDataTable').dataTable({
            "bStateSave": true,
            "bRetrieve": true,
            "iCookieDuration": 60 * 60 * 24 * 365,
            "bJQueryUI": true,
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": true,
            "bInfo": false,
            "bAutoWidth": false,
            "aoColumns": [
                null,
                null,
                {"sType": "modifier-visibility"},
                {"sType": "signature"},
                {"sType": "html"}
            ]
        });
    } else {
        $('.declaredMembersDataTable').dataTable({
            "bStateSave": true,
            "bRetrieve": true,
            "iCookieDuration": 60 * 60 * 24 * 365,
            "bJQueryUI": true,
            "bPaginate": false,
            "bLengthChange": false,
            "bFilter": false,
            "bSort": true,
            "bInfo": false,
            "bAutoWidth": false,
            "aoColumns": [
                null,
                null,
                {"sType": "modifier-visibility"},
                {"sType": "signature"}
            ]
        });
    }

    $('.overriddenBaseMembersDataTable').dataTable({
        "bStateSave": true,
        "bRetrieve": true,
        "iCookieDuration": 60 * 60 * 24 * 365,
        "bJQueryUI": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": true,
        "bInfo": false,
        "bAutoWidth": false,
        "aoColumns": [
            null,
            null,
            {"sType": "modifier-visibility"},
            {"sType": "signature"},
            null
        ]
    });

    $('.baseMembersDataTable').dataTable({
        "bStateSave": true,
        "bRetrieve": true,
        "iCookieDuration": 60 * 60 * 24 * 365,
        "bJQueryUI": true,
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": true,
        "bInfo": false,
        "bAutoWidth": false,
        "aoColumns": [
            null,
            null,
            {"sType": "modifier-visibility"},
            {"sType": "signature"},
            null
        ]
    });
}

function setSelectedIndexClass() {
    $("#navigation a").filter(function () {
        /* does the link in the navigation bar point to the current document? */
        return this.href == location.href;
    }).addClass("currentIndex");
}

function onIndexSite() {
    return location.href.indexOf("index.html") != -1;
}

function prepareCollapsing() {
    /* make only non index site collapse-able */
    if (onIndexSite())
        return;

    var cookieName = getCookieName();
    var cookie = $.cookie(cookieName);

    if (cookie == undefined || cookie == null) {
        $("caption:contains('Mixins')").addClass("visible");
        $("caption:contains('Attributes')").addClass("visible");
        $("caption:contains('Members')").addClass("visible");
        $(".treeHeader").addClass("visible");
        if ($("h2 a").text() == "[Involved Interface]")
            $("caption:contains('Members')").addClass("visible");
        saveCookie();
    }

    var cookieValue = $.cookie(cookieName);
    if (cookieValue == null || cookieValue == "")
        return;

    var classArray = cookieValue.split(",");

    $("caption, .treeHeader").each(function (n) {

        if (this.tagName.toUpperCase() == "CAPTION") {
            $(this).addClass(classArray[n]);

            if (classArray[n] == "hidden") {
                $(this).nextAll("thead, tfoot, tbody").hide();
            }

            $(this).click(function () {
                $(this).toggleClass("visible").toggleClass("hidden");

                if ($(this).hasClass("hidden"))
                    $(this).nextAll("thead, tfoot, tbody").hide();
                else
                    $(this).nextAll("thead, tfoot, tbody").show();
                saveCookie();
            });
        } else { // tagname = DIV           

            // internet explorer doesn't like collapsing of trees
            // if (!jQuery.browser.msie) {

            $(this).addClass(classArray[n]);

            if (classArray[n] == "hidden") {
                $('.treeView').children().hide();
            }

            $(this).click(function () {
                $(this).toggleClass("visible").toggleClass("hidden");
                $('.treeView').children().toggle();
                saveCookie();
            });
            //}
        }
    });
}

function saveCookie() {
    var collapseElements = $("caption, .treeHeader");
    var cookieValue = "";

    collapseElements.each(function (n) {
        if (n != 0)
            cookieValue += ",";
        cookieValue += ($(this).hasClass("visible") ? "visible" : "hidden");
    });

    $.cookie(getCookieName(), cookieValue);
}

function initTreeView() {

    if (onIndexSite())
        return;

    $(".treeView").children().treeview({
        collapsed: true,
        persist: "cookie",
        cookieId: getCookieName() + "_treeview"
    });
}

function highlight(elemId) {
    var elem = $(elemId);
    var color = $(elemId).parent().css("background-color");
    var elementColor = $(elemId).css("background-color");

    elem.animate({backgroundColor: '#ffff66'}, 1500);
    $(elemId).nextAll("td").animate({backgroundColor: '#ffff66'}, 1500);
    setTimeout(function () {
        $(elemId).animate({backgroundColor: elementColor}, 3000)
    }, 1000);
    setTimeout(function () {
        $(elemId).nextAll("td").animate({backgroundColor: color}, 3000)
    }, 1000);
}