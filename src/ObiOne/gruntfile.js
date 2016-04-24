/// <binding BeforeBuild='cssmin, uglify' Clean='cssmin, uglify, watch' />
/*
This file in the main entry point for defining grunt tasks and using grunt plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409
*/
module.exports = function (grunt) {
    /*
    // （1）Gruntの設定（初期化）
    grunt.initConfig({
        // js圧縮
        uglify: {
            jsAsshuku: {
                files: {
                    'wwwroot/script/app.js':	// 圧縮後のファイル
                    [
                      'Script/**.js'	// 圧縮対象のファイル
                    ]
                }
            }
        },
        //css圧縮
        cssmin: {
            minify: {
                files: {
                    'wwwroot/css/app.min.css': 'css/**.css'
                }
            }
        },
        //css変更監視
        watch: {
            files: ['Css/**.css'],
            tasks: ['cssmin']
        }

    });

    // （2）プラグインのロード
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-watch');

    // （3）タスクの登録
    grunt.registerTask('default', ['uglify']);
    */


};