'use strict'

var gulp = require("gulp"),
    debug = require("gulp-debug"),
    rename = require('gulp-rename'),
    babel = require("gulp-babel");

let {clean, restore, build, test, pack, publish, run} = require('gulp-dotnet-cli');


process.env.NODE_ENV = 'production';

gulp.task("transpile", function () {
    return gulp.src(["./**/*.jsx"], { base: "." })
        //.pipe(debug({ title: 'babel:' }))
        .pipe(babel({
            presets: ["react-app"]
        }))
        .pipe(rename({prefix:"_compiled_"}))
        .pipe(gulp.dest("."))
        .pipe(debug({ title: 'babel:' }));
});
