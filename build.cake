// #load "./../PleOps.Cake/src/PleOps.Cake/targets.cake"
#load "nuget:?package=PleOps.Cake&prerelease"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.CoverageTarget = 80;
    info.PreviewNuGetFeed = "https://nuget.pkg.github.com/Kaplas80/index.json";
    info.PreviewNuGetFeedToken = info.GitHubToken;
    info.StableNuGetFeed = "https://nuget.pkg.github.com/Kaplas80/index.json";
    info.StableNuGetFeedToken = info.GitHubToken;

    info.AddLibraryProjects("src/Libraries/TF3.Common.Yakuza");
    info.AddLibraryProjects("src/Plugins/TF3.Plugin.YakuzaKiwami2");
    info.AddApplicationProjects("src/Apps/TF3.CommandLine");
    info.AddTestProjects("src/Tests/TF3.Tests.Yakuza");
});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);
