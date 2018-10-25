# The-Temple-of-Ra
The Temple of Ra, a first person action game about fighting mummies.


Game:
Shooting with pistol deals a random amount of damage (within a specified short range of values). Hitting the head deals extra damage.
The Khopesh deals low damage to multiple opponents in front of you. Attacking opponents from behind with Khopesh deals massive damage.

Features of Khopesh:
- The khopesh has 3 different attacks:
A left slash, a right slash and a stab. The stab is composed of two separate hits, which together deal same amount of damage as the other slashes.
- The khopesh has separate animations for whether or not you have managed to hit an opponent. If hit, the khopesh stops, and player struggles to pull it back.
If the khopesh misses, it travels through whole screen without stopping.

Mummies:
- Mummies are simple chase and attack enemies (at the moment).
- They deal quite a bit of damage and can trap player in corners if they surround him.
- Mummies are slow, so you can easily outrun them.
- Mummies cannot move or rotate while attacking. This leaves them open to flanking which deals more damage (with Khopesh)

State:
PlayerController and CameraController replaced Unity's default FPSController. These new scripts should be easier to modify and extend.
PlayerController supports climbing ladders or any vertical surface with the appropriate tag. Press F while on top to climb down. 
Look at function for the CameraController for cinematic and gameplay purposes implemented.

TODO (Organized by priority):
- Mummy Priest AI (Command lesser mummies and use Health/Blood to modify Mummies)
        - Commands Normal mummies.
       		 - Normal mummies disregard personal safety to defend Priest.
	- Can turn Normal Mummies to Angered versions in turn exchange for health.
	- Can possess other mummies.
	     - Possessed mummies are stronger but will stay out of harms way.
	     - Once a possessed mummy dies, the Priest returns to previous form, minus health consumed for possession.
	    
- Make an 'angered' mummy version. Faster and stronger but is slowly depleted of health.
      - Normal mummies appear in pairs. If one is killed the other is angered.
      
- Particle effects when mummies are shot or slashes (sand cloud thing)
- Reacting animation when you shoot mummies or slash them. They exist, implement them.
- Add sounds to mummies:
  - Sounds when shot (sand pouring) + groan
  - Sound when they first notice you (some sort of surprised groad)
  - Sound when they are chasing you (an angry groan)
- Make all mummies turn to sand when they die.

- Replace FSM AI with GOAP.
- Add a tutorial for the game.
- Add something on top of Watchtower, so players aren't so dissapointed.

Future TODO:

Learn Coptic (or find someone who speaks Coptic and get him/her to teach you), record self, modulate voice and use that in place of groans.
