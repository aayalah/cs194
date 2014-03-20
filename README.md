cs194 Final Project - Bees With Jetpacks!
==========================================
Miles Johnson, Nick Yannecone, Alejandro Ayala-Hurtado




This is an overview of our Stanford Senior Project for CS194. For this project, we created a multiplayer, turn-based strategy game called Bees With Jetpacks. The game consists of players creating an army of bees (with jetpacks, of course) to fight to the death on a square, chess-like grid. Before starting the game, players are given the opportunity to customize their army by A) choosing from three different unit types and B) selecting unique skill distributions to apply to those units. The three different unit types (Bumblebee, Worker, Hornet) have a different number of hitpoints, different movement restrictions, and different attack ranges. The three skills of that unit (attack power, defense power, special power) are determined by combinations of histograms and stat distributions which the player can mix and match to his/her preference. Once the armies are built, the game begins by each player placing their units on their respective ends of the grid. The game is subdivided into rounds which consist of 3 turns per player. At the start of each round, each player selects (in order) which unit they wish to use during each turn of the round. After the order of the turns is decided, the players alternate using the units which they previously selected. On a single turn, a unit can first move (within their range and restrictions) and then either attack (within their range and restrictions) or charge their special. If the player chooses to attack, the attacking unit's damage is determined by randomly selecting a value from the attack range of its skill histogram. The defending unit determines its blocking power in a similar fashion. If the defending units blocking power is at least equal to the attackers damage, all damage is blocked. Otherwise, all damage is dealt and subtracted from the defender's total hitpoints. The game ends when all units on one team are destroyed. In the 'Game Options' menu, players can elect to play a 'King of the Hill' game mode where the players compete to control a specific tile of the grid for the most rounds. 

Bees With Jetpacks was created with the Unity game engine with scripting done in C#. The bulk of the project can be found in the Assets directory. This primarily includes textures, models, and scripts. Prefabs are "prefabricated" objects that consist of all the elements that determine the look and behavior of the object. For instance, the RedHornet prefab contains the Hornet mesh, the Red hornet texture, and the scripts that dictate the behavior of a red hornet unit in the game. It should be fairly obvious what the various prefabs and textures in the project are for. Listed below are all of the different scripts in the project and an overview of the purpose of each. For a more detailed description of their function, please refer to the scripts themselves.

Scripts:

MainMenu.cs - Handles the GUI elements of the main menu as well as their underlying logic.

GameOptions.cs - This is the primary script involved in the options menu. It creates GUI elements which allow the user to change board size, army size, game mode (Standard or King of the Hill), and whether each player is Human or AI controlled.

TeamBuilder.cs - Responsible for generating histograms and stat distributions for unit creation. Responds to user input to switch graphs/stat distrobutions and unit types, while displaying relevant information on the screen about the current unit configuration.

UnitManager.cs - Instantiated in the MainMenu scene, this script persists throughout both players unit creation scenes and the game scene itself. While TeamBuilder.cs creates the tools for creating units, UnitManager stores the data for each of the units created as well as which player each unit belongs to. When the game starts, this script instantiates an object of type Piece for each of the units created. The Player scripts can then retrieve these pieces with a call to the function GetUnit(int unitNum).

GameManager.cs - Handles all of the logic behind the game's rules. This script dictates what the players should be doing at any stage of the game while providing text prompts to guide the player. Keeps track of player turn order, piece order, units remaining per team, and things like whether the piece in play is on it's move phase or its attack phase, etc. 

Player.cs - Keeps track of all data pertaining to a single player like the units in their army. Contains methods for the actions a player can take (choosing piece placement, choosing order which pieces are to be used, etc). Also has corresponding AI methods for each function that normally requires user input.

Piece.cs - Parent of the following 3 scripts. Contains all the underlying data of a unit from attack and defense histograms, to number of remaining hitpoints, to current board position. Has User and AI functions for moving and attacking with pieces.

BrutePiece.cs - Brutes (Bumblebees) are units with lots of hitpoints, short movement range, and short attack range. This script overrides the getMoveLocations() and getAttackableTiles() methods of Piece.cs. The bumblebee can move to any space within a 5 square radius and can only attack adjacent units.

GruntPiece.cs - Grunts (Workers) are overall balanced units with moderate hitpoints, movement range and attack range. This script overrides the getMoveLocations() and getAttackableTiles() methods of Piece.cs. The worker can move up to 5 spaces away but can only move vertically or horizontally like a chess rook and can only attack vertically and horizontally as well. 

RangerPiece.cs - Rangers (Hornets) are high range, low durability units. They have very low hitpoints, but can move and attack over a long range. This script overrides the getMoveLocations(), getAttackableTiles(), and attack() methods of Piece.cs. The hornet can move up to 8 diagonal spaces away and can attack horizontally or vertically up to 6 spaces away. The hornet launces a projectile when attacking, so it must override the attack() function as well. 

HealthBar.cs - Simply keeps track of the amount of health a unit has and displays a red and green healthbar over the unit when the user hovers their mouse over it. 

CameraMovement.cs -

Clock.cs -

ClockKing.cs -

GridController.cs - 

TileController.cs -


