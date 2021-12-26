# TF3 FAQ

## What is TF3?

TF3 is a framework designed to make easier the translation of games.

## How does it works?

The steps to translate any game are (very often) the same:

1. Extract the texts (or images, or sounds...) from the original game files.
2. Translate.
3. Rebuild the game files using the translation.

TF3 automatize the steps 1 and 3 by using scripts and plugins.

## Does it work with [insert game name here]?

No, each game is different and TF3 needs to have specific [Yarhl Converters](https://scenegate.github.io/Yarhl/guides/Yarhl-nutshell.html#converters-putting-together-all-the-pieces) to be able to extract and repack the files.

The goal of TF3 is to get rid of the common tasks and let the developer focus in the game specific ones.

## Can you create the converters for [insert game name here]?

Short answer: No.

Long answer: I do this in my spare time, and it isn't too much, so I focus in the games I'm interested. But my goal is that anyone (with the necessary coding skills) can create their own plugins and use them with TF3.

## But, I'm very interested in translating [insert game name here] and I can pay you $[insert amount here]!

I'm sorry, but I don't do this for money.

## To what languages can I translate the supported games?

With my plugins, you should be able to translate, at least, to Spanish.

I try to make the plugins language independent but, often, the limitation is in the game itself.

For example, Yakuza Kiwami 2 use a variable width font for the characters in ASCII range from 0x20 to 0x7F and a fixed width font for any other character in UTF8. Modifying the .exe file, I'm able to use the variable width font for the characters in range 0x20 to 0xFF, leaving enough characters to translate to Spanish but, probably, insufficient for other languages.

In cases like this, if my plugin is not enough, you'll have to code your own plugin or patch the .exe by yourself.

## There is a plugin to translate the PC version of a game, but I want to translate the console version. Does it work?

I don't know. Sometimes, the files in both versions are the same (or there are minor changes like the file path) and, in this cases, I'll try to do my best to be able to translate it.

## Can I help you with the project?

Of course, you are welcome! You can fork the project and create pull requests to submit changes. Or you can create plugins for new games.

## I want to translate a pirate version of a game, but TF3 throws some errors and it doesn't work...

I do not support piracy, so you are on your own.

## I double click TF3.exe and it opens and closes immediately.

That's normal. TF3 is a command line app, so you need a (very) basic computer knowledge to open a command line window and run the app in it.

Probably, in the future, there will be a GUI.

## I'm translating a game but there are missing texts (or images...) in the extracted files.

Probably I missed some files when I created the plugin. Please, open an issue in the github page of the plugin indicating the **exact** missing text (a screen capture is better).

## I have found a bug in the app. How do I report it?

If you find a bug, please open an issue [here](https://github.com/Kaplas80/TF3/issues). I can't fix a bug if I'm not able to reproduce it, so, please, add much info as possible.

## I have other question, how can I contact you?

You can ask your questions [here](https://github.com/Kaplas80/TF3/discussions).