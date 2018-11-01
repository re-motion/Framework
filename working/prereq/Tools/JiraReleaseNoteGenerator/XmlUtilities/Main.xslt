<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
	version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:ru="http://www.rubicon-it.com"
  xmlns:functx="http://www.functx.com"
	exclude-result-prefixes="xs fn ru functx"
	>

  <xsl:output
	name="standardHtmlOutputFormat"
	method="html"
	indent="no"
	omit-xml-declaration="yes"
	doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"
    />

  <xsl:output
	name="emptyFileFormat"
	method="text"
	indent="no"
	omit-xml-declaration="yes"
    />

  <xsl:template match="/">
    <xsl:variable name="issues" select="/rss/channel/item"/>
    <xsl:variable name="root" select="/"/>
    
    <xsl:choose>
      <xsl:when test ="(count(functx:value-intersect($issues/status, $root//issueVisibility/visibleStatus)) > 0 or count($root//rss/channel/item[parent = $issues/key and ru:contains($root//issueVisibility/visibleStatus, status)]) > 0) and (count(functx:value-intersect($issues/resolution, $root//issueVisibility/visibleResolution)) > 0 or count($root//rss/channel/item[parent = $issues/key and ru:contains($root//issueVisibility/visibleResolution, resolution)]) > 0)">
        <xsl:call-template name="htmlSite">
          <xsl:with-param name="siteTitle" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:result-document format="emptyFileFormat"></xsl:result-document>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:function name="functx:is-value-in-sequence" as="xs:boolean" >
    <xsl:param name="value" as="xs:anyAtomicType?"/>
    <xsl:param name="seq" as="xs:anyAtomicType*"/>

    <xsl:sequence select="$value = $seq"/>
  </xsl:function>

  <xsl:function name="functx:value-intersect" as="xs:anyAtomicType*" >
    <xsl:param name="arg1" as="xs:anyAtomicType*"/>
    <xsl:param name="arg2" as="xs:anyAtomicType*"/>

    <xsl:sequence select="distinct-values($arg1[.=$arg2])"/>
  </xsl:function>


  <xsl:function name="ru:contains">
    <xsl:param name="sequence" />
    <xsl:param name="searchItem" />

    <xsl:copy-of select=" exists( index-of( $sequence, $searchItem ) )" />
  </xsl:function>

  <xsl:template name="htmlSite">
    <xsl:param name="siteTitle" />
    <xsl:result-document format="standardHtmlOutputFormat">
      <html xmlns="http://www.w3.org/1999/xhtml">
        <head>
          <title>
            <xsl:value-of select="$siteTitle" />
          </title>
          <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
          <!-- include resources -->
          <style type="text/css">
            body
            {
            background-image: url(images/bg.png);
            background-position: top left;
            background-repeat: repeat-x;

            margin-top: 15px;
            font-family: "Trebuchet MS", Arial, Verdana, Helvectica;
            font-size: 0.9em;
            overflow:scroll;
            }

            a
            {
            color: #00327d;
            text-decoration: none;
            }

            a:hover
            {
            text-decoration: underline;
            }

            h2{
            text-decoration:underline;
            }

            .detailNotVisible, .detailNotVisible h4, .listEntry .notVisible a
            {
            color: Gray;
            }

            .children
            {
            margin-left:30px;
            }

            .label
            {
            min-width: 11em;
            font-weight: bold;
            }

            .detailEntry, .listEntryGroup
            {
            margin-bottom: 20px;
            padding: 10px;
            border: dashed 1px #999999;
            background-color: #EEEEEE;
            }

            .releaseNoteList
            {
            padding-bottom: 30px;
            }

            .description, .component, .issueType, .status, .resolution, .fixVersion
            {
            margin-left:10px;
            }

            h4
            {
            margin-top: 0px;
            color: #000080;
            }
          </style>
        </head>
        <body>
          <h1>
            <xsl:value-of select="/rss/outputConfiguration/projectTitle"/> version <xsl:value-of select="/rss/outputConfiguration/generatedForVersion"/>
          </h1>
          <h2>List of Issues</h2>
          <div class="releaseNoteList">
            <xsl:call-template name="printConfiguredTypes">
              <xsl:with-param name="outputType" select="'list'"/>
            </xsl:call-template>
          </div>

          <h2>
            Details
          </h2>
          <xsl:call-template name="printConfiguredTypes">
            <xsl:with-param name="outputType" select="'details'"/>
          </xsl:call-template>
        </body>
      </html>
    </xsl:result-document>
  </xsl:template>

  <xsl:template name="printConfiguredTypes">
    <xsl:param name="outputType" />

    <xsl:for-each select="/rss/outputConfiguration/issueOrder/issue">

      <xsl:variable name="selectingType" select="type"/>

      <xsl:if test="$outputType = 'list'">

        <xsl:call-template name="issueListForType">
          <xsl:with-param name="root" select="/" />
          <xsl:with-param name="issues" select="/rss/channel/item[type=$selectingType]"/>
          <xsl:with-param name="visibleStatus" select="current()"/>
          <xsl:with-param name="title" select="title"/>
        </xsl:call-template>

      </xsl:if>

      <xsl:if test="$outputType = 'details'">
        <xsl:call-template name="issueDetailsForType">
          <xsl:with-param name="root" select="/" />
          <xsl:with-param name="issues" select="/rss/channel/item[type=$selectingType]"/>
          <xsl:with-param name="visibleStatus" select="current()"/>
        </xsl:call-template>
      </xsl:if>

    </xsl:for-each>
  </xsl:template>

  <xsl:template name="issueListForType">
    <xsl:param name="root" />
    <xsl:param name="issues" />
    <xsl:param name="visibleStatus" />
    <xsl:param name="title" />

    <xsl:if test="count(functx:value-intersect($issues/status, $root//issueVisibility/visibleStatus)) > 0 or count($root//rss/channel/item[parent = $issues/key and ru:contains($root//issueVisibility/visibleStatus, status)]) > 0">
      <xsl:if test="count(functx:value-intersect($issues/resolution, $root//issueVisibility/visibleResolution)) > 0 or count($root//rss/channel/item[parent = $issues/key and ru:contains($root//issueVisibility/visibleResolution, resolution)]) > 0">
        <h3>
          <xsl:value-of select="$title"/>
        </h3>
        
        <div class="listEntryGroup">
        
          <xsl:for-each select="$issues">
            <!-- select="functx:is-value-in-sequence(current()/status, $root//rss/channel/item[parent = current()/key]/status)" -->
        
            <xsl:variable name="hasValidChildren" select="count($root//rss/channel/item[parent = current()/key and ru:contains($root//issueVisibility/visibleStatus, status) and ru:contains($root//issueVisibility//visibleResolution, resolution)]) > 0" />
        
            <div class="listEntry">
              <xsl:if test="functx:is-value-in-sequence(status, $root//issueVisibility/visibleStatus) = true() and functx:is-value-in-sequence(resolution, $root//issueVisibility/visibleResolution) = true() and exists(invisible) = false()">
                <a href="#{key}">
                  <b>
                    [<xsl:value-of select="key"/>] <xsl:value-of select="component"/>:
                  </b>
                  <xsl:value-of select="summary"/>
                </a>
              </xsl:if>
              <xsl:if test="(functx:is-value-in-sequence(status, $root//issueVisibility/visibleStatus) = false() or functx:is-value-in-sequence(resolution, $root//issueVisibility/visibleResolution) = false() or  exists(invisible) = true() )and $hasValidChildren = true()">
                <span class="notVisible">
                  <a href="#{key}">
                    <b>
                      [<xsl:value-of select="key"/>] <xsl:value-of select="component"/>:
                    </b>
                    <xsl:value-of select="summary"/>
                  </a>
                </span>
              </xsl:if>
            </div>
        
            <xsl:if test="$hasValidChildren = true()">
              <xsl:call-template name="listChildren">
                <xsl:with-param name="root" select="$root" />
                <xsl:with-param name="key" select="key" />
              </xsl:call-template>
            </xsl:if>
        
          </xsl:for-each>
        </div>
      </xsl:if>
    </xsl:if>
  </xsl:template>

  <xsl:template name="issueDetailsForType">
    <xsl:param name="root" />
    <xsl:param name="issues" />
    <xsl:param name="class" />
    <xsl:param name="visibleStatus" />

    <xsl:for-each select="$issues">
      <xsl:variable name="hasValidChildren" select="count($root//rss/channel/item[parent = current()/key and ru:contains($root//issueVisibility/visibleStatus, status) and ru:contains($root//issueVisibility/visibleResolution, resolution)]) > 0" />
      <xsl:if test="functx:is-value-in-sequence(status, $root//issueVisibility/visibleStatus) = true() or functx:is-value-in-sequence(resolution, $root//issueVisibility/visibleResolution) = true() or $hasValidChildren = true()">
        <xsl:variable name="visibilityTag">
          <xsl:if test="functx:is-value-in-sequence(status, $root//issueVisibility/visibleStatus) = false() or functx:is-value-in-sequence(resolution, $root//issueVisibility/visibleResolution) = false() or exists(invisible) = true()">
            detailNotVisible
          </xsl:if>
        </xsl:variable>

        <div class="detailEntry {$class} {$visibilityTag}">

          <a name="{key}"/>
          <h4>
            <xsl:value-of select="title"/>
          </h4>

          <div class="component">
            <span class="label">Component/s: </span>
            <span class="value">
              <xsl:value-of select="component"/>
            </span>
          </div>
          <div class="issueType">
            <span class="label">Issue Type: </span>
            <span class="value">
              <xsl:value-of select="type"/>
            </span>
          </div>

          <div class="resolution">
            <span class="label">Resolution: </span>
            <span class="value">
              <xsl:value-of select="resolution"/>
            </span>
          </div>

          <!--
          <xsl:if test="status != 'Closed'">
          -->
          <div class="status">
            <span class="label">Status: </span>
            <span class="value">
              <xsl:value-of select="status"/>
            </span>
          </div>
          <!--
          </xsl:if>
          -->

          <xsl:if test="functx:is-value-in-sequence(status, $root//issueVisibility/visibleStatus) = false() or functx:is-value-in-sequence(resolution, $root//issueVisibility/visibleResolution) = false() or exists(invisible) = true()">
            <div class="fixVersion">
              <span class="label">FixVersion: </span>
              <span class="value">
                <xsl:value-of select="fixVersion"/>
              </span>
            </div>
          </xsl:if>
          <br/>
          <div class="description">
            <xsl:if test="description = ''">
              (no description)
            </xsl:if>
            <span>
              <xsl:value-of select="description" disable-output-escaping="yes"/>
            </span>
          </div>
        </div>

        <xsl:if test="$hasValidChildren = true()">
          <xsl:call-template name="detailsForChildren">
            <xsl:with-param name="root" select="$root" />
            <xsl:with-param name="key" select="key" />
            <xsl:with-param name="visibleStatus" select="$visibleStatus" />
          </xsl:call-template>
        </xsl:if>

      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="detailsForIssue">
    <xsl:param name="root" />
    <xsl:param name="class" />
    <xsl:param name="visibilityTag" />
    <xsl:param name="visibleStatus" />
    <xsl:param name="hasChildren" />

  </xsl:template>

  <xsl:template name="listChildren">
    <xsl:param name="root"/>
    <xsl:param name="key"/>

    <xsl:for-each select="$root//rss/channel/item[parent = $key]">
      <div class="children">
        <a href="#{key}">
          <b>
            [<xsl:value-of select="key"/>] <xsl:value-of select="component"/>:
          </b>
          <xsl:value-of select="summary"/>
        </a>
      </div>
    </xsl:for-each>

  </xsl:template>

  <xsl:template name="detailsForChildren">
    <xsl:param name="root"/>
    <xsl:param name="key"/>
    <xsl:param name="visibleStatus"/>

    <xsl:call-template name="issueDetailsForType">
      <xsl:with-param name="root" select="/" />
      <xsl:with-param name="issues" select="$root//rss/channel/item[parent = $key]"/>
      <xsl:with-param name="class" select="'children'" />
      <xsl:with-param name="visibleStatus" select="$visibleStatus" />
    </xsl:call-template>
  </xsl:template>

</xsl:stylesheet>