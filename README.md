# BetterKnucklesInteractions
## Works in Unity version 2018.2.2 and up.
### Requires the latest SteamVR version to be installed in your unity project.
A repository dedicated the the BKI library

## Library contents
### Improved hand collision
This part of the library is dedicated making fingers freeze when they collide with a mesh or pickupable object.
Can also freeze the hand of the player to prevent game-sequence breaking.
Designed to boost immersion.

### Improved input reading
At the time of making this library the knuckles controllers had the floating point inputs of individual fingers hidden in 
the endless stream of SteamVR code. This part is designed to make it easier to read out these values. (Shoutout and credits to [MayBeTall](https://github.com/MayBeTall/MayBeKnuckles) for putting in the basic work for gesture recognition).

### Gesture recognition
I've made a tool which allows any user to easily create gestures and save them. In editor you can also couple these gestures to functionality in a Start-Update-Exit function based system.

### How to install
Download and install SteamVR from the asset store.
#### The library will NOT work without it.
Download the contents of the repo and import them into the Assets folder of your project.
Make sure the resources folder is correctly merged with the resources folder your own project has (if it has one)
The folders contained in the resources folder should be in the root of the resources folder.

The knuckles controllers require the SteamVR beta (downloaded from Steam) as of January 2019.
