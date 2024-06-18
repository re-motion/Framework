<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <!-- general utility functions -->
    <xsl:function name="ru:contains">
        <xsl:param name="sequence"/>
        <xsl:param name="searchItem"/>

        <xsl:copy-of select=" exists( index-of( $sequence, $searchItem ) )"/>
    </xsl:function>

    <xsl:function name="ru:getDubiosInvolvedTypeClass">
        <xsl:param name="it"/>

        <xsl:copy-of
                select="if ( $it/@is-target = true() and $it/@is-mixin = true() ) then 'dubiosInvolvedType' else '' "/>
    </xsl:function>

    <xsl:function name="ru:getUnusedMixinClass">
        <xsl:param name="it"/>

        <xsl:copy-of select="if ( $it/@is-unusedmixin = true() ) then 'unusedMixinClass' else '' "/>
    </xsl:function>

    <!-- type name format functions -->
    <xsl:function name="ru:GetPrettyTypeName">
        <xsl:param name="fullname"/>

        <xsl:choose>
            <xsl:when test="$fullname = 'System.Boolean'">
                <xsl:text>bool</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Int16'">
                <xsl:text>short</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Int32'">
                <xsl:text>int</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Int64'">
                <xsl:text>long</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Single'">
                <xsl:text>float</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.UInt16'">
                <xsl:text>ushort</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.UInt32'">
                <xsl:text>uint</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.UInt64'">
                <xsl:text>ulong</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Byte'">
                <xsl:text>byte</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Char'">
                <xsl:text>char</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Decimal'">
                <xsl:text>decimal</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Double'">
                <xsl:text>double</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.SByte'">
                <xsl:text>sbyte</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.String'">
                <xsl:text>string</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Object'">
                <xsl:text>object</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Void'">
                <xsl:text>void</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.Boolean&gt;'">
                <xsl:text>bool?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.Int16&gt;'">
                <xsl:text>short?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.Int32&gt;'">
                <xsl:text>int?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.Int64&gt;'">
                <xsl:text>long?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.Single&gt;'">
                <xsl:text>float?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.UInt16&gt;'">
                <xsl:text>ushort?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.UInt32&gt;'">
                <xsl:text>uint?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.UInt64&gt;'">
                <xsl:text>ulong?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.Byte&gt;'">
                <xsl:text>byte?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.Char&gt;'">
                <xsl:text>char?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.Decimal&gt;'">
                <xsl:text>decimal?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.Double&gt;'">
                <xsl:text>double?</xsl:text>
            </xsl:when>
            <xsl:when test="$fullname = 'System.Nullable&lt;System.SByte&gt;'">
                <xsl:text>sbyte?</xsl:text>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$fullname"/>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:function>

    <!-- overall count functions -->
    <xsl:function name="ru:GetOverallTargetClassCount">
        <xsl:param name="rootMCR"/>
        <xsl:copy-of select="count( $rootMCR//InvolvedTypes/InvolvedType[@is-target = true()] )"/>
    </xsl:function>

    <xsl:function name="ru:GetOverallMixinCount">
        <xsl:param name="rootMCR"/>
        <xsl:copy-of
                select="count( $rootMCR//InvolvedTypes/InvolvedType[ @is-mixin = true() or @is-unusedmixin = true() ] )"/>
    </xsl:function>

    <xsl:function name="ru:GetOverallAssemblyCount">
        <xsl:param name="rootMCR"/>
        <xsl:copy-of select="count( $rootMCR//Assemblies/Assembly )"/>
    </xsl:function>


    <!-- tooltips for links -->
    <xsl:function name="ru:GetToolTip">
        <xsl:param name="rootMCR"/>
        <xsl:param name="item"/>

        <xsl:variable name="assemblyName"
                      select="if( $item/@assembly-ref = 'none' ) then 'external' else $rootMCR/key('assembly', $item/@assembly-ref)/@name"/>
        <xsl:copy-of select="concat($item/@namespace, ', ', $assemblyName)"/>
    </xsl:function>

    <!-- link generation templates -->
    <xsl:template name="GenerateGenericLink">
        <xsl:param name="rootMCR"/>
        <xsl:param name="id"/>
        <xsl:param name="keyName"/>
        <xsl:param name="dir"/>

        <xsl:variable name="item" select="$rootMCR/key($keyName, $id)"/>

        <xsl:if test="name($item) != 'Assembly'">
            <a href="{$dir}{$id}.html" title="{ru:GetToolTip($rootMCR, $item)}">
                <xsl:value-of select="$item/@name"/>
            </a>
        </xsl:if>
        <xsl:if test="name($item) = 'Assembly'">
            <a href="{$dir}{$id}.html">
                <xsl:value-of select="$item/@name"/>
            </a>
        </xsl:if>
    </xsl:template>

    <xsl:template name="GenerateAssemblyLink">
        <xsl:param name="rootMCR"/>
        <xsl:param name="assemblyId"/>
        <xsl:param name="dir"/>

        <xsl:if test="$assemblyId = 'none'">
            <xsl:text>external</xsl:text>
        </xsl:if>
        <xsl:if test="$assemblyId != 'none'">
            <xsl:call-template name="GenerateGenericLink">
                <xsl:with-param name="rootMCR" select="$rootMCR"/>
                <xsl:with-param name="id" select="$assemblyId"/>
                <xsl:with-param name="keyName">assembly</xsl:with-param>
                <xsl:with-param name="dir" select="concat($dir, '/assemblies/')"/>
            </xsl:call-template>
        </xsl:if>
    </xsl:template>

    <xsl:template name="GenerateInvolvedTypeLink">
        <xsl:param name="rootMCR"/>
        <xsl:param name="involvedTypeId"/>
        <xsl:param name="dir"/>

        <xsl:call-template name="GenerateGenericLink">
            <xsl:with-param name="rootMCR" select="$rootMCR"/>
            <xsl:with-param name="id" select="$involvedTypeId"/>
            <xsl:with-param name="keyName">involvedType</xsl:with-param>
            <xsl:with-param name="dir" select="concat($dir, '/involvedTypes/')"/>
        </xsl:call-template>
    </xsl:template>

    <xsl:template name="GenerateMixinReferenceLink">
        <xsl:param name="mixin"/>

        <xsl:variable name="item" select="key('involvedType', $mixin/@ref)"/>

        <a href="{$mixin/@ref}.html" title="{ru:GetToolTip(/, $item)}">
            <xsl:value-of select="$mixin/@instance-name"/>
        </a>
    </xsl:template>

    <xsl:template name="GenerateInterfaceLink">
        <xsl:param name="rootMCR"/>
        <xsl:param name="interfaceId"/>
        <xsl:param name="dir"/>

        <xsl:call-template name="GenerateGenericLink">
            <xsl:with-param name="rootMCR" select="$rootMCR"/>
            <xsl:with-param name="id" select="$interfaceId"/>
            <xsl:with-param name="keyName">interface</xsl:with-param>
            <xsl:with-param name="dir" select="concat($dir, '/interfaces/')"/>
        </xsl:call-template>

        <xsl:if test="@is-composed-interface = true()">
            <span class="composed-interface-annotation">[CI]</span>
        </xsl:if>
    </xsl:template>

    <xsl:template name="GenerateAttributeLink">
        <xsl:param name="rootMCR"/>
        <xsl:param name="attributeId"/>
        <xsl:param name="dir"/>

        <xsl:call-template name="GenerateGenericLink">
            <xsl:with-param name="rootMCR" select="$rootMCR"/>
            <xsl:with-param name="id" select="$attributeId"/>
            <xsl:with-param name="keyName">attribute</xsl:with-param>
            <xsl:with-param name="dir" select="concat($dir, '/attributes/')"/>
        </xsl:call-template>
    </xsl:template>


</xsl:stylesheet>
