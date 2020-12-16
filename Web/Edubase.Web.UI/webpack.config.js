'use strict';
const webpack = require('webpack');
const path = require('path');
const glob = require('glob');
const buildDir = path.resolve('./public/assets/scripts/build');
const entryDirPath = path.resolve('./Assets/Scripts/Entry/');
const entryFiles = path.join(entryDirPath, '**/*.js');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const VueLoaderPlugin = require('vue-loader/lib/plugin');

module.exports = (env) => {
  const isProdBuild = env === 'prod';
  return {
    mode: isProdBuild ? 'production' : 'development',

    entry: glob
      .sync(entryFiles)
      .reduce((acc, filePath) => {
        const file = path.parse(filePath);
        acc[file.name] = path.resolve(process.cwd(), filePath);
        return acc;
      }, {}),

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
    ]
  };
};

