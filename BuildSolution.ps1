param([string] $buildType = "Release")

pushd .\Src\Jobbie.Sample.UI.Host
msbuild -t:restore
npm install
webpack
popd

nuget restore .\Src\Jobbie.sln
msbuild -t:build .\Src\Jobbie.sln -p:configuration=$buildType