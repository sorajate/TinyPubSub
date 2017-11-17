///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Default").IsDependentOn("Build");

Task("Build").Does(() =>
{
    //DotNetBuild("./src/TinyPubSub.sln");
    //DotNetCoreRestore("./src");
    //DotNetCoreBuild("./src/TinyPubSub.sln");
});

RunTarget(target);