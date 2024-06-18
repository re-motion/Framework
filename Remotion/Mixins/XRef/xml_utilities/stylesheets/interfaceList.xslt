<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <xsl:template name="interfaceList">
        <xsl:param name="rootMCR"/>
        <xsl:param name="interfaces"/>
        <xsl:param name="dir"/>

        <xsl:call-template name="tableTemplate">
            <xsl:with-param name="rootMCR" select="$rootMCR"/>
            <xsl:with-param name="items" select="$interfaces"/>
            <xsl:with-param name="dir" select="$dir"/>
            <xsl:with-param name="tableName">interfaceListTable</xsl:with-param>
            <xsl:with-param name="emptyText">No&#160;Involved&#160;Interfaces</xsl:with-param>
        </xsl:call-template>

    </xsl:template>

    <xsl:template name="interfaceListTable">
        <xsl:param name="rootMCR"/>
        <xsl:param name="interfaces"/>
        <xsl:param name="dir"/>

        <table cellpadding="0" cellspacing="0" border="0" class="display interfaceDataTable">
            <xsl:if test="$dir = '..'">
                <caption>
                    Interfaces&#160;(<xsl:value-of select="count( $interfaces )"/>)
                </caption>
            </xsl:if>
            <thead>
                <tr>
                    <th>Namespace</th>
                    <th>Name</th>
                    <th># of Implementing Classes</th>
                    <th>Assembly</th>
                </tr>
            </thead>
            <tfoot>
                <tr>
                    <td>
                        <xsl:value-of select="count( distinct-values( $interfaces/@namespace ) )"/>
                    </td>
                    <td>
                        <xsl:value-of select="count( $interfaces )"/>
                    </td>
                    <td>-</td>
                    <td>
                        <xsl:value-of select="count( distinct-values( $interfaces/@assembly-ref ) )"/>
                    </td>
                </tr>
            </tfoot>
            <tbody>
                <xsl:for-each select="$interfaces">
                    <tr>
                        <td>
                            <xsl:value-of select="@namespace"/>
                        </td>
                        <td>
                            <xsl:call-template name="GenerateInterfaceLink">
                                <xsl:with-param name="rootMCR" select="$rootMCR"/>
                                <xsl:with-param name="interfaceId" select="@id"/>
                                <xsl:with-param name="dir" select="$dir"/>
                            </xsl:call-template>
                        </td>
                        <td>
                            <xsl:value-of select="count( ImplementedBy/InvolvedType-Reference )"/>
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
    </xsl:template>

</xsl:stylesheet>
