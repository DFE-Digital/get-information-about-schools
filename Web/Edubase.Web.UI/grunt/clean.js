module.exports = function () {
  var pathUtil = require('path');
  return {
    "elements-css": {
      force: true,
      src: pathUtil.join(process.cwd(), './public/assets/stylesheets')
    },
    "elements-js": {
      force: true,
      src: pathUtil.join(process.cwd(), './public/assets/scripts/elements.js')
    },
    styleguide: {
      force: true,
      src: pathUtil.join(process.cwd(), './public/assets/styleguide')
    }
  };
};