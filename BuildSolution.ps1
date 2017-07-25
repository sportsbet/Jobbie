param([string] $buildType = "Release")

pushd .\Src\Jobbie.Sample.UI.Host
npm install
webpack
popd

nuget restore .\Src\Jobbie.sln
dotnet build .\Src\Jobbie.sln --configuration=$buildType