// #load "./../PleOps.Cake/src/PleOps.Cake/targets.cake"
#load "nuget:?package=PleOps.Cake&prerelease"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("MyLibrary");
    info.AddApplicationProjects("MyConsole");
    info.AddTestProjects("MyTests");

    // No need to set if you want to use nuget.org
    info.PreviewNuGetFeed = "https://pkgs.dev.azure.com/benito356/NetDevOpsTest/_packaging/Example-Preview/nuget/v3/index.json";
    info.StableNuGetFeed = "https://pkgs.dev.azure.com/benito356/NetDevOpsTest/_packaging/Example-Preview/nuget/v3/index.json";
});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);
