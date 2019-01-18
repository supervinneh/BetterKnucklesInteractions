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
the endless stream of SteamVR code. This part is designed to make it easier to read out these values. (Shoutout and credits to [MayBeTall](https://github.com/MayBeTall/MayBeKnuckles) for putting in the basic work for gesture recognition.

### Gesture recognition
I've made a tool which allows any user to easily create gestures and save them. In editor you can also couple these gestures to functionality in a Start-Update-Exit function based system.
