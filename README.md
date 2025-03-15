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
