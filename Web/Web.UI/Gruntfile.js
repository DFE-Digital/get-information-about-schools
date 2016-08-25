/// <vs BeforeBuild='compile-dev-assets' />
module.exports = function (grunt) {
    var pathUtil = require("path");

    require('load-grunt-config')(grunt, {
        configPath: pathUtil.join(process.cwd(), 'Grunt'),
        // config: require("./grunt/environment")(grunt, __dirname)
    });
};
