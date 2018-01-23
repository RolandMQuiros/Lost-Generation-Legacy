You are an eager new adventurer, fresh from the wizardry academy, the knighthood apprenticeship, the hunter's guild, ready to take to the dungeons and forge your name among Del Abismo's greatest heroes!

But first, you need to pay off your student loans.

# Lost-Generation

Lost Generation is a roguelite dungeon crawler without permadeath, where you can lose everything except the friends you make along the way.

- Plumb the procredurally-generated dungeon depths with up to five party members

- Fight the unforgiving hordes in a unique, simultaneously-resolving tactical combat system

- Build a community of fellow impoverished adventurers with characters with dynamic personalities, schedules, and goals

- Form bonds and rivalries; earn respect or disdain; find thrills, frustration, love, and purpose. And maybe, if you're lucky, some financial stability.

# Technical Features 

## Voxel Renderer
The [BlockMesh](Assets/LostGeneration/Scripts/Display/BlockMesh.cs) MonoBehaviour can generate voxel-like geometry based on 3D byte arrays, creating meshes not unlike the terrain in Minecraft.

![Voxel Terrain](https://i.imgur.com/i5PFDpL.png)

![Voxel Textures](https://i.imgur.com/GY7lHWs.png) ![AutoTile textures](https://i.imgur.com/Ydcvsl0.png)

In addition, each side of each block is dynamically textured with autotiles, which change according to adjacent block types. I use the same approach to AutoTiles as [RPG Maker VX and up](http://blog.rpgmakerweb.com/tutorials/anatomy-of-an-autotile/).

## Character Customization
Every "human" character you meet in the game will be either procedurally generated or created by other players. Using a large number of blend shapes and control bones on a base human mesh, players will be able to greatly customize their avatar's appearance.

![Character Customization](https://i.imgur.com/7jx2Qq0.gif)

The screen above also generates sliders based on the blend shape and control bone names, so changes to the source FBX files will immediately be reflected in the customizer.

## Planning AI Systems
All NPCs will use [Goal-Oriented Action Planning](http://alumni.media.mit.edu/~jorkin/goap.html) to determine their actions both in and out of the dungeon. This lets the game create interesting (not necessarily *optimal*) behaviors with, ideally, less hand-crafted content.

My implementation includes some small optimizations inspired by [Hierarchical Task Networks](http://www.gameaipro.com/GameAIPro/GameAIPro_Chapter12_Exploring_HTN_Planners_through_Example.pdf).

## Basic Combat Architecture
When dungeon-diving, players control up to five party members at once in a turn-based tactical combat system. Unlike most tactics games, turns resolve "simultaneously".

![Simultaneous Resolution Combat](https://i.imgur.com/ZUoKWnh.gif)

The system supporting the above is a general roguelike framework, completely decoupled from Unity, that supports action-based state changes and undo/redo stacks.

## A Lotta Testing
![Unit Tests](https://i.imgur.com/bnvwC0Q.png)

The transactional nature of turned-based games make them similar to more traditional apps. This lets me test core game logic more in managable pieces, more often than I would with realtime games.
