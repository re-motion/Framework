<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <xsl:template name="attributeRefList">
        <xsl:param name="rootMCR"/>
        <xsl:param name="attributeRefs"/>
        <xsl:param name="dir"/>

        <xsl:call-template name="tableTemplate">
            <xsl:with-param name="rootMCR" select="$rootMCR"/>
            <xsl:with-param name="items" select="$attributeRefs"/>
            <xsl:with-param name="dir" select="$dir"/>
            <xsl:with-param name="tableName">attributeRefListTable</xsl:with-param>
            <xsl:with-param name="emptyText">No&#160;Attributes&#160;Applied</xsl:with-param>
        </xsl:call-template>

    </xsl:template>

    <xsl:template name="attributeRefListTable">
        <xsl:param name="rootMCR"/>
        <xsl:param name="attributeRefs"/>
        <xsl:param name="dir"/>

        <table cellpadding="0" cellspacing="0" border="0" class="attributeTable display">
            <caption>Applied&#160;Attributes&#160;(<xsl:value-of select="count( $attributeRefs )"/>)
            </caption>
            <thead>
                <tr>
                    <th>Namespace</th>
                    <th>Name</th>
                    <th># of Uses</th>
                    <th>Assembly</th>
                    <th>Arguments</th>
                </tr>
            </thead>
            <tfoot>
                <tr>
                    <td>
                        <xsl:value-of
                                select="count( distinct-values( /MixinXRefReport/Attributes/Attribute[ ru:contains($attributeRefs/@ref, @id) ]/@namespace ) )"/>
                    </td>
                    <td>
                        <xsl:value-of
                                select="count( distinct-values( /MixinXRefReport/Attributes/Attribute[ ru:contains($attributeRefs/@ref, @id) ]/@id ) )"/>
                    </td>
                    <td>-</td>
                    <td>
                        <xsl:value-of
                                select="count( distinct-values( /MixinXRefReport/Attributes/Attribute[ ru:contains($attributeRefs/@ref, @id) ]/@assembly-ref ) )"/>
                    </td>
                    <td>-</td>
                </tr>
            </tfoot>
            <tbody>
                <xsl:for-each select="$attributeRefs">
                    <xsl:variable name="attr" select="key('attribute', @ref)"/>
                    <tr>
                        <td>
                            <xsl:value-of select="$attr/@namespace"/>
                        </td>
                        <td>
                            <xsl:call-template name="GenerateAttributeLink">
                                <xsl:with-param name="rootMCR" select="$rootMCR"/>
                                <xsl:with-param name="attributeId" select="$attr/@id"/>
                                <xsl:with-param name="dir" select="$dir"/>
                            </xsl:call-template>
                        </td>
                        <td>
                            <xsl:value-of select="count( $attr/AppliedTo/InvolvedType-Reference )"/>
                        </td>
                        <td>
                            <xsl:call-template name="GenerateAssemblyLink">
                                <xsl:with-param name="rootMCR" select="$rootMCR"/>
                                <xsl:with-param name="assemblyId" select="$attr/@assembly-ref"/>
                                <xsl:with-param name="dir" select="$dir"/>
                            </xsl:call-template>
                        </td>
                        <td>
                            <xsl:call-template name="tableTemplate">
                                <xsl:with-param name="rootMCR" select="$rootMCR"/>
                                <xsl:with-param name="items" select="Argument"/>
                                <xsl:with-param name="tableName">attributeArgumentListTable</xsl:with-param>
                                <xsl:with-param name="emptyText">No&#160;Arguments</xsl:with-param>
                            </xsl:call-template>
                        </td>
                    </tr>

                </xsl:for-each>
            </tbody>
        </table>
    </xsl:template>

    <xsl:template name="attributeArgumentListTable">
        <xsl:param name="rootMCR"/>
        <xsl:param name="arguments"/>

        <table cellpadding="0" cellspacing="0" border="0" class="display argumentTable">
            <caption>Arguments&#160;(<xsl:value-of select="count( $arguments )"/>)
            </caption>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Type</th>
                    <th>Value</th>
                </tr>
            </thead>
            <tfoot>
                <tr>
                    <td>
                        <xsl:value-of select="count( $arguments )"/>
                    </td>
                    <td>-</td>
                    <td>-</td>
                </tr>
            </tfoot>
            <tbody>
                <xsl:for-each select="$arguments">
                    <xsl:sort select="@name"/>
                    <xsl:sort select="@kind"/>
                    <tr>
                        <td>
                            <xsl:value-of select="@name"/>
                            <span class="small-method-type">[<xsl:value-of select="@kind"/>]
                            </span>
                        </td>
                        <td>
                            <xsl:value-of select="@type"/>
                        </td>
                        <td>
                            <xsl:value-of select="@value"/>
                        </td>
                    </tr>
                </xsl:for-each>
            </tbody>
        </table>

    </xsl:template>


</xsl:stylesheet>