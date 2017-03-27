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
    },
    jsonStubs: {
          files: [{
              expand: true,
              cwd: 'Assets/Scripts/JSON',
              src: '*.*',
              dest: 'public/assets/scripts/JSON/'
      }]      
    }
  };
};
