/*!
 *
 * Small scripts needed for browser detection ,navigations or tooltips
 * 
 */
 
 $.support.transition = false
 
// Add ie10 class for the html tag in IE10
if (/*@cc_on!@*/false) {
    document.documentElement.className+=' ie' + document.documentMode;
}

// Add ie11 class for the html tag in IE11
if(navigator.userAgent.match(/Trident.*rv:11\./)) {
	jQuery('html').addClass('ie11');
}

// IE bug for buttons
$(window).load(function(){ 
	if (!!navigator.userAgent.match(/Trident\/7\./)){
		$(function() {
			$('button.close').wrapInner('<span></span>');
			$('.btn:not(.left):not(.right):not(.go2first):not(.go2last):not(.prev):not(.next)').wrapInner('<span></span>');
			$('.btn:not(.left):not(.right) span.caret').each(function() {
				$(this).appendTo($(this).parent().parent());
			});
		});
	};
});


//detect if touchscreen or not
document.documentElement.className += (("ontouchstart" in document.documentElement) ? ' touch' : ' no-touch');	


//Disable bootstrap transitions
$(document).ready(function () {
    $.support.transition = false
});


//----------------- Tooltips
//add "tooltip-error" class
$('.tooltip-error-trig').tooltip({
	template: '<div class="tooltip tooltip-error"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>'
})

// Tooltip title - add data-html="true" to the html tag that activates the tooltip
$('[data-toggle=tooltip]').tooltip({
	//selector: "[rel=tooltip]"
	html: true
})
//----------------- /Tooltips



//----------------- Popover
$('[data-toggle=popover]:not([data-container=body])').on('shown.bs.popover', function () {
	$('.popover').css('top',parseInt($('.popover').css('top')) + 38 + 'px')
})

$(function () {
	$('[data-toggle="popover"]').popover({
        html : true
	})
})
//----------------- /Popover



//----------------- Select field
$(".select-wrapper select:not([multiple])").each(function(){
	$(this).after("<span class='holder'></span>");
});
$(".select-wrapper select:not([multiple])").change(function(){
	var selectedOption = $(this).find(":selected").text();
	$(this).next(".holder").text(selectedOption);
}).trigger('change'); 

// Pressed
$(".select-wrapper select:not([multiple])").focus(function(){
	$(this).parent().addClass("activeState");
}).blur(function(){
	$(this).parent().removeClass("activeState");
}).change(function(){
	$(this).parent().removeClass("activeState");
}) 
//----------------- /Select field



//---------------- Clear Search Field
$(function(){
	    $.fn.clearField = function () {
	        var _this = $(this);
	        var _parent = $(this).parent();
	        var _trigger = $('<span class="clear-search"></span>');

	        _parent.append(_trigger);

	        _trigger.click(function(){
	        	_this.val("");
	        	_trigger.removeClass('show').parent().removeClass('typing');
	        });

	        _this.on('keyup', function(){
	        	if (_this.val() !== "") {
	        		if (!_trigger.hasClass('show')) { _trigger.addClass('show').parent().addClass('typing'); }
	        	} else {_trigger.removeClass('show').parent().removeClass('typing');}
	        });
	    };

	$('.search-group .form-control').each(function(){
		$(this).clearField();
	});
	
	// Hover state
	$('.search-group').hover(
		function(){ $(this).addClass('hover') },
		function(){ $(this).removeClass('hover') }
	)
});


		
		
//----------------- Customized radio/checkbox elements - Focus state
$(".checkbox input[type='checkbox']").focus(function(){
	$(this).parent().addClass("focusState");
	}).blur(function(){
		$(this).parent().removeClass("focusState");
	})   
$(".radio input[type='radio']").focus(function(){
	$(this).parent().addClass("focusState");
	}).blur(function(){
		$(this).parent().removeClass("focusState");
	})   
//----------------- /Customized radio/checkbox elements - Focus state



//----------------- Tables
// pressed state on the head of the table
$(function(){
	$('.table-active thead th, .table-active thead td, .table-sorter thead th, .table-sorter thead td').mousedown(function(){
		$(this).addClass('pressed');
	})
	.mouseup(function(){
		$(this).removeClass('pressed');
	});
});


