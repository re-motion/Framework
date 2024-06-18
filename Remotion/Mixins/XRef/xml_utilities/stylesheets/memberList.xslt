<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <xsl:template name="memberList">
        <xsl:param name="members"/>
        <xsl:param name="rootMCR"/>

        <xsl:call-template name="tableTemplate">
            <xsl:with-param name="rootMCR" select="$rootMCR"/>
            <xsl:with-param name="items" select="$members"/>
            <xsl:with-param name="dir"></xsl:with-param>
            <xsl:with-param name="tableName">memberListTable</xsl:with-param>
            <xsl:with-param name="emptyText">No Public Members</xsl:with-param>
        </xsl:call-template>

    </xsl:template>

    <xsl:template name="memberListTable">
        <xsl:param name="members"/>
        <xsl:param name="rootMCR"/>

        <table cellpadding="0" cellspacing="0" border="0" class="declaredMembersDataTable display">
            <caption>
                Declared&#160;Members&#160;(<xsl:value-of
                    select="count( $members[@is-declared-by-this-class = true()] )"/>)
            </caption>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Modifiers</th>
                    <th>Signature</th>
                    <xsl:if test="exists($members[@is-declared-by-this-class = true()]/Overrides/Mixin-Reference)">
                        <th>Overridden By</th>
                    </xsl:if>
                </tr>
            </thead>
            <tfoot>
                <tr>
                    <td>
                        <xsl:value-of select="count( $members[@is-declared-by-this-class = true()] )"/>
                    </td>
                    <td>-</td>
                    <td>-</td>
                    <td>-</td>
                    <xsl:if test="exists($members[@is-declared-by-this-class = true()]/Overrides/Mixin-Reference)">
                        <td>-</td>
                    </xsl:if>
                </tr>
            </tfoot>
            <tbody>
                <xsl:for-each select="$members[@is-declared-by-this-class = true()] ">
                    <tr>
                        <!-- to be XHTML conform -->
                        <xsl:if test="@name = '.ctor'">
                            <td id="Constructor">
                                <xsl:value-of select="@name"/>
                            </td>
                        </xsl:if>
                        <xsl:if test="@name != '.ctor'">
                            <td id="{@name}">
                                <xsl:value-of select="@name"/>
                            </td>
                        </xsl:if>
                        <td>
                            <xsl:value-of select="@type"/>
                        </td>
                        <td>
                            <xsl:apply-templates select="Modifiers"/>
                        </td>
                        <td>
                            <xsl:apply-templates select="Signature"/>
                        </td>
                        <xsl:if test="exists($members[@is-declared-by-this-class = true()]/Overrides/Mixin-Reference)">
                            <td>
                                <xsl:for-each select="Overrides/Mixin-Reference">
                                    <xsl:if test="position() != 1">,</xsl:if>
                                    <xsl:call-template name="GenerateMixinReferenceLink">
                                        <xsl:with-param name="mixin" select="."/>
                                    </xsl:call-template>
                                </xsl:for-each>
                            </td>
                        </xsl:if>
                    </tr>
                </xsl:for-each>
            </tbody>
        </table>

        <xsl:if test="count( $members[@is-declared-by-this-class = false()] ) > 0">
            <table cellpadding="0" cellspacing="0" border="0" class="display overriddenBaseMembersDataTable">
                <caption>
                    Base&#160;Members&#160;overridden&#160;by&#160;mixins&#160;(<xsl:value-of
                        select="count( $members[@is-declared-by-this-class = false()] )"/>)
                </caption>
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Type</th>
                        <th>Modifiers</th>
                        <th>Signature</th>
                        <xsl:if test="exists($members[@is-declared-by-this-class = false()]/Overrides)">
                            <th>Overridden By</th>
                        </xsl:if>
                    </tr>
                </thead>
                <tfoot>
                    <tr>
                        <td>
                            <xsl:value-of select="count( $members[@is-declared-by-this-class = false()] )"/>
                        </td>
                        <td>-</td>
                        <td>-</td>
                        <td>-</td>
                        <xsl:if test="exists($members[@is-declared-by-this-class = false()]/Overrides)">
                            <td>-</td>
                        </xsl:if>
                    </tr>
                </tfoot>
                <tbody>
                    <xsl:for-each select="$members[@is-declared-by-this-class = false()] ">
                        <tr>
                            <td id="{@name}">
                                <xsl:value-of select="@name"/>
                            </td>
                            <td>
                                <xsl:value-of select="@type"/>
                            </td>
                            <td>
                                <xsl:apply-templates select="Modifiers"/>
                            </td>
                            <td>
                                <xsl:apply-templates select="Signature"/>
                            </td>
                            <xsl:if test="exists($members/Overrides)">
                                <td>
                                    <xsl:for-each select="Overrides/Mixin-Reference">
                                        <xsl:if test="position() != 1">
                                            <br/>
                                        </xsl:if>
                                        <xsl:call-template name="GenerateMixinReferenceLink">
                                            <xsl:with-param name="mixin" select="."/>
                                        </xsl:call-template>
                                    </xsl:for-each>
                                </td>
                            </xsl:if>
                        </tr>
                    </xsl:for-each>
                </tbody>
            </table>
        </xsl:if>

        <!-- Shows ALL derived members - large performance impact -->
        <xsl:if test="@base-ref != 'none'">

            <xsl:variable name="baseMembers">
                <xsl:call-template name="membersFromBaseClasses">
                    <xsl:with-param name="rootMCR" select="$rootMCR"/>
                    <xsl:with-param name="currentNode"
                                    select="/MixinXRefReport/InvolvedTypes/InvolvedType[@id = current()/@base-ref]"/>
                    <xsl:with-param name="members" select="$members"/>
                </xsl:call-template>
            </xsl:variable>

            <xsl:variable name="baseMemberCount" select="count($baseMembers/node())"/>

            <table cellpadding="0" cellspacing="0" border="0" class="display baseMembersDataTable">
                <caption>
                    Members&#160;declared&#160;by&#160;a&#160;base&#160;class&#160;(<xsl:value-of
                        select="$baseMemberCount"/>)
                </caption>
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Type</th>
                        <th>Modifiers</th>
                        <th>Signature</th>
                        <th>Delcared by</th>
                    </tr>
                </thead>
                <tfoot>
                    <tr>
                        <td>
                            <xsl:value-of select="$baseMemberCount"/>
                        </td>
                        <td>-</td>
                        <td>-</td>
                        <td>-</td>
                        <td>-</td>
                    </tr>
                </tfoot>
                <tbody>
                    <xsl:copy-of select="$baseMembers"/>
                </tbody>
            </table>
        </xsl:if>

    </xsl:template>

    <xsl:template name="membersFromBaseClasses">
        <xsl:param name="rootMCR"/>
        <xsl:param name="currentNode"/>
        <xsl:param name="members"/>

        <xsl:for-each select="$currentNode/Members/Member ">
            <xsl:if test="@name != '.ctor'">
                <tr>
                    <td id="{@name}">
                        <xsl:value-of select="@name"/>
                    </td>
                    <td>
                        <xsl:value-of select="@type"/>
                    </td>
                    <td>
                        <xsl:apply-templates select="Modifiers"/>
                    </td>
                    <td>
                        <xsl:apply-templates select="Signature"/>
                    </td>
                    <td>
                        <xsl:call-template name="GenerateInvolvedTypeLink">
                            <xsl:with-param name="rootMCR" select="$rootMCR"/>
                            <xsl:with-param name="involvedTypeId" select="$currentNode/@id"/>
                            <xsl:with-param name="dir" select="'..'"/>
                        </xsl:call-template>
                    </td>
                </tr>
            </xsl:if>
        </xsl:for-each>

        <xsl:if test="$currentNode/@base-ref != 'none'">
            <xsl:call-template name="membersFromBaseClasses">
                <xsl:with-param name="rootMCR" select="$rootMCR"/>
                <xsl:with-param name="currentNode"
                                select="/MixinXRefReport/InvolvedTypes/InvolvedType[@id = $currentNode/@base-ref]"/>
                <xsl:with-param name="members" select="$members"/>
            </xsl:call-template>
        </xsl:if>
    </xsl:template>


    <!-- without strip-space, each span would be created in an own line. -->
    <xsl:strip-space elements="Modifiers Signature"/>

    <xsl:template match="Keyword | Type | Text | Name | ParameterName | ExplicitInterfaceName">
        <span class="{if (@languageType) then @languageType else name(.)}">
            <xsl:choose>
                <xsl:when test="name(.) = 'Type'">
                    <xsl:value-of select="ru:GetPrettyTypeName(./text())"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:value-of select="."/>
                </xsl:otherwise>
            </xsl:choose>
            <xsl:if test=". != '.' and . != '(' and . != '[' and . != '&lt;' and name(.) != 'ParameterName' and name(.) != 'ExplicitInterfaceName' and following-sibling::*[1] !=  ',' and following-sibling::*[1] !=  ']' and following-sibling::*[1] !=  '&gt;'">
                <xsl:text> </xsl:text>
            </xsl:if>
        </span>
    </xsl:template>

</xsl:stylesheet>
