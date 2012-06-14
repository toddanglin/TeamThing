/*
* Kendo UI Complete v2012.1.515 (http://kendoui.com)
* Copyright 2012 Telerik AD. All rights reserved.
*
* Kendo UI Complete commercial licenses may be obtained at http://kendoui.com/complete-license
* If you do not own a commercial license, this file shall be governed by the trial license terms.
*/
(function(a,b){var c=window.kendo,d=c.mobile.ui,e=c.ui.Popup,f="visibility",g="hidden",h="visible",i='<div class="km-shim"/>',j=d.Widget,k=j.extend({init:function(b,d){var e=this,f=a(i).hide();j.fn.init.call(e,b,d),e.shim=f,e.element=b,e.options.modal||e.shim.on(c.support.mouseup,a.proxy(e.hide,e)),c.notify(e)},options:{name:"Shim",modal:!0,duration:200},viewInit:function(a){var b=this,c=a.application,d="center center",f="fade:in";c.os==="ios"&&(d="bottom left",f="slideIn:up"),c.element.append(b.shim),b.popup=new e(b.element,{anchor:b.shim,appendTo:b.shim,origin:d,position:d,animation:{open:{effects:f,duration:b.options.duration},close:{duration:b.options.duration}},closed:function(){b.shim.hide()},open:function(){b.shim.show()}})},show:function(){this.popup.open(),this.popup.wrapper.css({width:"",left:"",top:""})},hide:function(){this.popup.close()}});d.plugin(k)})(jQuery)