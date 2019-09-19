Escape From Bird Version 0.2:

Changes : 
 - Difficulty can now be chosen. Upon selecting the "Play" button, you are taken to a screen to either select
a premade difficulty, or you can customize your own options. There are options for Maze Size, Enemy Speed, and 
Number of Rocks.
 - Rocks spawn randomly in the maze, and when they play gets close, a message prompt appears on screen to 
pick up the rock. This function is not yet implemented, you cannot pick up the rock.
 - Updated rendering, so larger maps don't trash the FPS.

Issues : 
 - For some reason, the player is able to travel through walls/floors in random places in the map. This bug
is very difficult to reproduce, so fix may not be incoming for a while.

Plans : 
 - Still need to add a pause menu. At the moment, the only way to close the game is through the main menu, which can only be accessed by dying,
which has proven quite hard on large maps.
 - Larger maps need to be harder. The AI currently goes around to random positions until it gets close to the player, so it is quite
possible that the player could go the entire game without ever seeing the enemy. 
 - Want to update the wall visuals. The boxes are pretty lame, and since rendering is now more optimized, I should be able to get a good 
wall model to add into the game.
 - Still need to finish the bird model. This is probably last priority (and it isn't fun).
