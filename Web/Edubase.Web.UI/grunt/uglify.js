module.exports = function (grunt) {
    var pathUtil = require('path');
    var scriptBundle = [
        'node_modules/govuk_frontend_toolkit/javascripts/govuk/analytics/*.js',
        'Assets/Scripts/libs/vue/vue.min.js',
        'Assets/Scripts/*.js',
        'Assets/Scripts/Elements/**/*.js',
        // Views
        'Assets/Scripts/Views/create-edit-school.js',
        'Assets/Scripts/Views/edit-governors.js',
        'Assets/Scripts/Views/edit-iebt.js',
        'Assets/Scripts/Views/establishment-details.js',
        'Assets/Scripts/Views/gias-search.js',
        'Assets/Scripts/Views/governor-search.js',
        'Assets/Scripts/Views/group-details.js',
        'Assets/Scripts/Views/school-search-form.js'
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
        "bundle-elements-prod": {
            options: {
                sourceMap: true,
                beautify: false,
                compress: true,
                mangle: true
            },
            src: scriptBundle,
            dest: 'public/assets/scripts/application.js'
        },
        "process-standalone": {
            options: {
                sourceMap: isLocal,
                beautify: isLocal,
                compress: !isLocal,
                mangle: !isLocal
            },
            files: [{
                expand: true,
                flatten: true,
                cwd: '.',
                src: ['Assets/Scripts/standalone/*.js'],
                dest: 'public/assets/scripts/standalone/',
            }]
        },
        "leaflet-bundle": {
            options: {
                sourceMap: isLocal,
                beautify: isLocal,
                compress: !isLocal,
                mangle: !isLocal,
                preserveComments: isLocal
            },
            src: [
                'Assets/Scripts/libs/leaflet/leaflet-src.js',
                'Assets/Scripts/libs/leaflet/plugins/leaflet.markercluster-src.js'
            ],

            dest: 'public/assets/scripts/leaflet-bundle.js'
        }
    };
};