// pressed state on the row in the body of the table
$(function(){
	$('.table-active tbody th, .table-active tbody td, .table-sorter tbody th, .table-sorter tbody td').mousedown(function(){
		$(this).parent().addClass('pressed');
	})
	.mouseup(function(){
		$(this).parent().removeClass('pressed');
	});
});

// active on body
$('.table-active tbody tr, .table-sorter tbody tr').click(function() {
	$('.table-active tbody tr, .table-sorter tbody tr').removeClass('active');
	$(this).addClass('active');
}); 
//----------------- /Tables



//----------------- Vertical menu
// add an additional class if one of the child elemnt is active 
$('.sidenav .inner-scroll > .nav').on('activate.bs.scrollspy', function () {
	if ($('.nav .nav > li').hasClass('active')) {
		$('.sidenav .inner-scroll > .nav > li.active').addClass('active-child')
	}
	else {
		$('.sidenav .inner-scroll > .nav > li').removeClass('active-child')
	}
});

$('.sidenav .inner-scroll > .nav > li:not(.show-nav) > a').click(function() {
	$('.sidenav .inner-scroll > .nav > li').removeClass('active');
	$(this).parent().addClass('active');
	$('.sidenav .inner-scroll > .nav > li').removeClass('active-child');
	$('.sidenav .inner-scroll > .nav .nav > li').removeClass('active');
}); 

$('.sidenav .inner-scroll > .nav .nav > li > a').click(function() {
	$('.sidenav .inner-scroll > .nav .nav > li').removeClass('active');
	$(this).parent().addClass('active');
	$(this).parent().parent().parent().addClass('active-child');
});


//----------------- Show/Hide vertical menu - Multiple levels
if ($('body.bs-body-class-js').length > 0) {
	var menuLeftMultiLvl = document.getElementById( 'cbp-spmenu-multiLvl' ),
		contentRightMultiLvl = document.getElementById( 'contentPushRight-multiLvl' ),
		showLeftPushMultiLvl = document.getElementById( 'showLeftPush-multiLvl' ),
		body = document.body;

	showLeftPushMultiLvl.onclick = function() {
		classie.toggle( this, 'active' );
		classie.toggle( body, 'cbp-spmenu-push-toright' );
		classie.toggle( menuLeftMultiLvl, 'cbp-spmenu-open' );
		$(contentRightMultiLvl).toggleClass('col-md-12');
		disableOther( 'showLeftPush-multiLvl' );
	};
}

	function disableOther( button ) {
		if( button !== 'showLeftPush2' ) {
			classie.toggle( showLeftPushMultiLvl, 'disabled' );
		}
	} 

// ----------------- Show/Hide vertical menu - Multiple levels - primary
if ($('body.bs-body-class-js').length > 0) {
	var menuLeftMultiLvlPrimary = document.getElementById( 'cbp-spmenu-multiLvl-primary' ),
		contentRightMultiLvlPrimary = document.getElementById( 'contentPushRight-multiLvl-primary' ),
		showLeftPushMultiLvlPrimary = document.getElementById( 'showLeftPush-multiLvl-primary' ),
		body = document.body;

	showLeftPushMultiLvlPrimary.onclick = function() {
		classie.toggle( this, 'active' );
		classie.toggle( body, 'cbp-spmenu-push-toright' );
		classie.toggle( menuLeftMultiLvlPrimary, 'cbp-spmenu-open' );
		$(contentRightMultiLvlPrimary).toggleClass('col-md-12');
		disableOther( 'showLeftPush-multiLvl-primary' );
	};
}

	function disableOther( button ) {
		if( button !== 'showLeftPush2' ) {
			classie.toggle( showLeftPushMultiLvlPrimary, 'disabled' );
		}
	} 

	
