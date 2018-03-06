module.exports = function(grunt) {
  return {
    scripts: {
      options: {
        jshintrc: true
      },
      files: {
        src: ['Assets/Scripts/*.js', 'Assets/Scripts/Views/**/*.js', 'Assets/Scripts/Elements/**/*.js']
      }
    }
  };
};