module.exports = {
    "compile-dev-assets": [
        "compile-dev-css",
        "compile-dev-js"
    ],
    "compile-dev-js": [
        "clean:elements-js",
        "uglify:bundle-elements-dev",
        "uglify:process-standalone",
        "uglify:leaflet-bundle"
    ],
    "compile-dev-css": [
        "clean:elements-css",
        "sass:generate-dev"
    ],
    "compile-styleguide": [
        "clean:styleguide",
        "hologram:generate"
    ],
    "build-dev": [
      "clean:elements-css",
      "sass:generate-dev",
      "clean:elements-js",
      "uglify:bundle-elements-dev",
      "jshint:scripts"
    ],
    "build-prod": [
      "clean:elements-css",
      "sass:generate-prod",
      "clean:elements-js",
      "uglify:bundle-elements-prod",
      "jshint:scripts"
    ],
    "test": [
        "compile-dev-assets",
        "karma:chrome",
        "karma:firefox",
        "karma:ie"
    ],
    "test-server": [
        "compile-dev-assets",
        "karma:ie"
    ]
};
