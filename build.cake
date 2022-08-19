#load "nuget:?package=PleOps.Cake&version=0.7.0"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.CoverageTarget = 100;
    info.PreviewNuGetFeed = "https://nuget.pkg.github.com/Kaplas80/index.json";
    info.PreviewNuGetFeedToken = info.GitHubToken;
    info.StableNuGetFeed = "https://nuget.pkg.github.com/Kaplas80/index.json";
    info.StableNuGetFeedToken = info.GitHubToken;

    info.AddLibraryProjects("src/Libraries/TF3.Core/TF3.Core.csproj");
    info.AddApplicationProjects("src/Apps/TF3.CommandLine/TF3.CommandLine.csproj");
    info.AddTestProjects("src/Tests/TF3.Tests/TF3.Tests.csproj");
});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);
