<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
        xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        xmlns:xs="http://www.w3.org/2001/XMLSchema"
        xmlns:fn="http://www.w3.org/2005/xpath-functions"
        xmlns:ru="http://www.rubicon-it.com"
        version="2.0"
        xmlns="http://www.w3.org/1999/xhtml"
        exclude-result-prefixes="xs fn ru"
>

    <xsl:output
            name="standardHtmlOutputFormat"
            method="html"
            indent="yes"
            omit-xml-declaration="yes"
            doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
            doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"
    />


    <!-- keys -->
    <xsl:key name="assembly" match="//Assemblies/Assembly" use="@id"/>
    <xsl:key name="involvedType" match="//InvolvedTypes/InvolvedType" use="@id"/>
    <xsl:key name="interface" match="//Interfaces/Interface" use="@id"/>
    <xsl:key name="attribute" match="//Attributes/Attribute" use="@id"/>

    <!-- include utilities -->
    <xsl:include href="stylesheets/template.xslt"/>
    <xsl:include href="stylesheets/tableTemplate.xslt"/>
    <xsl:include href="stylesheets/utility.xslt"/>

    <!-- include sub stylesheets for sites -->
    <xsl:include href="stylesheets/index.xslt"/>
    <xsl:include href="stylesheets/assembly.xslt"/>
    <xsl:include href="stylesheets/involvedType.xslt"/>
    <xsl:include href="stylesheets/interface.xslt"/>
    <xsl:include href="stylesheets/attribute.xslt"/>

    <!-- component stylesheets -->
    <xsl:include href="stylesheets/involvedTypeList.xslt"/>
    <xsl:include href="stylesheets/memberList.xslt"/>
    <xsl:include href="stylesheets/interfaceList.xslt"/>
    <xsl:include href="stylesheets/attributeList.xslt"/>
    <xsl:include href="stylesheets/attributeRefList.xslt"/>
    <xsl:include href="stylesheets/errorList.xslt"/>
    <xsl:include href="stylesheets/treeBuilder.xslt"/>
    <xsl:include href="stylesheets/mixinList.xslt"/>


    <!-- 'main' template -->
    <xsl:template match="/">
        <!-- main index (summary) -->
        <xsl:call-template name="htmlSite">
            <xsl:with-param name="siteTitle">Mixin Documentation Index</xsl:with-param>
            <xsl:with-param name="siteFileName">index.html</xsl:with-param>
            <xsl:with-param name="bodyContentTemplate">index</xsl:with-param>
        </xsl:call-template>
        <!-- assembly index + assembly sites -->
        <xsl:call-template name="htmlSite">
            <xsl:with-param name="siteTitle">Assembly Index</xsl:with-param>
            <xsl:with-param name="siteFileName">assembly_index.html</xsl:with-param>
            <xsl:with-param name="bodyContentTemplate">assembly</xsl:with-param>
        </xsl:call-template>
        <!-- involved type index (now seperate mixin/target site, therefor no 'new' page) + involved type sites -->
        <xsl:call-template name="involvedType"/>
        <!-- interface index + interface sites -->
        <xsl:call-template name="htmlSite">
            <xsl:with-param name="siteTitle">Interface Index</xsl:with-param>
            <xsl:with-param name="siteFileName">interface_index.html</xsl:with-param>
            <xsl:with-param name="bodyContentTemplate">interface</xsl:with-param>
        </xsl:call-template>
        <!-- attribute index + attribute sites -->
        <xsl:call-template name="htmlSite">
            <xsl:with-param name="siteTitle">Attribute Index</xsl:with-param>
            <xsl:with-param name="siteFileName">attribute_index.html</xsl:with-param>
            <xsl:with-param name="bodyContentTemplate">attribute</xsl:with-param>
        </xsl:call-template>
    </xsl:template>

</xsl:stylesheet>
