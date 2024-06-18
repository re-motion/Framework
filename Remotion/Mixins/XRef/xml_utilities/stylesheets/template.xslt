<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <xsl:template name="htmlSite">
        <xsl:param name="siteTitle"/>
        <xsl:param name="siteFileName"/>
        <xsl:param name="bodyContentTemplate"/>
        <!-- if sitename contains a path seperator then this is a index file -->
        <xsl:variable name="dir" select=" if( contains($siteFileName, '/') ) then '..' else '.' "/>


        <xsl:result-document format="standardHtmlOutputFormat" href="{$siteFileName}">
            <html xmlns="http://www.w3.org/1999/xhtml">
                <head>
                    <title>
                        <xsl:value-of select="$siteTitle"/>
                    </title>
                    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
                    <!-- include resources -->
                    <xsl:call-template name="includeResources">
                        <xsl:with-param name="dir" select="$dir"/>
                    </xsl:call-template>
                </head>
                <body>

                    <!-- navigation bar -->
                    <xsl:call-template name="navigation">
                        <xsl:with-param name="dir" select="$dir"/>
                    </xsl:call-template>

                    <!-- list of all callable templates -->
                    <xsl:choose>
                        <!-- index -->
                        <xsl:when test="$bodyContentTemplate = 'index'">
                            <xsl:call-template name="index"/>
                        </xsl:when>
                        <!-- assembly index + assembly sites -->
                        <xsl:when test="$bodyContentTemplate = 'assembly'">
                            <xsl:call-template name="assembly"/>
                        </xsl:when>
                        <!-- assembly detail site -->
                        <xsl:when test="$bodyContentTemplate = 'assemblyDetail'">
                            <xsl:call-template name="assemblyDetail"/>
                        </xsl:when>
                        <!-- mixin index -->
                        <xsl:when test="$bodyContentTemplate = 'involvedTypeMixin'">
                            <xsl:call-template name="involvedTypeMixin"/>
                        </xsl:when>
                        <!-- target index -->
                        <xsl:when test="$bodyContentTemplate = 'involvedTypeTarget'">
                            <xsl:call-template name="involvedTypeTarget"/>
                        </xsl:when>
                        <!-- involved type detail site -->
                        <xsl:when test="$bodyContentTemplate = 'involvedTypeDetail'">
                            <xsl:call-template name="involvedTypeDetail"/>
                        </xsl:when>
                        <!-- interface index + interface sites -->
                        <xsl:when test="$bodyContentTemplate = 'interface'">
                            <xsl:call-template name="interface"/>
                        </xsl:when>
                        <!-- interface detail site -->
                        <xsl:when test="$bodyContentTemplate = 'interfaceDetail'">
                            <xsl:call-template name="interfaceDetail"/>
                        </xsl:when>
                        <!-- attribute index + attribute sites -->
                        <xsl:when test="$bodyContentTemplate = 'attribute'">
                            <xsl:call-template name="attribute"/>
                        </xsl:when>
                        <!-- attribute detail site -->
                        <xsl:when test="$bodyContentTemplate = 'attributeDetail'">
                            <xsl:call-template name="attributeDetail"/>
                        </xsl:when>

                        <!-- fail fast -->
                        <xsl:otherwise>
                            <xsl:message terminate="yes">site template rule '<xsl:value-of
                                    select="$bodyContentTemplate"/>' could not be found
                            </xsl:message>
                        </xsl:otherwise>
                    </xsl:choose>

                </body>
            </html>
        </xsl:result-document>
    </xsl:template>

    <xsl:template name="includeResources">
        <xsl:param name="dir"/>

        <link rel="shortcut icon" type="image/x-icon" href="{$dir}/resources/images/favicon.ico"/>
        <link rel="stylesheet" type="text/css" href="{$dir}/resources/style.css"/>
        <link rel="stylesheet" type="text/css" href="{$dir}/resources/css/table_jui.css"/>

        <script type="text/javascript" src="{$dir}/resources/javascript/jquery-1.4.2.min.js"></script>
        <script type="text/javascript" src="{$dir}/resources/javascript/jquery.dataTables.min.js"></script>

        <xsl:if test="$dir = '.'">
            <link rel="stylesheet" type="text/css" href="{$dir}/resources/css/jquery-ui-1.7.2.custom.css"/>
        </xsl:if>
        <xsl:if test="$dir = '..'">
            <link rel="stylesheet" type="text/css" href="{$dir}/resources/css/jquery-ui-1.7.2.simple.css"/>
            <script type="text/javascript" src="{$dir}/resources/javascript/jquery.cookie.js"></script>
            <script type="text/javascript" src="{$dir}/resources/javascript/jquery.treeview.js"></script>
            <script type="text/javascript" src="{$dir}/resources/javascript/jquery.color.js"></script>
        </xsl:if>

        <script type="text/javascript" src="{$dir}/resources/init.js"></script>
    </xsl:template>

    <xsl:template name="navigation">
        <xsl:param name="dir"/>

        <ul id="navigation">
            <li>
                <a href="{$dir}/index.html">Summary</a>
            </li>
            <li>
                <a href="{$dir}/assembly_index.html">Assemblies</a>
            </li>
            <li>
                <a href="{$dir}/mixin_index.html">Mixins</a>
            </li>
            <li>
                <a href="{$dir}/target_index.html">Target Classes</a>
            </li>
            <li>
                <a href="{$dir}/interface_index.html">Involved Interfaces</a>
            </li>
            <li>
                <a href="{$dir}/attribute_index.html">Involved Attributes</a>
            </li>
        </ul>
    </xsl:template>

</xsl:stylesheet>
