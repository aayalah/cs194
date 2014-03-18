cs194 Final Project - Bees With Jetpacks!
==========================================
Miles Johnson, Nick Yannecone, Alejandro Ayala-Hurtado




This is an overview of our Stanford Senior Project for CS194. For this project, we created a multiplayer, turn-based strategy game called Bees With Jetpacks. The game consists of players creating an army of bees (with jetpacks, of course) to fight to the death on a square, chess-like grid. Before starting the game, players are given the opportunity to customize their army by A) choosing from three different unit types and B) selecting unique skill distributions to apply to those units. The three different unit types (Bumblebee, Worker, Hornet) have a different number of hitpoints, different movement restrictions, and different attack ranges. The three skills of that unit (attack power, defense power, special power) are determined by combinations of histograms and stat distributions which the player can mix and match to his/her preference. Once the armies are built, the game begins by each player placing their units on their respective ends of the grid. The game is subdivided into rounds which consist of 3 turns per player. At the start of each round, each player selects (in order) which unit they wish to use during each turn of the round. After the order of the turns is decided, the players alternate using the units which they previously selected. On a single turn, a unit can first move (within their range and restrictions) and then either attack (within their range and restrictions) or charge their special. If the player chooses to attack, the attacking unit's damage is determined by randomly selecting a value from the attack range of its skill histogram. The defending unit determines its blocking power in a similar fashion. If the defending units blocking power is at least equal to the attackers damage, all damage is blocked. Otherwise, all damage is dealt and subtracted from the defender's total hitpoints. The game ends when all units on one team are destroyed. In the 'Game Options' menu, players can elect to play a 'King of the Hill' game mode where the players compete to control a specific tile of the grid for the most rounds. 

Bees With Jetpacks was created with the Unity game engine with scripting done in C#. The bulk of the project can be found in the Assets directory. This primarily includes textures, models, and scripts. Prefabs are "prefabricated" objects that consist of all the elements that determine the look and behavior of the object. For instance, the RedHornet prefab contains the Hornet mesh, the Red hornet texture, and the scripts that dictate the behavior of a red hornet unit in the game. It should be fairly obvious what the various prefabs and textures in the project are for. Listed below are all of the different scripts in the project and an overview of the purpose of each. For a more detailed description of their function, please refer to the scripts themselves.

Scripts:

MainMenu.cs -

GameOptions.cs -

TeamBuilder.cs -

UnitManager.cs -

GameManager.cs -

Player.cs -

Piece.cs -

BrutePiece.cs -

GruntPiece.cs -

RangerPiece.cs -

HealthBar.cs -

CameraMovement.cs -

Clock.cs -

ClockKing.cs -

GridController.cs -

TileController.cs -


