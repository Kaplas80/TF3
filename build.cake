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

    info.AddLibraryProjects("src/Libraries/TF3.Common.Yakuza/TF3.Common.Core.csproj");
    info.AddLibraryProjects("src/YarhlPlugins/TF3.YarhlPlugin.YakuzaCommon/TF3.Plugin.YakuzaCommon.csproj");
    info.AddLibraryProjects("src/YarhlPlugins/TF3.YarhlPlugin.YakuzaKiwami2/TF3.Plugin.YakuzaKiwami2.csproj");
    info.AddLibraryProjects("src/Plugins/TF3.Plugin.YakuzaKiwami2/TF3.Plugin.YakuzaKiwami2.csproj");
    info.AddApplicationProjects("src/Apps/TF3.CommandLine/TF3.CommandLine.csproj");
    info.AddTestProjects("src/Tests/TF3.Tests.Yakuza/TF3.Tests.Yakuza.csproj");
    info.AddTestProjects("src/Tests/TF3.Tests.YakuzaKiwami2/TF3.Tests.YakuzaKiwami2.csproj");
});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);
