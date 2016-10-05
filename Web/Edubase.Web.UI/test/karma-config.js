module.exports = function (config) {

  "use strict";

  return config.set({
    basePath: '.',
    port: 9876,
    frameworks: ['jasmine'],
    reporters: ['mocha', 'coverage'],
    log: 'karma.log',
    colors: true,
    plugins: [
      'karma-mocha-reporter',
      'karma-chrome-launcher',
      'karma-ie-launcher',
      'karma-firefox-launcher',
      'karma-jasmine',
      'karma-coverage'
    ],
    preprocessors: {
      '../Assets/Scripts/**/*.js': ['coverage']
    },
    coverageReporter: {
      dir: 'results/coverage/', // placed under ./test/results/coverage/
      reporters: [
        // reporters not supporting the `file` property
        { type: 'html', subdir: 'report-html' }, // friendly viewable site
        { type: 'lcovonly', subdir: 'report-lcov' } // used by sonar
      ]
    },
    files: [
      // libs
      '../public/jquery/dist/jquery.js',
      './helpers/event-helpers.js',

      // app code
      //'../public/assets/scripts/application.js',
    '../node_modules/govuk_frontend_toolkit/javascripts/*.js',
    '../node_modules/govuk_frontend_toolkit/javascripts/govuk/*.js',
    '../node_modules/govuk_frontend_toolkit/javascripts/govuk/analytics/*.js',
    '../Server/Assets/Scripts/*.js',
    '../Server/Assets/Scripts/Behaviours/Collapsible/*.js',
    '../Server/Assets/Scripts/Behaviours/Accordion/*.js',
    '../Server/Assets/Scripts/Behaviours/PartialRequest/*.js',
    '../Server/Assets/Scripts/Elements/**/*.js',
    '../Server/Assets/Scripts/Views/**/*.js',
      // tests
      'unit/**/*.js'
    ],
    browsers: ['Chrome'],
    singleRun: false
  });

};