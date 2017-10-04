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
