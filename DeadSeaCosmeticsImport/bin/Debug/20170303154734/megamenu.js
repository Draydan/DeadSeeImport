$(document).ready(function(){var timeout=500;var sizewait=0;var hoverwait=100;var hovertimer=null;var sizetimer=null;var closetimer=null;var hoverParent=null;var hoverBin=null;var hoverSlots=null;var megaSlots=null;var megaParents=null;var hideOffset=-9000;var megaParents=$('.megamenu-menu').find('.megamenu-parent');var megaParentTitles=$('.megamenu-menu').find('.megamenu-parent-title');var megaBins=$('.megamenu-menu').find('.megamenu-bin');var oldParentIdx=-1;var hoverParentIdx=-2;megaBins.css('top',hideOffset);function megamenu_open(){megamenu_canceltimer();if($(this).hasClass('megamenu-parent-title')){hoverParentIdx=megaParentTitles.index($(this));}
else if($(this).hasClass('megamenu-bin')){hoverParentIdx=megaParents.index($(this).parents('.megamenu-parent'));}
hoverParent=megaParents.eq(hoverParentIdx);if(hoverParentIdx!=oldParentIdx){megamenu_close();megamenu_hovertimer();}else{megamenu_display();}}
function megamenu_display(){if(hoverParent){hoverParent.addClass('hovering');hoverBin=hoverParent.find('.megamenu-bin');hoverBin.css('top','auto');megamenu_sizer();}}
function megamenu_close(){if(hoverParent){oldParentIdx=$('.megamenu-parent').index(hoverParent);}
for(var i=0;i<megaParents.length;i++){megaParents.eq(i).removeClass('hovering');}
if(hoverBin)hoverBin.css('top',hideOffset);if(hoverBin)hoverBin.css('width',0+'px');}
function megamenu_closeAll(){if(hoverBin)hoverBin.css('top',hideOffset);for(var i=0;i<megaParents.length;i++){megaParents.eq(i).removeClass('hovering');}
oldParentIdx=-1;if(hoverBin)hoverBin.css('width',0+'px');}
function megamenu_stopPropagation(event){event.stopPropagation();}
function megamenu_timer(){megamenu_canceltimer();megamenu_cancelhovertimer();closetimer=window.setTimeout(megamenu_closeAll,timeout);}
function megamenu_canceltimer(){if(closetimer){window.clearTimeout(closetimer);closetimer=null;}}
function megamenu_hovertimer(){megamenu_cancelhovertimer();hovertimer=window.setTimeout(megamenu_display,hoverwait);}
function megamenu_cancelhovertimer(){if(hovertimer){window.clearTimeout(hovertimer);hovertimer=null;}}
function megamenu_sizetimer(){sizetimer=window.setTimeout(megamenu_sizer,sizewait);}
function megamenu_sizer(){for(var k=0;k<megaBins.length;k++){megaBins.eq(k).css('left',0+'px');megaBins.eq(k).css('width',0+'px');var megaSlots=megaBins.eq(k).find('.mega-wrapit');if(megaBins.eq(k).hasClass('megamenu-slots-columnar')){var slotTotalWidth=0;for(var i=0;i<megaSlots.length;i++){slotTotalWidth+=megaSlots.eq(i).outerWidth(true)+183;if(slotTotalWidth>$(window).width()){slotTotalWidth=0;for(var j=0;j<i;j++){slotTotalWidth+=megaSlots.eq(i).outerWidth(true);}
slotTotalWidth+=100;break;}}
slotTotalWidth=950;megaBins.eq(k).css('width',slotTotalWidth);megaBins.eq(k).width(slotTotalWidth);}
else{for(var i=0;i<megaSlots.length;i++){}
megaBins.eq(k).css('width',megaSlots.eq(0).outerWidth(true));}
var edgeOverlap=($(window).width()-(megaBins.eq(k).offset().left+ megaBins.eq(k).outerWidth(true)));if(edgeOverlap<0){megaBins.eq(k).css('left',(edgeOverlap)+'px');}}}
$('.megamenu-parent-title').bind('mouseover',megamenu_open);$('.megamenu-parent-title').bind('mouseout',megamenu_timer);$('.megamenu-bin').bind('mouseover',megamenu_open);$('.megamenu-bin').bind('mouseout',megamenu_timer);$("body").bind('click',megamenu_closeAll);$(".megamenu-menu").bind('click',megamenu_stopPropagation);$(window).bind('resize',megamenu_sizer);});