//----------------- Show/Hide vertical menu - Sigle level
if ($('body.bs-body-class-js').length > 0) {
	var menuLeftOneLvl = document.getElementById( 'cbp-spmenu-oneLvl' ),
		contentRightOneLvl = document.getElementById( 'contentPushRight-oneLvl' ),
		showLeftPushOneLvl = document.getElementById( 'showLeftPush-oneLvl' ),
		body = document.body;

	showLeftPushOneLvl.onclick = function() {
		classie.toggle( this, 'active' );
		classie.toggle( body, 'cbp-spmenu-push-toright' );
		classie.toggle( menuLeftOneLvl, 'cbp-spmenu-open' );
		$(contentRightOneLvl).toggleClass('col-md-12');
		disableOther( 'showLeftPush-oneLvl' );
	};
}

	function disableOther( button ) {
		if( button !== 'showLeftPush2' ) {
			classie.toggle( showLeftPushOneLvl, 'disabled' );
		}
	}	
	
//----------------- Show/Hide vertical menu - Sigle level - primary
if ($('body.bs-body-class-js').length > 0) {
	var menuLeftOneLvlPrimary = document.getElementById( 'cbp-spmenu-oneLvl-primary' ),
		contentRightOneLvlPrimary = document.getElementById( 'contentPushRight-oneLvl-primary' ),
		showLeftPushOneLvlPrimary = document.getElementById( 'showLeftPush-oneLvl-primary' ),
		body = document.body;

	showLeftPushOneLvlPrimary.onclick = function() {
		classie.toggle( this, 'active' );
		classie.toggle( body, 'cbp-spmenu-push-toright' );
		classie.toggle( menuLeftOneLvlPrimary, 'cbp-spmenu-open' );
		$(contentRightOneLvlPrimary).toggleClass('col-md-12');
		disableOther( 'showLeftPush-oneLvl-primary' );
	};
}

	function disableOther( button ) {
		if( button !== 'showLeftPush2' ) {
			classie.toggle( showLeftPushOneLvlPrimary, 'disabled' );
		}
	}
//----------------- /Vertical menu 



//----------------- Tabs
// Enable Tabdrop - add dropdown when you have more tabs that can fit in one row
$('.nav.nav-tabs').tabdrop('layout');

// add pressed state class
$('.nav-tabs:not(.navbar-nav) > li > a').on('mousedown', function(e){ 
	$(this).addClass("pressed");
	});
$('.nav-tabs:not(.navbar-nav) > li > a').on('mouseup', function(e){ 
	$(this).removeClass("pressed");
	});
//----------------- /Tabs



//----------------- Wizard
// add class on parent div width the number of childs, used for the each item width
$(".wizard > ul").each(function(){
	var count = $(this).children().size();
	$(this).parent().addClass('childs-' + count);
});

// add pressed state class
$( ".wizard > ul > li:not(.active)" )
.mouseup(function() {
	$(this).removeClass("pressed");
})
.mousedown(function() {
	$(this).addClass("pressed");
});

//----------------- /Wizard



//----------------- Attach button
$("input[type='file'].input-btn-attach").change(function () {
    if ($("input[type='file'].input-btn-attach").val() == "") {
        return;
    }
    
	// your ajax submit
	$("input[type='file'].input-btn-attach").addClass("file-attached");	
});
//----------------- /Attach button



//---------------- Navigation
// Enable Tabdrop - add dropdown when you have more tabs that can fit in one row
$('.navbar-nav').tabdrop('layout');

// Toggle class to display burger menu animation 
$('.navbar.navbar-default .navbar-toggle').click(function () {
    $('.navbar.navbar-default').toggleClass('close-navigation');
});

$(window).load(function(){ 	
	// move second level in the hierarchy
	var nav = ('.navbar-collapse');
	$(nav).each(function(){		
		$(this).find('.first-level > .active.first-level-item > .second-level.in').clone().appendTo($(this));
	});
	
	// Dropdown caret
	if ($(this).parent().children('.second-level > .second-level-item > a').hasClass('active')){
		$(this).siblings('.second-level').addClass('in');
		$(this).parent().addClass('open');
	} else {
		$(this).siblings('.second-level').removeClass('in');
		$(this).parent().removeClass('open');
	}
	
	// Hover state
	$('.navbar .dropdown-toggle:not(:first-child)').hover(
		function(){ $(this).parent().addClass('hoverState') },
		function(){ $(this).parent().removeClass('hoverState') }
	)
	
	// navbar-form - focus state
	$(".navbar-form .search-group input[type='text']").focus(function(){
		$(this).parent().addClass("focus");
	}).blur(function(){
		$(this).parent().removeClass("focus");
	})
});


