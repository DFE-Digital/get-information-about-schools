module.exports = function () {
  return {
    /**
     * Copies asset files from the govuk_frontend_toolkit npm module
     */
    govuk: {
      files: [{
        expand: true,
        cwd: 'node_modules/govuk_frontend_toolkit',
        src: [
          './javascripts/**/*.*',
          './images/**/*.*'
        ],
        dest: 'app/assets/govuk_frontend_toolkit/'
      }]
    }
  };
};
