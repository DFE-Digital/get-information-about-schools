'use strict';

describe('Map interactions', function(){

    beforeEach(function(){
       var mapElem = '<div id="map" style="width:400px; height:400px;" class="school-detail-map" data-latlng="[0.15585,51.5]"' +
        'data-school-name="School name" data-school-address="School Road, School Town, SC10 00L"></div>';

        document.body.insertAdjacentHTML(
            'afterbegin',
            mapElem);


        spyOn(DfE.mapInteractions, 'loadGoogleScript').and.callThrough();
        spyOn(DfE.mapInteractions, 'initMap').and.callThrough();

        DfE.mapInteractions.loadGoogleScript("", DfE.mapConfig.apiKey);
    });


    afterEach(function(){
       document.body.removeChild(document.getElementById('map'));
    });


    it('Map interaction script is available', function(){
       expect(typeof window.DfE.mapInteractions !== 'undefined').toBeTruthy();

    });

    it('Map interactions has a `loadGoogleScript` method', function(){
        expect($.isFunction(DfE.mapInteractions.loadGoogleScript)).toBeTruthy();
    });

    it('Map interactions has an `initMap` method', function(){
        expect($.isFunction(DfE.mapInteractions.initMap)).toBeTruthy();
    });

    it('Google scripts loaded', function(){
        expect(DfE.mapInteractions.loadGoogleScript).toHaveBeenCalled();
    });

    it('Map initialisation called', function(){
        window.setTimeout(function(){
            expect(DfE.mapInteractions.initMap).toHaveBeenCalled();
        },500);
    });

    it('Map element has a child node containing the Google copyright message', function(){
        window.setTimeout(function(){
            expect($('map').find('.gmnoprint').length > 0).toBeTruthy();
        },1000);

    });
});
