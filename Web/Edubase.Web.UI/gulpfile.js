'use strict';

const gulp = require('gulp');
const sass = require('gulp-sass');
const del = require('del');
const run = require('gulp-run');
const gulpIf = require('gulp-if');
const sourcemaps = require('gulp-sourcemaps');
const postcss = require('gulp-postcss');
const rev = require('gulp-rev');
const autoprefixer = require('autoprefixer');
const cssnano = require('cssnano');
const es = require('event-stream');

/*
 * Main tasks to build CSS and JS
 */
gulp.task('compile-ui-assets:dev', ['compile-js:dev', 'compile-css:dev', 'compile-error-css']);
gulp.task('compile-ui-assets:prod', ['unit-test', 'compile-js:prod', 'compile-css:prod', 'compile-error-css']);

gulp.task('unit-test', () => run('npm test -- --verbose --ci').exec());

/**
 * Proxy to npm scripts as gulp-webpack does not use webpack 2 and also NODE_ENV
 * is required to be set via cross-env for windows/mac
 */
gulp.task('compile-js:dev', () => run('npm run compile-js:dev').exec());
gulp.task('compile-js:prod', () => run('npm run compile-js:prod').exec());

/**
 * Watch sass files for changes
 * Use webpack directly for watching JS changes
 * `npm run compile-js:dev -- --watch`
 */
gulp.task('watch', () => {
  gulp.watch(['Assets/Sass/**/*.scss', '!Assets/Sass/error-page.scss'], ['compile-css:dev']);
});

gulp.task('clean:css', () => del(['public/assets/stylesheets/*', '!public/assets/stylesheets/error-page.css']));
gulp.task('lint:css', () => run('npm run stylelint').exec());

gulp.task('compile-css:dev', ['lint:css', 'clean:css'], compileSass({production: false}));
gulp.task('compile-css:prod', ['lint:css', 'clean:css'], compileSass({production: true}));

gulp.task('compile-error-css', () => {
  return gulp.src('Assets/Sass/error-page.scss')
    .pipe(sass({
      includePaths: ['node_modules/govuk_frontend_toolkit/stylesheets'],
    }))
    .pipe(postcss(plugins))
    .pipe(postcss([cssnano()]))
    .pipe(gulp.dest('public/assets/stylesheets'));
});

const plugins = [
  autoprefixer(),
];

function compileSass(opts) {
  const sassOpts = {
    includePaths: ['node_modules/govuk_frontend_toolkit/stylesheets'],
    outputStyle: 'expanded',
    imagePath: '../images',
  };
  const isProduction = opts.production;

  return () => {
    const sassInputs = [
      'main',
      'raise-print',
      'main-ie6',
      'main-ie7',
      'main-ie8',
    ];
    const streams = sassInputs.map(file => {
      return gulp.src(`Assets/Sass/${file}.scss`)
        .pipe(gulpIf(!isProduction, sourcemaps.init()))
        .pipe(sass(sassOpts))
        .pipe(postcss(plugins))
        .pipe(gulpIf(isProduction, postcss([cssnano()])))
        .pipe(rev())
        .pipe(gulpIf(!isProduction, sourcemaps.write('./sourcemaps')))
        .pipe(gulp.dest('public/assets/stylesheets'));
    });
    return es.concat(streams);
  };
}
