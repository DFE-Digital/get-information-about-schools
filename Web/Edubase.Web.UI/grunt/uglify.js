module.exports = function (grunt) {
    var pathUtil = require('path');
    return {
        "bundle-elements-dev": {
            options: {
                sourceMap: false,
                beautify: true,
                compress: false,
                mangle: false,
                preserveComments: 'some' // allow jScript conditional compilation
            },
            src: [
              // couldn't use /**/*.js as govuk/analytics/error-tracking.js causes an error with GOVUK undefined
              'node_modules/govuk_frontend_toolkit/javascripts/*.js',
              'node_modules/govuk_frontend_toolkit/javascripts/govuk/*.js',
              'node_modules/govuk_frontend_toolkit/javascripts/govuk/analytics/*.js',
              'Assets/Scripts/libs/vue/vue.min.js',
              'Assets/Scripts/*.js',
              // 'Scripts/jquery.validate.js',
              // 'Scripts/jquery.validate.unobtrusive.js',
              'Assets/Scripts/Behaviours/Collapsible/*.js',
              'Assets/Scripts/Behaviours/Accordion/*.js',
              'Assets/Scripts/Behaviours/PartialRequest/*.js',
              'Assets/Scripts/Behaviours/floatThead/*.js',
              'Assets/Scripts/Elements/**/*.js',
              'Assets/Scripts/Views/**/*.js'
            ],
            dest: 'public/assets/scripts/application.js'
        },
        "bundle-elements-local": { // task to build application.js with the development version of vue.js
            options: {
                sourceMap: false,
                beautify: true,
                compress: false,
                mangle: false,
                preserveComments: 'some' // allow jScript conditional compilation
            },
            src: [
              // couldn't use /**/*.js as govuk/analytics/error-tracking.js causes an error with GOVUK undefined
              'node_modules/govuk_frontend_toolkit/javascripts/*.js',
              'node_modules/govuk_frontend_toolkit/javascripts/govuk/*.js',
              'node_modules/govuk_frontend_toolkit/javascripts/govuk/analytics/*.js',
              'Assets/Scripts/libs/vue/vue.js',
              'Assets/Scripts/*.js',
              // 'Scripts/jquery.validate.js',
              // 'Scripts/jquery.validate.unobtrusive.js',
              'Assets/Scripts/Behaviours/Collapsible/*.js',
              'Assets/Scripts/Behaviours/Accordion/*.js',
              'Assets/Scripts/Behaviours/PartialRequest/*.js',
              'Assets/Scripts/Behaviours/floatThead/*.js',
              'Assets/Scripts/Elements/**/*.js',
              'Assets/Scripts/Views/**/*.js'
            ],
            dest: 'public/assets/scripts/application.js'
        },
        "process-standalone": {
            options: {
                sourceMap: false,
                beautify: true,
                compress: true,
                mangle: true
            },
            files: [{
                expand: true,
                flatten: true,
                cwd: '.',
                src: ['Assets/Scripts/standalone/*.js'],
                dest: 'public/assets/scripts/standalone/',                    
            }]
        }
    };
};