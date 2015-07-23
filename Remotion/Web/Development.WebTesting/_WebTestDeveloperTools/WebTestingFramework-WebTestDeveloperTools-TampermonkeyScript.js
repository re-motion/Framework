// ==UserScript==
// @name         WebTestingFramework Development Tools
// @namespace    http://www.re-motion.org/
// @version      1.1
// @description  Displays relevant data-* attributes for the WebTestingFramework. Adapt match argument to your needs.
// @author       Dominik Rauch
// @match        localhost/ActaNova_trunk_com/web/*
// @match        localhost/ActaNova_trunk_fed/web/*
// @match        localhost/ActaNova_trunk_mun/web/*
// @match        w0019/pad/web/*
// @match        localhost/pad/web/*
// @match        w0019/padng_test/web/*
// @match        localhost/padng_test/web/*
// @grant        none
// ==/UserScript==

// constants
var modeOff = 0;
var modeFollow = 1;
var modeShow = 2;
var numberOfModes = 3;

String.prototype.endsWith = function(suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

// variables (global is not optimal, however, user scripts are isolated)
var mode = modeOff;
var savedElem = null;
var savedAbsX = -1;
var savedAbsY = -1;
var savedFrameX = -1;
var savedFrameY = -1;

// startup
if(window.jQuery) {
    $(function() {
        if(!areWeExecutedInFrame()) {
            init();
        } else {
            initFrame();
        }
    });
}

function init () {
    console.log('Starting WebTestingFramework development tools...');
    
    $('body').append('<div id="dmadiv" style="position:absolute;top:0;left:0;padding:0.5em;z-index:10000;background-color:white;display:none;"><table><tr><td><span id="dmadivFrameIndicator"></span></td><td style="text-align:right"><button id="dmadivCloseButton">Close</button></td></tr><tr><td colspan="2"><div id="dmadivContent"></div></td></tr></table></div>');

    $('#dmadivCloseButton').click(mainOnCloseButtonClick);    
    $(document).mousemove(mainOnMousemove);    
    $(document).one('keydown', mainOnKeydown);    
    $(document).keyup(mainOnKeyup);    
    window.addEventListener("message", mainOnMessage, false);
    
    console.log('WebTestingFramework development tools are up & running.');
}

function initFrame () {
    console.log('Adding frame to WebTestingFramework development tools...');
    
    $(document).mousemove(frameOnMousemove);    
    $(document).one('keydown', frameOnKeydown);    
    $(document).keyup(frameOnKeyup);
        
    console.log('Frame added.');
}

// event handlers
function mainOnCloseButtonClick () {
    mode = modeOff;
    updateMode();
}

function mainOnMousemove (e) {
    handleMousemove(e.pageX, e.pageY, null, null);
}

function mainOnKeydown (e) {
    this.onkeydown = null;
        
    if(e.which !== 18) // ALT key
        return;
    
    toggleMode();
}

function mainOnKeyup (e) {
    this.onkeydown = mainOnKeydown;
  	e.preventDefault();
}

function mainOnMessage (e) {
    var msg = e.data;
    
    if(msg.indexOf('mousemove') === 0) {
        var split = msg.split('_');
        var absX = Number(split[1]);
        var absY = Number(split[2]);
        var frameX = Number(split[3]);
        var frameY = Number(split[4]);
        handleMousemove(absX, absY, frameX, frameY);
    }
    else if(msg == 'keydown') {
        toggleMode();
    }
}

function frameOnMousemove (e) {
    var frameLeft = $(window.frameElement).offset().left;
    var absX = Math.round(frameLeft + e.pageX);
    
    var frameTop = $(window.frameElement).offset().top;
    var absY = Math.round(frameTop + e.pageY);
    
    window.parent.postMessage('mousemove_' + absX + '_' + absY + '_' + e.pageX + '_' + e.pageY, '*');
}

function frameOnKeydown (e) {
    this.onkeydown = null;
    
    if(e.which !== 18) // ALT key
        return;
        
    window.parent.postMessage('keydown', '*');
}

function frameOnKeyup (e) {
    this.onkeydown = frameOnKeydown;
    e.preventDefault();
}

// functionality
function toggleMode () {
    ++mode;
    mode = mode % numberOfModes;
    updateMode();
}

function updateMode () {
    console.log('New mode: ' + mode);
            
    var dmadiv = $('#dmadiv');
    dmadiv.css('display', isDisplayed() ? 'block' : 'none');
    if(shouldFollowMouse())
    	updateDiv();
}

function handleMousemove (absX, absY, frameX, frameY) {
    savedAbsX = absX;
    savedAbsY = absY;
    savedFrameX = frameX;
    savedFrameY = frameY;
    
    if(shouldFollowMouse())
    	updateDiv();
}

function updateDiv () {   
    var dmadiv = $('#dmadiv');
    var dmadivContent = dmadiv.find('#dmadivContent');
    var dmadivFrameIndicator = dmadiv.find('#dmadivFrameIndicator');
    
    var elem = null;
    if(!savedFrameX)
    	elem = document.elementFromPoint(savedAbsX, savedAbsY);
    else
        elem = window.frames.WorkSpaceFrame.document.elementFromPoint(savedFrameX, savedFrameY);
    
    if(elem != savedElem)
    {
        savedElem = elem;
    	_updateDivContents(elem, dmadivContent, dmadivFrameIndicator);
    }
    
    _positionDiv(dmadiv);
}

function _updateDivContents (newElem, dmadivContent, dmadivFrameIndicator) {
    dmadivContent.empty();
    
    var dma = _collectDmaFor(newElem);
    if(dma) {
        var dmaTable = '<table style="margin:0.2em;border:1px solid black">';
        for(var dmaprop in dma) {
            dmaTable += '<tr><td>' + dmaprop + ':</td><td><input type="text" onclick="this.select();" readonly="readonly" size="' + Math.min((dma[dmaprop].length + 1), 50) + '" value="' + quoteattr(dma[dmaprop], false) + '"/></td></tr>';
        }
        dmaTable += '</table>';
        dmadivContent.append(dmaTable);
    } else {
        dmadivContent.text('No DMA available.');
    }
    
    dmadivFrameIndicator.text(!savedFrameX ? 'MAIN' : 'FRAME');
}

function _collectDmaFor (newElem) {
    var elem = $(newElem);
    
    var dma = null;
    while(true) {
        dma = _getAllDmaFor(elem);
        if(!$.isEmptyObject(dma))
            break;
        
        elem = elem.parent();
        // TODO: analyze why Chrome sometimes returns undefined at some point.
        if(elem.is('html') || elem[0] === undefined)
            break;
    }
    
    if($.isEmptyObject(dma))
        return null;
    
    return dma;
}

function _getAllDmaFor (elem) {
    var dma = {};
    
    if(elem.data('command-name') !== undefined)
        dma.CommandName = elem.data('command-name');
    
    if(elem.data('content') !== undefined)
        dma.Content_DisplayText = elem.data('content');
    
    if(elem.data('control-type') !== undefined)
        dma.ControlType = elem.data('control-type');
    
    if(elem.data('index') !== undefined)
        dma.Index = elem.data('index');
    
    if(elem.data('item-id') !== undefined)
        dma.ItemID = elem.data('item-id');
    
    if(elem.data('display-name') !== undefined)
        dma.DisplayName = elem.data('display-name');
    
    if(elem.data('bound-type') !== undefined)
        dma.BoundType = elem.data('bound-type');
    
    if(elem.data('bound-property') !== undefined)
        dma.BoundProperty = elem.data('bound-property');
    
    while(dma.ControlType == 'Command' && dma.ItemID === undefined) {
        elem = elem.parent();
        if(elem.data('item-id') !== undefined) {
            dma.ItemID = elem.data('item-id');
            
            if(dma.ItemID.endsWith('_Tab')) {
                dma.ItemID = dma.ItemID.substring(0, dma.ItemID.length - 4);
            }
        }
        if(elem.data('content') !== undefined) {
            dma.Content_DisplayText = elem.data('content');
        }
    }
    
    return dma;
}

function _positionDiv (div) {
    var width = div.outerWidth();
    var height = div.outerHeight();
    
    var windowWidth = $(window).width();
    var windowHeight = $(window).height();
    
    var newLeft = savedAbsX + 10;
    if((newLeft + width) > windowWidth) {
        newLeft = savedAbsX - 10 - width;
    }
    
    var newTop = savedAbsY + 10;
    if((newTop + height) > windowHeight) {
        newTop = savedAbsY - 10 - height;
    }
    
    div.offset({top:newTop, left:newLeft});
}

// helper functions
function areWeExecutedInFrame () {
   return window.frameElement !== null; 
}

function isDisplayed () {
    return mode === modeFollow || mode === modeShow;
}

function shouldFollowMouse () {
    return mode === modeFollow;
}

// Taken from http://stackoverflow.com/a/9756789/1400869
function quoteattr (s, preserveCR) {
    preserveCR = preserveCR ? '&#13;' : '\n';
    return ('' + s) /* Forces the conversion to string. */
        .replace(/&/g, '&amp;') /* This MUST be the 1st replacement. */
        .replace(/'/g, '&apos;') /* The 4 other predefined entities, required. */
        .replace(/"/g, '&quot;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        /*
        You may add other replacements here for HTML only 
        (but it's not necessary).
        Or for XML, only if the named entities are defined in its DTD.
        */ 
        .replace(/\r\n/g, preserveCR) /* Must be before the next replacement. */
        .replace(/[\r\n]/g, preserveCR);
}
