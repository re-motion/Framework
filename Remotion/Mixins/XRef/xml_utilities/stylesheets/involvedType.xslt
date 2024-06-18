<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <xsl:template name="involvedType">

        <xsl:call-template name="htmlSite">
            <xsl:with-param name="siteTitle">Mixin Index</xsl:with-param>
            <xsl:with-param name="siteFileName">mixin_index.html</xsl:with-param>
            <xsl:with-param name="bodyContentTemplate">involvedTypeMixin</xsl:with-param>
        </xsl:call-template>

        <xsl:call-template name="htmlSite">
            <xsl:with-param name="siteTitle">Target Class Index</xsl:with-param>
            <xsl:with-param name="siteFileName">target_index.html</xsl:with-param>
            <xsl:with-param name="bodyContentTemplate">involvedTypeTarget</xsl:with-param>
        </xsl:call-template>

        <xsl:for-each select="/MixinXRefReport/InvolvedTypes/InvolvedType">
            <xsl:variable name="classTypeName" select="if( @is-target = true() and @is-mixin = true() ) then 'Mixed Mixin'
                else if ( @is-target = true() ) then 'Target Class'
                else 'Mixin' "/>

            <!-- generate involved type detail site -->
            <xsl:call-template name="involvedTypeDetailSite">
                <xsl:with-param name="title" select="@name"/>
                <xsl:with-param name="type" select="$classTypeName"/>
            </xsl:call-template>
        </xsl:for-each>
    </xsl:template>


    <xsl:template name="involvedTypeMixin">
        <xsl:call-template name="involvedTypeList">
            <xsl:with-param name="rootMCR" select="/"/>
            <xsl:with-param name="involvedTypes"
                            select="/MixinXRefReport/InvolvedTypes/InvolvedType[ @is-mixin = true() or @is-unusedmixin = true() ]"/>
            <xsl:with-param name="dir">.</xsl:with-param>
            <xsl:with-param name="caption">Mixins</xsl:with-param>
            <xsl:with-param name="emptyText">No&#160;Mixins</xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template name="involvedTypeTarget">
        <xsl:call-template name="involvedTypeList">
            <xsl:with-param name="rootMCR" select="/"/>
            <xsl:with-param name="involvedTypes"
                            select="/MixinXRefReport/InvolvedTypes/InvolvedType[@is-target = true() ]"/>
            <xsl:with-param name="dir">.</xsl:with-param>
            <xsl:with-param name="caption">Target&#160;Classes</xsl:with-param>
            <xsl:with-param name="emptyText">No&#160;Target&#160;Classes</xsl:with-param>
        </xsl:call-template>
    </xsl:template>


    <xsl:template name="involvedTypeDetailSite">
        <xsl:param name="title"/>
        <xsl:param name="type"/>
        <xsl:call-template name="htmlSite">
            <xsl:with-param name="siteTitle">
                <xsl:value-of select="$title"/> (<xsl:value-of select="$type"/>)
            </xsl:with-param>
            <xsl:with-param name="siteFileName">involvedTypes/<xsl:value-of select="@id"/>.html
            </xsl:with-param>
            <xsl:with-param name="bodyContentTemplate">involvedTypeDetail</xsl:with-param>
        </xsl:call-template>
    </xsl:template>

    <xsl:template name="involvedTypeDetail">

        <xsl:variable name="classTypeName" select="if( @is-target = true() and @is-mixin = true() ) then 'Mixed Mixin'
                else if ( @is-target = true() ) then 'Target Class'
                else 'Mixin' "/>
        <xsl:variable name="classTypeIndexLink" select="if ( @is-mixin = true() ) then 'mixin' else 'target' "/>

        <h1>
            <xsl:value-of select="@name"/>
        </h1>
        <h2>
            <a href="../{$classTypeIndexLink}_index.html">[<xsl:value-of select="$classTypeName"/>]
            </a>
        </h2>
        <xsl:variable name="children" select="/MixinXRefReport/InvolvedTypes/InvolvedType[@base-ref = current()/@id]"/>

        <fieldset>
            <legend>Summary</legend>

            <div>
                <label>Modifiers:</label>
                <xsl:apply-templates select="Modifiers"/>
                <div class="clean"/>
            </div>
            <div>
                <label>Namespace:</label>
                <xsl:value-of select="@namespace"/>
            </div>
            <div>
                <label>Assembly:</label>
                <xsl:call-template name="GenerateAssemblyLink">
                    <xsl:with-param name="rootMCR" select="/"/>
                    <xsl:with-param name="assemblyId" select="@assembly-ref"/>
                    <xsl:with-param name="dir">..</xsl:with-param>
                </xsl:call-template>
            </div>
            <div>
                <label>Base:</label>
                <xsl:call-template name="involvedTypeBaseLink"/>
            </div>

            <!--
            <div>
              <label>Children:</label>
              <p class="involvedType-interfaceLinkList">

              <xsl:for-each select="$children">
                <xsl:sort select="@name"/>
                      <xsl:if test="position() != 1">, </xsl:if>
                <xsl:call-template name="GenerateInvolvedTypeLink">
                  <xsl:with-param name="rootMCR" select="/" />
                  <xsl:with-param name="involvedTypeId" select="@id" />
                  <xsl:with-param name="dir">..</xsl:with-param>
                </xsl:call-template>
                  </xsl:for-each>
              <xsl:if test="empty($children)">-</xsl:if>
              </p>
            </div>
            -->
            <div>
                <label>Interfaces:</label>
                <p class="involvedType-interfaceLinkList">
                    <xsl:for-each select="ImplementedInterfaces/ImplementedInterface">
                        <xsl:sort select="/MixinXRefReport/Interfaces/Interface[@id = current()/@ref]/@name"/>
                        <xsl:if test="position() != 1">,</xsl:if>
                        <xsl:call-template name="GenerateInterfaceLink">
                            <xsl:with-param name="rootMCR" select="/"/>
                            <xsl:with-param name="interfaceId" select="@ref"/>
                            <xsl:with-param name="dir">..</xsl:with-param>
                        </xsl:call-template>
                    </xsl:for-each>
                </p>
                <xsl:if test="empty(ImplementedInterfaces/ImplementedInterface)">-</xsl:if>
            </div>
            <div>
                <label>Mixins applied:</label>
                <xsl:value-of select="count( Mixins/Mixin )"/>
            </div>

            <div>
                <label>Applied to:</label>
                <xsl:value-of select="count( Targets/Target )"/>
            </div>

            <xsl:if test="@is-generic-definition = true() and @is-target = true()">
                <div>
                    <span class="dubiosInvolvedType">This type is a generic definition. Therefore detailed Mixin
                        information is not available.
                    </span>
                </div>
            </xsl:if>

            <xsl:if test="@is-interface = true() and @is-target = true()">
                <div>
                    <span class="dubiosInvolvedType">This type is an interface. Therefore detailed Mixin information is
                        not available.
                    </span>
                </div>
            </xsl:if>

            <div class="involvedType-summary">
                <xsl:apply-templates select="summary"/>
            </div>
        </fieldset>

        <xsl:if test="count($children)>0">
            <xsl:call-template name="genericTreeBuilder">
                <xsl:with-param name="caption">Inheritors</xsl:with-param>
                <xsl:with-param name="treeNodes" select="."/>
                <xsl:with-param name="allReferences" select="/MixinXRefReport/InvolvedTypes/InvolvedType"/>
            </xsl:call-template>
        </xsl:if>
        <xsl:if test="count($children)=0">
            <div class="emptyText">No Inheritors</div>
        </xsl:if>

        <xsl:call-template name="mixinList">
            <xsl:with-param name="involvedType" select="."/>
        </xsl:call-template>

        <xsl:if test="@is-mixin = true()">
            <xsl:call-template name="treeBuilder">
                <xsl:with-param name="caption">Targets&#160;(<xsl:value-of select="count( Targets/Target )"/>)
                </xsl:with-param>
                <xsl:with-param name="treeNodes"
                                select="/MixinXRefReport/InvolvedTypes/InvolvedType[ (ru:contains(current()/Targets/Target/@ref, @id))]"/>
                <xsl:with-param name="allReferences" select="Targets/Target/@ref"/>
            </xsl:call-template>
        </xsl:if>


        <xsl:if test="@is-mixin = false()">
            <div class="emptyText">No Targets</div>
        </xsl:if>

        <xsl:call-template name="attributeRefList">
            <xsl:with-param name="rootMCR" select="/"/>
            <xsl:with-param name="attributeRefs" select="HasAttributes/HasAttribute"/>
            <xsl:with-param name="dir">..</xsl:with-param>
        </xsl:call-template>

        <xsl:call-template name="memberList">
            <!-- summaries may contain other tags, eg. 'cref' and content -->
            <xsl:with-param name="members" select="Members/Member"/>
            <xsl:with-param name="rootMCR" select="/"/>
        </xsl:call-template>

    </xsl:template>


    <xsl:template match="InvolvedType/summary/*">
        <xsl:text> </xsl:text>
        <b>
            <xsl:value-of select="."/>
            <xsl:value-of select="substring(@*, 3)"/>
        </b>
        <xsl:text> </xsl:text>
    </xsl:template>

    <xsl:template name="involvedTypeBaseLink">
        <xsl:if test="@base-ref != 'none' ">
            <a href="{@base-ref}.html" title="{ru:GetToolTip (/, key('involvedType', @base-ref) )}">
                <xsl:value-of select="@base"/>
            </a>
        </xsl:if>
        <xsl:if test="@base-ref = 'none' ">
            <xsl:value-of select="@base"/>
        </xsl:if>
    </xsl:template>

</xsl:stylesheet>
