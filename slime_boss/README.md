## Slime Boss for an Indie Platformer Game

## Introduction
I had the opportunity to work on a unique boss mechanic for an indie platformer game developed by a small studio in Japan. <br />
The goal was to create a challenging yet enjoyable experience for players by designing the Slime Boss, one of the first to many key antagonist in the game.

## Features and Mechanics
### Jump Mechanic:<br />
The Slime Boss can perform a triple jump to reach the player's position, adding a layer of unpredictability to the battle. <br />
### Vulnerability Phase: <br />
After three jumps, the boss enters a vulnerable state, allowing players to attack. <br />
### Knockback Reaction: <br />
When hit, the boss jumps back, maintaining its focus on the player. <br />
### Health System: <br />
The boss has a health system that triggers different behaviors based on its remaining health. <br />

## Technologies Used
Unity 3D <br />
C# scripting <br />
Rigidbody physics <br />

## Challenges and Solutions

#### Enemy Landing in the Air: <br />
Initially, if the player jumped and the enemy jumped at the player's location while in the air, the enemy would land in the air. I solved this by adjusting the enemy's landing position to ensure it always landed on the floor.

#### Enemy Rotation on Landing: <br />
I noticed that if the player was in the air and the slime jumped, the slime's rotation would be facing upward when it landed, which looked odd. I fixed this by resetting the enemy's rotation to be aligned with the ground after landing.

#### Enemy Pushed Downwards Through Floor: <br />
When in the vulnerable state, if the slime was attacked, it would get pushed back and downwards through the floor rather than just straight back. I solved this by adjusting the enemy's position and animation to ensure it jumped back without moving downwards.

#### Maintaining Enemy Size Without Clipping: <br />
I needed to increase the size of the boss without causing it to clip through the floor. I solved this by adjusting the collider's dimensions and ensuring the enemy's position was set correctly to prevent clipping.

#### Adding Landing Smoke Effect: <br />
I wanted to add a particle effect for when the boss lands. I solved this by creating a method to instantiate the particle effect at the landing position and adjusting its size and position to match the boss's landing.

#### Adding a Stunned Effect Around the Boss: <br />
I wanted to add an effect to visually indicate when the boss is stunned. I solved this by creating a method to instantiate a particle effect above the boss's head and programming it to move in a circle to simulate a "birdy" effect.

#### Ensuring Boss Dies Properly: <br />
I encountered an issue where the boss would keep looping between attack and vulnerable states even after dying. I solved this by adding a check in the vulnerable state to transition to a dead state and prevent further state transitions when the boss's health reached zero.

#### Preventing Boss From Sinking Into Floor: <br />
I noticed that the boss would sink into the floor after certain actions like jumping or getting knocked back. I solved this by storing the original Y position of the boss and ensuring it was reset to this position after each action that could cause sinking.

#### Preventing Kinematic Rigidbody Warning: <br />
I encountered a warning message about setting the linear velocity of a kinematic Rigidbody. I solved this by checking if the Rigidbody was kinematic before setting its velocity to zero.

#### Smooth Transition to Original Y Position: <br />
I wanted to ensure that the boss smoothly transitioned to its original Y position after jumping or getting knocked back, instead of instantly blinking to that position. I solved this by using a coroutine to interpolate the boss's position back to the original Y position over a short duration.

#### Stopping Sound Effects on Death: <br />
I encountered an issue where sound effects, such as the stun sound, would continue playing even after the boss died. I solved this by stopping the sound effects in the OnStep method of the VulnerableState when the boss's health was empty.

#### Playing Death Sound: <br />
I wanted to play a death sound when the boss's health reached zero. I solved this by adding a line to play the death sound clip in the OnStep method of the VulnerableState when the boss's health was empty.

#### NullReferenceException on Player Death: <br />
I encountered a NullReferenceException when the player died while the boss was in the vulnerable state. I solved this by adding a null check for the player before accessing its position in the StunCoroutine.

#### Maintaining Boss Position After Knockback: <br />
I noticed that the boss would sink further into the ground after each knockback. I solved this by ensuring that the boss's Y position was set back to the original position after the knockback animation.

## Screenshots and Videos <br />
https://youtu.be/_gcrVDS0MgM

## Conclusion
Working on the Slime Boss mechanics was a rewarding experience that allowed me to explore complex AI behaviors and contribute to an exciting indie game.
