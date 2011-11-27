function fireEvent(eventName, element)
{
    if(Sys.Browser.agent===Sys.Browser.InternetExplorer)
    {
        element.fireEvent('on'+eventName);
        Sys.Debug.trace('IE '+eventName+' fired');
    }
    else
    {
        var evt = document.createEvent('UIEvents');
        evt.initUIEvent(eventName,true,false,window,1);
        var bool = element.dispatchEvent(evt);
        Sys.Debug.trace('Mozilla '+eventName+' fired');
        //alert(bool);
    }
}

function setTextBoxSelectionRange(element,startIndex, endIndex)
{
    if(Sys.Browser.agent===Sys.Browser.InternetExplorer)
    {
        var oRange = element.createTextRange();
        oRange.moveStart('character', startIndex);
        oRange.moveEnd('character', endIndex);
        oRange.select();
        element.focus();
    }
    else
    {
        element.setSelectionRange(startIndex,endIndex);
        element.focus();
    }
}

function findPos(obj) {
	var curleft = curtop = 0;
	if (obj.offsetParent) {
		curleft = obj.offsetLeft
		curtop = obj.offsetTop
		while (obj = obj.offsetParent) {
			curleft += obj.offsetLeft
			curtop += obj.offsetTop
		}
	}
	return [curleft,curtop];
}