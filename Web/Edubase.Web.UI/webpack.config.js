'use strict';
const webpack = require('webpack');
const path = require('path');
const glob = require('glob');
const buildDir = path.resolve('./public/');
const entryDirPath = path.resolve('./Assets/Scripts/Entry/');
const scssEntryPath = path.resolve('./Assets/Sass/');
const entryFiles = path.join(entryDirPath, '**/*.js');
const { VueLoaderPlugin } = require('vue-loader');
const MiniCssExtractPlugin = require('mini-css-extract-plugin')
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const StyleLintPlugin = require('stylelint-webpack-plugin');
const TerserPlugin = require("terser-webpack-plugin");
const base64 = require('postcss-base64');

const { ProvidePlugin, DefinePlugin, SourceMapDevToolPlugin } = webpack;

const config = {
  entry: () => {
    const jsFiles = glob
      .sync(entryFiles)
      .reduce((acc, filePath) => {
        const file = path.parse(filePath);
        acc[file.name] = path.resolve(process.cwd(), filePath);
        return acc;
      }, {});

    const cssFiles = glob
      .sync(path.join(scssEntryPath, '/*.scss'))
      .reduce((acc, filePath) => {
        const file = path.parse(filePath);
        acc[file.name] = path.resolve(process.cwd(), filePath);
        return acc;
      }, {});

    return Object.assign(jsFiles, cssFiles);
  },
  optimization: {
    minimize: true,
    minimizer: [new TerserPlugin({
      extractComments: false,
      terserOptions: {
        format: {
          comments: false,
        },
      },

    })],
  },
  output: {
    filename: '[name].[contenthash].js',
    path: buildDir + '/assets/scripts/build/'
  },
  resolve: {
    alias: {
      'vue$': 'vue/dist/vue.esm-bundler.js'
    },
    extensions: ['.js', '.vue', '.json']
  },
  module: {
    rules: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: {
            presets: [
              ['@babel/preset-env', {
                useBuiltIns: 'usage',
                corejs: 3
              }]
            ]
          }
        },
      },
      {
        test: /\.(scss|css)$/,
        use: [
          {
            loader: MiniCssExtractPlugin.loader,
            options: {
              publicPath: '../../',
            },
          },
          {
            loader: 'css-loader',
            options: {
              url: false,
              importLoaders: 2,
              modules: {
                auto: true,
                localIdentName: '[name]__[local]___[hash:base64:5]',
              },
            },
          },
          {
            loader: 'postcss-loader',
            options: {
              postcssOptions: {
                plugins: [
                  base64({
                    excludeAtFontFace: false,
                    replaceValues: true,
                    extensions: ['.woff2', '.woff']
                  }),
                ]
              }
            }
          },
          {
            loader: 'sass-loader',
            options: {
              sassOptions: {
                includePaths: [
                  path.resolve(__dirname, 'node_modules')
                ],
              },
            },
          },
        ],
      },
      {
        test: /\.vue$/,
        loader: 'vue-loader'
      }
    ]
  },
  plugins: [
    new MiniCssExtractPlugin({
      filename: '../../stylesheets/[name].css',
    }),

    new VueLoaderPlugin(),

    new CleanWebpackPlugin(),

    new ProvidePlugin({
      $: "jquery",
      jQuery: "jquery"
    }),

    new DefinePlugin({
      __VUE_OPTIONS_API__: JSON.stringify(true),
      __VUE_PROD_DEVTOOLS__: JSON.stringify(false),
      __VUE_PROD_HYDRATION_MISMATCH_DETAILS__: JSON.stringify(false),
    }),
  ]
};

module.exports = (env, argv) => {
  if (argv.mode === 'development') {
    config.devtool = 'source-map';
    config.optimization.minimize = false;
  }
  if (argv.mode === 'production') {
    config.devtool = 'source-map';
  }
  if (!argv.env.ci) {
    config.devtool = 'source-map';
    config.plugins.push(
      new StyleLintPlugin({
        configFile: '.stylelintrc',
        configBaseDir: 'node_modules',
        context: "./Assets/Sass"
      }),

      new CleanWebpackPlugin(),

      new SourceMapDevToolPlugin({
        filename: '[file].map[query]',
        columns: false,
        exclude: /node_modules/,
        test: /\.(css|js)$/,
      }),

    );
  } else {
    delete config.devtool;
  }

  return config;
};

