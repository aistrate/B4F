    function getControl(e,id) 
    {
        var elem;

        if (window.event) 
            elem = window.event.srcElement;
        else if (e && e.target) 
        {
            elem = e.target;
            while (elem.nodeType != 1) 
                elem = elem.parentNode;
        }
        
        if (elem)
        {
            switch(id)
            {
                case 'DecimalSeparator':
                case 'NumberGroupSeparator':
                case 'DecimalPlaces':
                    var controlName = elem.id.substring(0, elem.id.lastIndexOf('_') + 1) + 'hdf' + id;
                    return document.getElementById(controlName);
                default:
                    return elem;
            }
        }
        
        return null;    
    }

    // Prevents event bubble up or any usage after this is called.
    // pE - event object
    function StopEvent(pE)
    {
       if (!pE)
         if (window.event)
	    pE = window.event;
         else
	    return;
       if (pE.cancelBubble != null)
          pE.cancelBubble = true;
       if (pE.stopPropagation)
          pE.stopPropagation();
       if (pE.preventDefault)
          pE.preventDefault();
       if (window.event)
          pE.returnValue = false;
       if (pE.cancel != null)
          pE.cancel = true;
    }

    function ChangeControlInnerText(control, value)
    {
        if (control)
        {
            if (document.all)
            {
                control.innerText = value;
            }
            else
            {
                control.value = value;
            }
        }
    }
    
    function getSelectionStart(o) {
	    if (o.createTextRange) {
		    var r = document.selection.createRange().duplicate()
		    r.moveEnd('character', o.value.length)
		    if (r.text == '') return o.value.length
		    return o.value.lastIndexOf(r.text)
	    } else return o.selectionStart
    }

    function getSelectionEnd(o) {
	    if (o.createTextRange) {
		    var r = document.selection.createRange().duplicate()
		    r.moveStart('character', -o.value.length)
		    return r.text.length
	    } else return o.selectionEnd
    }

    function setSelectionStart(o, start) {
        range = o.createTextRange();
        //var m = range.moveStart('character', start);
        var m = range.move('character', start);
        range.select(); 
    } 

    function getControlValue(e,id) 
    {
        var control = getControl(e,id);
        if(control != null)
        {
            return control.value;
        }    
        return ''
    }

    function getDecimalSeparator(e) 
    {
        return getControlValue(e,'DecimalSeparator');
    }

    function getNumberGroupSeparator(e) 
    {
        return getControlValue(e,'NumberGroupSeparator');
    }

    function getDecimalPlaces(e) 
    {
        var decimals = getControlValue(e,'DecimalPlaces');
        if (decimals == null || decimals == '')
            return 0;
        else
            return parseInt(decimals);
    }

    function getKeyCode(e) 
    {
        var keynum;
        
        if(window.event) // IE
        {
            keynum = e.keyCode;
        }
        else if(e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which;
        }
        return keynum;
    }

    function checkDecimalSeparator(e) 
    {
        var decsep = getDecimalSeparator(e);
        var control = getControl(e,'tbDecimal');
        var selStart = getSelectionStart(control);
        var didChange = false;

        if(getKeyCode(e) == 110)
        {
            if(control != null && control.value != '')
            {
                ChangeControlInnerText(control, control.value.replace('.', decsep));
                //control.value = control.value.replace('.', decsep);
                
                selStart-=1;
                didChange = true;
            }
        }
        else
        {
            if (control != null && control.value != '')
            {
                var numsep = getNumberGroupSeparator(e);
                if (control.value.length > 1 && control.value.substring(0, 1) == decsep)
                {
                    ChangeControlInnerText(control, '0' + control.value);
                    didChange = true;
                }
                
                if (control.value.indexOf(numsep) > -1)
                {
                    ChangeControlInnerText(control, control.value.replace(numsep, ''));
                    didChange = true;
                }

                var locDecSep = control.value.indexOf(decsep);
                
                // check extra decimal separators
                if(locDecSep > -1 && control.value.lastIndexOf(decsep) > locDecSep)
                    ChangeControlInnerText(control, control.value.substring(0, locDecSep + 1) + control.value.substring(locDecSep + 2).replace(decsep, ''));
                
                var allowedDecimals = getDecimalPlaces(e);
                if (allowedDecimals > 0  && locDecSep > -1)
                {
                    var len = control.value.length;
                    var decimals = len - (locDecSep + 1);
                    if (decimals > allowedDecimals)
                    {
                        ChangeControlInnerText(control, control.value.substring(0, locDecSep + 1 + allowedDecimals));
                        didChange = true;
                    }
                }
            }
        }        
        if (didChange)
            setSelectionStart(control, selStart + 1);
    }

    function checkInput(e) 
    {
        var keynum = getKeyCode(e);
        var keyChar = String.fromCharCode(keynum);
        var control = getControl(e,'tbDecimal');
        var selStart = getSelectionStart(control);
        var selEnd = getSelectionEnd(control);

        if(keynum == 109 || keynum == 189)
        {
            if (control.value != '' && control.value.indexOf('-') > -1)
            {
                if(!(selStart == 0 && selEnd > 0))
                    StopEvent(e);
            }
            else if(selStart > 0)
                StopEvent(e);
            return;
                
        }

        var decsep = getDecimalSeparator(e);
        var numsep = getNumberGroupSeparator(e);
        var isDecSep = false;
        var isNumSep = false;

        switch(keynum)
        {
            case 110:
                isDecSep = true;
                break;
            case 188:
                if(decsep == ',')
                    isDecSep = true;
                else if(numsep == ',')
                    isNumSep = true;
                break;
            case 190:
                if(decsep == '.')
                    isDecSep = true;
                else if(numsep == '.')
                    isNumSep = true;
                break;
        }

        if(isNumSep)
            StopEvent(e);
        else
        {
            var locDecSep = control.value.indexOf(decsep);
            if(isDecSep)
            {
                if(control != null && control.value != '')
                {   
                    if (locDecSep > -1 )
                    {
                        if (!(selStart < selEnd && selStart <= locDecSep && selEnd > locDecSep))
                            StopEvent(e);
                    }
                }
            }
            else if ((keynum >= 48 && keynum <= 57) || (keynum >= 96 && keynum <= 105))
            {
                // check decimals
                if (control.value != null && control.value.length > 0)
                {
                    // if only zero -> replace
                    if(control.value == '0') 
                        ChangeControlInnerText(control, '');
                    
                    var allowedDecimals = getDecimalPlaces(e);
                    if (allowedDecimals > 0  && locDecSep > -1)
                    {
                        if (selStart > locDecSep)
                        {
                            if (selStart == selEnd)
                            {
                                var len = control.value.length;
                                var decimals = len - (locDecSep + 1);
                                if (decimals >= allowedDecimals)
                                    StopEvent(e);
                            }
                        }
                    }
                }
            }
        }
    }
    
    function checkNumber(e) 
    {
        var control = getControl(e,'tbDecimal');
        if(control != null && control.value != '')
        {   
            control.value = control.value.replace(getNumberGroupSeparator(e), '');
            
            var decsep = getDecimalSeparator(e);
            if (control.value.charAt(0) == decsep)
                control.value = '0' + control.value;

            if (control.value.charAt(control.value.length - 1) == decsep)
                control.value = control.value.substring(0, control.value.length - 1);

            if (control.value.indexOf('-') > 0)
                control.value = control.value.replace('-', '');

            if (control.value.length > 1 && control.value.charAt(0) == '0' && control.value.charAt(1) != decsep)
                control.value = control.value.substring(1, control.value.length);

            if (isNaN(parseFloat(control.value)))
                control.value = '';
        }
    }