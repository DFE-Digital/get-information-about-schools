'use strict';
const webpack = require('webpack');
const path = require('path');
const buildDir = path.resolve('./public/assets/scripts/build');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const VueLoaderPlugin = require('vue-loader/lib/plugin');

module.exports = (env) => {
  const isProdBuild = env === 'prod';
  return {
    mode: isProdBuild ? 'production' : 'development',

    entry: {
      'bundle': './Assets/Scripts/Entry/entry.js',
      'leaflet-bundle': './Assets/Scripts/Entry/leaflet.js',
      'search-bundle': './Assets/Scripts/Entry/search.js',
      'search-results-bundle': './Assets/Scripts/Entry/search-results.js',
      'landing-page-bundle': './Assets/Scripts/Entry/landing-pages.js',
      'download-select-fields': './Assets/Scripts/Entry/download-select-fields',
      'edit-details': './Assets/Scripts/Entry/edit-details',
      'add-edit-governor': './Assets/Scripts/Entry/add-edit-governor',
      'edit-establishment-links': './Assets/Scripts/Entry/edit-establishment-links',
      'edit-location': './Assets/Scripts/Entry/edit-location',
      'edit-helpdesk': './Assets/Scripts/Entry/edit-helpdesk',
    },

    output: {
      filename: '[name].js',
      path: buildDir
    },
    resolve: {
      alias: {
        'vue$': 'vue/dist/vue.esm.js'
      },
      extensions: ['*', '.js', '.vue', '.json']
    },
    module: {
      rules: [
       {
          test: /\.js$/,
          exclude: /node_modules/,
          loader: 'babel-loader',
        },
        {
          test: /\.vue$/,
          loader: 'vue-loader'
        }
      ]
    },
    plugins: [
      new VueLoaderPlugin(),
      new webpack.ProvidePlugin({
        $: "jquery",
        jQuery: "jquery"
      }),
      new UglifyJsPlugin({
        uglifyOptions: {
          sourceMap: !isProdBuild,
          compress: isProdBuild,
          output: {
            comments: !isProdBuild
          }
        }
      }),
      new webpack.DefinePlugin({
        'process.env': {
          NODE_ENV: '"development"'
        }
      })
    ]
  };
};

