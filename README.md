# Translation Framework v3 (Powered by [Yarhl](https://scenegate.github.io/Yarhl/))
[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/) ![Build and release](https://github.com/kaplas80/TF3/workflows/Build%20and%20release/badge.svg)

## Current State

This framework is in ***alpha*** state and under active development. There might be breaking changes at any moment.

## Plugins

Currently, 2 games are supported:

* [Yakuza Kiwami 2 (Steam)](https://github.com/Kaplas80/TF3.YakuzaPlugins)
* [Zwei: The Arges Adventure (Steam)](https://github.com/Kaplas80/TF3.ZweiPlugins)

## Requisites

* [.NET 6.0 runtime](https://dotnet.microsoft.com/en-us/download)

## Usage

**NOTE: This is a command line application. "Double clicking" on the exe is not enough.**

1. Create `plugins` and `scripts` directories inside the app folder.
2. Copy plugins files to the respective folders. See each plugin README for details.
3. Run the app.

### List available plugins

```shell
TF3.CommandLine.exe listscripts
```

### Extract assets

```shell
TF3.CommandLine.exe extract --script [script-name] --install-dir [game-files-directory] --output-dir [output-directory]
```

### Rebuild assets

```shell
TF3.CommandLine.exe rebuild --script [script-name] --install-dir [game-files-directory] --translation-dir [translation-files-directory] --output-dir [output-directory]
```

## FAQ

If you have questions, see [FAQ](https://github.com/Kaplas80/TF3/blob/main/FAQ.md) or open a new thread in [Discussions](https://github.com/Kaplas80/TF3/discussions)

## Credits

* Thanks to Pleonex for [Yarhl](https://scenegate.github.io/Yarhl/) and [PleOps.Cake](https://www.pleonex.dev/PleOps.Cake/).
* Other libraries used: [CommandLineParser](https://github.com/commandlineparser/commandline), [Dahomey.Json](https://github.com/dahomey-technologies/Dahomey.Json), [xxHash](https://github.com/uranium62/xxHash), [BCnEncoder.Net](https://github.com/nominom/bcnencoder.net), [ImageSharp](https://sixlabors.com/products/imagesharp/)
* Icon by [Papirus Development Team](https://github.com/PapirusDevelopmentTeam/papirus-icon-theme/).
