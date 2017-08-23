module.exports = function (grunt) {
    var pathUtil = require('path');
    var scriptBundle = [
        'node_modules/govuk_frontend_toolkit/javascripts/govuk/analytics/*.js',
        'Assets/Scripts/libs/vue/vue.min.js',
        'Assets/Scripts/*.js',
        'Assets/Scripts/Elements/**/*.js',
        'Assets/Scripts/Views/**/*.js'
    ];
    var isLocal = grunt.option('target') === "local";
    var localBundle = (function () {
        if (isLocal) {
            scriptBundle.splice(1, 1, 'Assets/Scripts/libs/vue/vue.js');
            return scriptBundle;
        }
        
    }());
    return {
        "bundle-elements-dev": {
            options: {
                sourceMap: isLocal,
                beautify: isLocal,
                compress: !isLocal,
                mangle: !isLocal,
                preserveComments: 'some' // allow jScript conditional compilation
            },
            src: scriptBundle,
            
            dest: 'public/assets/scripts/application.js'
        },
        "process-standalone": {
            options: {
                sourceMap: false,
                beautify: false,
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