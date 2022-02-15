<img align="right" src="Nodsoft.WowsReplaysUnpack/logo.png" alt="logo" width="200"/>

# WoWS-ReplaysUnpack
A C# file unpacking library for World of Warships Replays, inspired by Monstrofil's [replays_unpack](https://github.com/Monstrofil/replays_unpack/).

## Information before using the library
The library supports only World of Warships replays starting with version 0.10.10. 
Trying to use an older replay can result in unexpected errors when processing the replay.

## How to use
To analyze a replay, simply use the ReplayUnpacker:
```c#
ReplayUnpacker unpacker = new();
using Stream replayFileStream = File.OpenRead("<Path to replay file>");
ReplayRaw unpackedReplay = unpacker.UnpackReplay(replayFileStream);
```

An instance of `ReplayRaw` contains a list of all players involved in the current match and a list of all player chat messages.

Information about the current configuration of a player's ship is available using the `ShipData` property of a `ReplayPlayer` from the extracted replay data.
Note that this property is a lazy property, meaning that it is constructed when it is accessed for the first time.
Construction of this property can take a bit longer than usual due to it having to decode serialized data.
To avoid impacting the replay unpack performance due to a feature that is not always necessary, this property is not created when initializing the player object itself.

## Advanced use
To use a custom replay reader implementation, provide the replay unpacker with an implementation of the `IReplayParserProvider` interface.
```c#
ReplayUnpacker unpacker = new();
IReplayParserProvider provider = new MyCustomReplayParserProvider();
using Stream replayFileStream = File.OpenRead("<Path to replay file>");
ReplayRaw unpackedReplay = unpacker.UnpackReplay(replayFileStream, provider);
```
