#load "version.cake"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var incremental = Argument("incremental", false);

var version = BuildVersion.Calculate(Context);

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context => {
    Information("Version = {0}", version.SemVersion);
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Pack");

Task("Clean")
    .WithCriteria(() => !incremental)
    .Does(() =>
{
    CleanDirectory("./.artifacts");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => 
{
    NuGetRestore("./src/TinyPubSub.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    MSBuild("./src/TinyPubSub.sln", settings => 
        settings
            .WithProperty("Version", version.SemVersion)
            .WithProperty("AssemblyVersion", version.Version)
            .WithProperty("FileVersion", version.Version)
            .SetConfiguration($"{configuration}-CI")
            .SetVerbosity(Verbosity.Minimal));
});

Task("Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest($"./src/TinyPubSub.Tests/TinyPubSub.Tests.csproj", 
        new DotNetCoreTestSettings {
            NoBuild = true,
            Configuration = configuration
        });
});

Task("Pack")
    .IsDependentOn("Unit-Tests")
    .Does(() =>
{
    DotNetCorePack("./src/TinyPubSub/TinyPubSub.csproj", new DotNetCorePackSettings {
        Configuration = "Release-CI",
        OutputDirectory = "./.artifacts",
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", version.SemVersion)
            .WithProperty("AssemblyVersion", version.Version)
            .WithProperty("FileVersion", version.Version)
    });

    // DotNetCorePack("./src/TinyPubSub.Forms/TinyPubSub.Forms.csproj", new DotNetCorePackSettings {
    //     Configuration = "Release-CI",
    //     OutputDirectory = "./.artifacts",
    //     MSBuildSettings = new DotNetCoreMSBuildSettings()
    //         .WithProperty("Version", version.SemVersion)
    //         .WithProperty("AssemblyVersion", version.Version)
    //         .WithProperty("FileVersion", version.Version)
    // });
});

RunTarget(target);