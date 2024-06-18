<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <xsl:template name="involvedTypeList">
        <xsl:param name="rootMCR"/>
        <xsl:param name="involvedTypes"/>
        <xsl:param name="dir"/>
        <xsl:param name="caption"/>
        <xsl:param name="emptyText"/>

        <xsl:call-template name="tableTemplate">
            <xsl:with-param name="rootMCR" select="$rootMCR"/>
            <xsl:with-param name="items" select="$involvedTypes"/>
            <xsl:with-param name="dir" select="$dir"/>
            <xsl:with-param name="tableName">involvedTypeListTable</xsl:with-param>
            <xsl:with-param name="emptyText" select="$emptyText"/>
            <xsl:with-param name="caption" select="$caption"/>
        </xsl:call-template>

    </xsl:template>

    <xsl:template name="involvedTypeListTable">
        <xsl:param name="rootMCR"/>
        <xsl:param name="involvedTypes"/>
        <xsl:param name="dir"/>
        <xsl:param name="caption"/>

        <xsl:if test="$dir = '.'">
            <xsl:call-template name="involvedTypeListTableWithClass">
                <xsl:with-param name="rootMCR" select="$rootMCR"/>
                <xsl:with-param name="involvedTypes" select="$involvedTypes"/>
                <xsl:with-param name="dir" select="$dir"/>
                <xsl:with-param name="caption" select="$caption"/>
                <xsl:with-param name="tableType">indexDataTable</xsl:with-param>
            </xsl:call-template>
        </xsl:if>

        <xsl:if test="$dir = '..'">
            <xsl:call-template name="involvedTypeListTableWithClass">
                <xsl:with-param name="rootMCR" select="$rootMCR"/>
                <xsl:with-param name="involvedTypes" select="$involvedTypes"/>
                <xsl:with-param name="dir" select="$dir"/>
                <xsl:with-param name="caption" select="$caption"/>
                <xsl:with-param name="tableType">dataTable</xsl:with-param>
            </xsl:call-template>
        </xsl:if>

    </xsl:template>


    <xsl:template name="involvedTypeListTableWithClass">
        <xsl:param name="rootMCR"/>
        <xsl:param name="involvedTypes"/>
        <xsl:param name="dir"/>
        <xsl:param name="caption"/>
        <xsl:param name="tableType"/>

        <xsl:variable name="isTargetList"
                      select="count( $involvedTypes[@is-target = true()] ) = count( $involvedTypes ) "/>
        <xsl:variable name="isMixinList" select=" not( $isTargetList ) "/>
        <div>
            <table cellpadding="0" cellspacing="0" border="0" class="display {$tableType}">
                <xsl:if test="$dir = '..'">
                    <caption>
                        <xsl:value-of select="$caption"/>&#160;(<xsl:value-of select="count( $involvedTypes )"/>)
                    </caption>
                </xsl:if>
                <thead>
                    <tr>
                        <th>Namespace</th>
                        <th>Name</th>
                        <xsl:if test="$isTargetList">
                            <th># of Mixins applied</th>
                        </xsl:if>

                        <xsl:if test="$isMixinList">
                            <th>applied to # Targets</th>
                        </xsl:if>

                        <th>Base</th>
                        <th>Assembly</th>
                    </tr>
                </thead>
                <tfoot>
                    <tr>
                        <td>
                            <xsl:value-of select="count( distinct-values( $involvedTypes/@namespace ) )"/>
                        </td>
                        <td>
                            <xsl:value-of select="count( $involvedTypes )"/>
                        </td>
                        <xsl:if test="$isTargetList">
                            <td>-</td>
                        </xsl:if>
                        <xsl:if test="$isMixinList">
                            <td>-</td>
                        </xsl:if>
                        <td>-</td>
                        <td>
                            <xsl:value-of select="count( distinct-values( $involvedTypes/@assembly-ref ) )"/>
                        </td>
                    </tr>
                </tfoot>
                <tbody>
                    <xsl:for-each select="$involvedTypes">
                        <tr class="{ ru:getDubiosInvolvedTypeClass(.) } { ru:getUnusedMixinClass(.) }">
                            <td>
                                <xsl:value-of select="@namespace"/>
                            </td>
                            <td>
                                <xsl:call-template name="GenerateInvolvedTypeLink">
                                    <xsl:with-param name="rootMCR" select="$rootMCR"/>
                                    <xsl:with-param name="involvedTypeId" select="@id"/>
                                    <xsl:with-param name="dir" select="$dir"/>
                                </xsl:call-template>
                            </td>
                            <xsl:if test="$isTargetList">
                                <td>
                                    <xsl:value-of select="count( Mixins/Mixin )"/>
                                </td>
                            </xsl:if>
                            <xsl:if test="$isMixinList">
                                <td>
                                    <xsl:value-of select="count( Targets/Target )"/>
                                </td>
                            </xsl:if>
                            <td>
                                <xsl:if test="@base-ref = 'none'">
                                    <xsl:value-of select="@base"/>
                                </xsl:if>

                                <xsl:if test="@base-ref != 'none'">
                                    <xsl:variable name="base-ref" select="base-ref"/>
                                    <xsl:call-template name="GenerateInvolvedTypeLink">
                                        <xsl:with-param name="rootMCR" select="$rootMCR"/>
                                        <xsl:with-param name="involvedTypeId" select="@base-ref"/>
                                        <xsl:with-param name="dir" select="$dir"/>
                                    </xsl:call-template>
                                </xsl:if>
                            </td>
                            <td>
                                <xsl:call-template name="GenerateAssemblyLink">
                                    <xsl:with-param name="rootMCR" select="$rootMCR"/>
                                    <xsl:with-param name="assemblyId" select="@assembly-ref"/>
                                    <xsl:with-param name="dir" select="$dir"/>
                                </xsl:call-template>
                            </td>
                        </tr>
                    </xsl:for-each>
                </tbody>
            </table>
        </div>
    </xsl:template>

</xsl:stylesheet>