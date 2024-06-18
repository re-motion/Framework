<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <xsl:template name="tableTemplate">
        <xsl:param name="rootMCR"/>
        <xsl:param name="items"/>
        <xsl:param name="dir"/>
        <xsl:param name="tableName"/>
        <xsl:param name="emptyText"/>
        <xsl:param name="caption">call 'tableTemplate' with-param 'caption'</xsl:param>
        <xsl:param name="target"/>


        <xsl:if test="count ( $items ) = 0">
            <div class="emptyText">
                <xsl:value-of select="$emptyText"/>
            </div>
        </xsl:if>

        <xsl:if test="count ( $items ) > 0">
            <!-- do not output "overlay" on non-index sites -->
            <xsl:if test="$dir = '.'">
                <div id="overlay">Please wait...</div>
            </xsl:if>
            <xsl:choose>
                <!-- assembly table (only on index site) -->
                <xsl:when test="$tableName = 'assemblyTable'">
                    <xsl:call-template name="assemblyTable"/>
                </xsl:when>
                <!-- involved type table -->
                <xsl:when test="$tableName = 'involvedTypeListTable'">
                    <xsl:call-template name="involvedTypeListTable">
                        <xsl:with-param name="rootMCR" select="$rootMCR"/>
                        <xsl:with-param name="involvedTypes" select="$items"/>
                        <xsl:with-param name="dir" select="$dir"/>
                        <xsl:with-param name="caption" select="$caption"/>
                    </xsl:call-template>
                </xsl:when>
                <!-- attribute table -->
                <xsl:when test="$tableName = 'interfaceListTable'">
                    <xsl:call-template name="interfaceListTable">
                        <xsl:with-param name="rootMCR" select="$rootMCR"/>
                        <xsl:with-param name="interfaces" select="$items"/>
                        <xsl:with-param name="dir" select="$dir"/>
                    </xsl:call-template>
                </xsl:when>
                <!-- attribute table -->
                <xsl:when test="$tableName = 'attributeListTable'">
                    <xsl:call-template name="attributeListTable">
                        <xsl:with-param name="rootMCR" select="$rootMCR"/>
                        <xsl:with-param name="attributes" select="$items"/>
                        <xsl:with-param name="dir" select="$dir"/>
                    </xsl:call-template>
                </xsl:when>
                <!-- attribute table -->
                <xsl:when test="$tableName = 'attributeRefListTable'">
                    <xsl:call-template name="attributeRefListTable">
                        <xsl:with-param name="rootMCR" select="$rootMCR"/>
                        <xsl:with-param name="attributeRefs" select="$items"/>
                        <xsl:with-param name="dir" select="$dir"/>
                    </xsl:call-template>
                </xsl:when>
                <!-- public members table -->
                <xsl:when test="$tableName = 'memberListTable'">
                    <xsl:call-template name="memberListTable">
                        <xsl:with-param name="members" select="$items"/>
                        <xsl:with-param name="rootMCR" select="$rootMCR"/>
                    </xsl:call-template>
                </xsl:when>
                <!-- mixin list at involved type site -->
                <xsl:when test="$tableName = 'mixinListTable'">
                    <xsl:call-template name="mixinListTable">
                        <xsl:with-param name="rootMCR" select="$rootMCR"/>
                        <xsl:with-param name="mixinRefs" select="$items"/>
                        <xsl:with-param name="dir" select="$dir"/>
                        <xsl:with-param name="target" select="$target"/>
                    </xsl:call-template>
                </xsl:when>
                <xsl:when test="$tableName = 'attributeArgumentListTable'">
                    <xsl:call-template name="attributeArgumentListTable">
                        <xsl:with-param name="rootMCR" select="$rootMCR"/>
                        <xsl:with-param name="arguments" select="$items"/>
                    </xsl:call-template>
                </xsl:when>

                <!-- fail fast -->
                <xsl:otherwise>
                    <xsl:message terminate="yes">table template rule '<xsl:value-of select="$tableName"/>' could not be
                        found
                    </xsl:message>
                </xsl:otherwise>
            </xsl:choose>

            <!--
            <xsl:if test="$dir = '.'">
              <div class="pager">
                <form action="">
                  <div>
                    <img src="{$dir}/resources/images/first.png" class="first" alt="First"/>
                    <img src="{$dir}/resources/images/prev.png" class="prev" alt="Previous"/>
                    <input type="text" class="pagedisplay" size="8"/>
                    <img src="{$dir}/resources/images/next.png" class="next" alt="Next"/>
                    <img src="{$dir}/resources/images/last.png" class="last" alt="Last"/>
                    <select class="pagesize">
                      <option value="10">10</option>
                      <option value="20">20</option>
                      <option value="30">30</option>
                      <option  value="50">50</option>
                      <option  value="100">100</option>
                      <option  value="32000">all</option>
                    </select>
                  </div>
                </form>
              </div>
            </xsl:if>
            -->
        </xsl:if>

    </xsl:template>

</xsl:stylesheet>
