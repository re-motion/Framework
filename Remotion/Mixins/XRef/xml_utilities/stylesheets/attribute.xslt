<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <xsl:template name="attribute">

        <xsl:call-template name="attributeList">
            <xsl:with-param name="rootMCR" select="/"/>
            <xsl:with-param name="attributes" select="/MixinXRefReport/Attributes/Attribute"/>
            <xsl:with-param name="dir">.</xsl:with-param>
        </xsl:call-template>

        <xsl:for-each select="/MixinXRefReport/Attributes/Attribute">
            <!-- generate interface detail site for each interface -->
            <xsl:call-template name="attributeDetailSite">
                <xsl:with-param name="title" select="@name"/>
            </xsl:call-template>

        </xsl:for-each>
    </xsl:template>

    <xsl:template name="attributeDetailSite">
        <xsl:param name="title"/>
        <xsl:call-template name="htmlSite">
            <xsl:with-param name="siteTitle">
                <xsl:value-of select="$title"/> (Attribute)
            </xsl:with-param>
            <xsl:with-param name="siteFileName">attributes/<xsl:value-of select="@id"/>.html
            </xsl:with-param>
            <xsl:with-param name="bodyContentTemplate">attributeDetail</xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template name="attributeDetail">
        <h1>
            <xsl:value-of select="@name"/>
        </h1>
        <h2>
            <a href="../attribute_index.html">[Involved Attribute]</a>
        </h2>

        <div class="fromAssembly">
            from assembly
            <xsl:call-template name="GenerateAssemblyLink">
                <xsl:with-param name="rootMCR" select="/"/>
                <xsl:with-param name="assemblyId" select="@assembly-ref"/>
                <xsl:with-param name="dir">..</xsl:with-param>
            </xsl:call-template>
        </div>

        <xsl:call-template name="treeBuilder">
            <xsl:with-param name="caption">Used&#160;On&#160;(<xsl:value-of
                    select="count( AppliedTo/InvolvedType-Reference )"/>)
            </xsl:with-param>
            <xsl:with-param name="treeNodes"
                            select="/MixinXRefReport/InvolvedTypes/InvolvedType[ (ru:contains(HasAttributes/HasAttribute/@ref, current()/@id))]"/>
            <xsl:with-param name="allReferences" select="AppliedTo/InvolvedType-Reference/@ref"/>
        </xsl:call-template>

        <xsl:call-template name="memberList">
            <xsl:with-param name="members" select="Members/Member"/>
            <xsl:with-param name="rootMCR" select="/"/>
        </xsl:call-template>

    </xsl:template>

</xsl:stylesheet>
