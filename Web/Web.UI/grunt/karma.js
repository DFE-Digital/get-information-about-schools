module.exports = function (grunt) {
    return {
        options: {
            configFile: 'test/karma-config.js'
        },
        chrome: {
            browsers: ['Chrome'],
            singleRun: true
        },
        firefox: {
            browsers: ['Firefox'],
            singleRun: true
        },
        ie: {
            browsers: ['IE'],
            singleRun: true
        },
        debug: {
          browsers: ['Chrome'],
          singleRun: false,
          // overrides coverage preprocessor so we can see the code
          preprocessors: {}
        }
    };
};