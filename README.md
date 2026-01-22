# Fountain of Objects - C# Player's Guide

## Summary
My attempt at the Fountain of Objects set of challenges based on RB Whitaker's book, C# Player's Guid (https://csharpplayersguide.com/).

The challenge was to create a console app to simulate a dungeon crawl through a grid of connected rooms. The player is able to input text commands (as well as asking for help) to navigate through the level. The objective is to find the Fountain within the level, activate it and return to the entrance.

> N.B. entrance, fountain and monster locations are hard-coded at the moment.

This project also includes completed versions of all of the Expansion challenges:
* dynamic map size (small, medium and large) that is set by the player on starting the game,
* pit rooms that kill the player on entry,
* maelstrom monsters that cause the player to be relocated in the dungeon (clamped within dungeon extents),
* rooms able to contain monsters; if the player enters a room containing a monster, the player dies resulting in game-over,
* weapon (bow and arrow) that the player can use to shoot into adjacent rooms, killing monsters if present and making the room safe,
* a help system that provides more explicit instruction.

## Learnings
I really felt this project was a good learning experience with regards to OOP. Ensuring good separation across classes and use of polymorphism for the room and monster inheritance. Use of enums to constrain possible values and structs to pass around small data packages. With that said, inheritance and object initialisers is an area that I need to read over again.

Features that I wasn't quite able to utilise:
* interfaces,
* use of the static keyword (an area I would like to improve understanding)
* generics (through lack of understanding mechanics, but also where best to use them)

I also think I ended up with quite a bloated GameManager class that would benefit from some functionality being moved to other or new classes.

I would probably refactor the code to present the player with a list of possible actions (move, shoot etc.) to remove ambiguity and reliance on string checking but given the challenge I feel like I created a robust solution.
