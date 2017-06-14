param([string] $buildType = "Release")

& ".\Src\Jobbie.Infrastructure.DatabaseMigrations\bin\$buildType\FluentMigratorTools\Migrate.exe" `
    -a ".\Src\Jobbie.Infrastructure.DatabaseMigrations\bin\$buildType\Jobbie.Infrastructure.DatabaseMigrations.dll" `
    --dbType SqlServer2014 `
    --conn Development