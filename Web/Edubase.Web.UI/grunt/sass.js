module.exports = function (grunt) {
  var isLocal = grunt.option('target') === "local";
  return {
    'generate-dev': {
      files: {
        'public/assets/stylesheets/main.css': 'Assets/Sass/main.scss',
        'public/assets/stylesheets/main-ie6.css': 'Assets/Sass/main-ie6.scss',
        'public/assets/stylesheets/main-ie7.css': 'Assets/Sass/main-ie7.scss',
        'public/assets/stylesheets/main-ie8.css': 'Assets/Sass/main-ie8.scss'
      },
      options: {
        includePaths: ['node_modules/govuk_frontend_toolkit/stylesheets'],
        outputStyle: isLocal ? 'expanded' : 'compressed',
        imagePath: '../images',
        sourceMap: isLocal
      }
    }
  };
};
