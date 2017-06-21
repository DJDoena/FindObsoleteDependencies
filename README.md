# FindObsoleteDependencies

FindObsoleteDependencies is a code project that inspects your C# project files and their registered dependencies against the actual dependencies in your compiled DLL.

This way you can remove unecessary dependencies and enhance the performance of your build process, especially if you build on multiple cores.