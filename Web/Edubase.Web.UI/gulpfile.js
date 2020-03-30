const { src, dest, watch, series, parallel } = require('gulp');
const sass = require('gulp-sass');
const del = require('del');
const sourcemaps = require('gulp-sourcemaps');
const gulpIf = require('gulp-if');
const postcss = require('gulp-postcss');
const autoprefixer = require('autoprefixer');
const cssnano = require('cssnano');
const run = require('gulp-run');
const gulpStylelint = require('gulp-stylelint');

const scssPath = './Assets/Sass/*.scss';
const cssDest = './public/assets/stylesheets';


function cleanCss() {
  return del(cssDest)
}

function cleanJs(){
  return del('./public/assets/scripts/build');
}

function lintScss() {
  // return run('npm run stylelint').exec();
  return src('./Server/Assets/Sass/**/*.scss')
    .pipe(gulpStylelint({
      reporters: [
        {formatter: 'string', console: true}
      ]
    }));
}

// build compressed css file no source maps
function prodCss() {
  return src(scssPath)
    .pipe(sourcemaps.init())
    .pipe(sass())
    .pipe(postcss([ autoprefixer(), cssnano() ]))
    .pipe(dest('./public/assets/stylesheets'));
}

// build uncompressed css files with source maps
function devCss() {
  return src(scssPath)
    .pipe(sourcemaps.init())
    .pipe(sass())
    .pipe(postcss([ autoprefixer() ]))
    .pipe(sourcemaps.write('./sourcemaps'))
    .pipe(dest('./public/assets/stylesheets'));
}


function devJs() {
  return run('npm run build:local').exec();
}

function prodJs() {
  return run('npm run build:prod').exec();
}

function unitTest() {
  return  run('npm test -- --verbose --ci').exec();
}

const compileProdCss = series(cleanCss, lintScss,  parallel(prodCss));
const compileDevCss = series(cleanCss, lintScss, parallel(devCss));

const buildDevJs = series(cleanJs, devJs);
const buildProdJs = series(unitTest, prodJs);

const buildDev = series(cleanCss, lintScss, parallel(devCss, devJs));

const build = series(cleanCss, lintScss, parallel(prodCss, prodJs));

exports.compileCss = compileProdCss;
exports.compileDevCss = compileDevCss;

exports.buildJs = buildProdJs;
exports.buildDevJs = buildDevJs;

exports.unitTest = unitTest;

exports.buildDev = buildDev;

exports.build = build;
