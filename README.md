# The-Temple-of-Ra
The Temple of Ra, a first person action game about fighting mummies.

PlayerController and CameraController fully working.
Functionality of Unity FirstPersonController successfully replicated and extended to support climbing ladders, 
or any vertical surface with the appropriate tag. 
This new system should be a bit easier to modify and extend.
Player can now climb down ladders/walls.
Look at function for the CameraController for cinematic and gameplay purposes implemented

TODO:
-Fix Visual Glitch when climbing down ladders.
-Fix bug, where while climbing down the ladder, if you look down, you fall off.
-Make ladder/wall climbing independent of camera (y rotation of player), but have vision constricted to 180 degrees.
-Add a tutorial for the game.

Future TODO:

-Add sounds to mummies:
  -Sounds when shot (sand pouring) + groan
  -Sound when they first notice you (some sort of surprised groad)
  -Sound when they are chasing you (an angry groan)
-Particle effects when mummies are shot or slashes (sand cloud thing)
-Reacting animation when you shoot mummies or slash them. They exist, implement them.

Learn Coptic (or find someone who speaks Coptic and get him/her to teach you), record self, modulate voice and use that in place of groans.

-Replace FSM AI with GOAP.
-Remake the Priest AI.
-Make an 'angered' mummy version. Faster and stronger but is slowly depleted of health. (maybe they are pouring sand, and when they die they collapse into a pile of ash)
-Make all mummies turn to sand when they die.
