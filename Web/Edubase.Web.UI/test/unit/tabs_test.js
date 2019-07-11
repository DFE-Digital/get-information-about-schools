
'use strict'
describe('Tab tests', function tabCheck(){

    beforeEach(function() {
        var fixture = '<div class="horizontal-tabs-wrapper" id="fixture">'+
            '<div class="tab-wrap">'+
            '<ul class="horizontal-tabs"><li><a href="#tab0" class="horizontal-tab">Tab 0</a></li>'+
            '<li><a href="#tab1" class="horizontal-tab">Tab 1</a></li>' +
            '<li><a href="#tab2" class="horizontal-tab">Tab 2</a></li></ul></div>'+
            '<div class="tab-content-wrapper horizontal-tabs-content">'+
            '<div id="tab0" class="tab-content">Tab zero</div>'+
            '<div id="tab1" class="tab-content">Tab one</div>'+
            '<div id="tab2" class="tab-content">Tab two</div></div></div>';

        // tabs are hidden with css - so inject the pertinent rules to the head
        var essentialTabCss = '<style>.hidden-tab-content { display: none;}</style>'

        $('head').append($(essentialTabCss));

        document.body.insertAdjacentHTML(
            'afterbegin',
            fixture);

        $('.horizontal-tabs-wrapper').tabs();
    });

    afterEach(function() {
        document.body.removeChild(document.getElementById('fixture'));
    });

	it('jQuery is available', function(){
		expect(window.$).not.toBeUndefined();
	});

	it('jQuery has a defined prototype', function(){
		expect($.fn).not.toBeUndefined();
	});

	it('jQuery prototype has a tabs property', function() {
		expect($.fn.hasOwnProperty('tabs')).toBeTruthy();
	});

    it('tabs is a function', function() {
        expect(typeof $.fn.tabs).toBe('function');
    });

    it('tab-able content has an aria-hidden property set', function(){
       expect(document.getElementById('tab0').hasAttribute('aria-hidden')).toBeTruthy();
    });

	it('tab-able content has a tabindex property and it is set to zero', function(){
	    var contentTab0 = document.querySelectorAll('.tab-content')[0];
       expect(contentTab0.hasAttribute('tabindex') && contentTab0.getAttribute('tabindex') == 0).toBeTruthy();
    });

    it('initially, only one tab is visible', function(){
        var $allTabContent = $('.tab-content');
        expect($allTabContent.filter(':visible').length === 1 ).toBeTruthy();
    });

    it('Activating an additional tab updates UI to display selected tab content', function(){
        var $allTabs = $('.horizontal-tabs').find('a'),
            $tabZero = $allTabs.eq(0),
            $tabOne = $allTabs.eq(1);

        $tabOne.click();
        expect(
            $tabOne.hasClass('selected-tab') &&
            !$tabZero.hasClass('selected-tab') &&
            $('#tab1').is(':visible') &&
            !$('#tab0').is(':visible')
        ).toBeTruthy();
    });

    it ('URL hash is updated after changing tabs', function(){
        var $tabTwo = $('.horizontal-tabs').find('a').eq(2);
        $tabTwo.click();
        expect(window.location.hash === $tabTwo.prop('href') );
    })

});
