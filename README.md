# ADGAC Giant Mod
This mod makes you bigger, like huge even

## Installation and Usage
This mod is built in the BepInEx framework, that means you'll need to [install BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html) version 5.\
Once you've done that, navigate to the [releases](https://github.com/BarackOBusiness/ADGACGiantMod/releases) section of this repository and download the latest version of the mod.\
Place the download in the `BepInEx/plugins` folder inside of your game's root folder, and you're done with installation.

## Compilation
Make sure you have a dotnet sdk installed, version 7.x is verified to work here.\
Clone the repository, and create the directory `lib/netstandard2.1` within the project root, copy ADGACs Assembly-CSharp.dll into it.\
Use `dotnet build` to compile the project, and the dll can then be found in the `bin/Debug/netstandard2.1` folder
