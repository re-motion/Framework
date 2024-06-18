<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:ru="http://www.rubicon-it.com"
                version="2.0" xmlns="http://www.w3.org/1999/xhtml"
                exclude-result-prefixes="xs fn ru">

    <xsl:template name="errorList">

        <xsl:if test="count(//Exception) = 0">
            <p class="errorList-noErrors">No Errors were found. (Mixin Configuration Errors, Validation Errors)</p>
        </xsl:if>

        <xsl:if test="count(//Exception) > 0">
            <p class="errorList-errorsFound">
                <xsl:value-of select="count(//Exception)"/> error(s) detected!
            </p>
        </xsl:if>


        <xsl:if test="count(//ConfigurationErrors/Exception) > 0">
            <div>
                <span class="treeHeader">
                    Configuration Errors (<xsl:value-of select="count(//ConfigurationErrors/Exception)"/>)
                </span>
            </div>
            <xsl:call-template name="showExceptions">
                <xsl:with-param name="exceptions" select="//ConfigurationErrors/Exception"/>
            </xsl:call-template>

        </xsl:if>

        <xsl:if test="count(//ValidationErrors/Exception) > 0">
            <div>
                <span class="treeHeader">
                    Validation Errors (<xsl:value-of select="count(//ValidationErrors/Exception)"/>)
                </span>
            </div>


            <xsl:call-template name="showExceptions">
                <xsl:with-param name="exceptions" select="//ValidationErrors/Exception"/>
            </xsl:call-template>

        </xsl:if>

    </xsl:template>

    <xsl:template name="showExceptions">
        <xsl:param name="exceptions"/>

        <div class="treeview">
            <xsl:for-each select="$exceptions">
                <fieldset>
                    <legend>
                        <xsl:value-of select="@type"/>
                    </legend>
                    <xsl:if test="exists(ValidationLog/@number-of-rules-executed)">
                        <div>
                            <label># of rules executed:</label>
                            <xsl:value-of select="ValidationLog/@number-of-rules-executed"/>
                        </div>
                        <div>
                            <label># of failures:</label>
                            <xsl:value-of select="ValidationLog/@number-of-failures"/>
                        </div>
                        <div>
                            <label># of unexp. excep.:</label>
                            <xsl:value-of select="ValidationLog/@number-of-unexpected-exceptions"/>
                        </div>
                        <div>
                            <label># of warnings:</label>
                            <xsl:value-of select="ValidationLog/@number-of-warnings"/>
                        </div>
                        <div>
                            <label># of successes:</label>
                            <xsl:value-of select="ValidationLog/@number-of-successes"/>
                        </div>
                    </xsl:if>
                    <div>
                        <label class="message">Message:</label>
                        <br/>
                        <p class="errorMessage">
                            <xsl:value-of select="Message"/>
                        </p>
                    </div>
                    <div>
                        <label>StackTrace:</label>
                        <br/>
                        <pre>
                            <xsl:value-of select="StackTrace"/>
                        </pre>
                    </div>
                </fieldset>

            </xsl:for-each>
        </div>

    </xsl:template>

</xsl:stylesheet>