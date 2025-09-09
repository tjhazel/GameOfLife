<div align="center"><strong>Conway's Game of Life</strong></div>
<div align="center">A take on how to implement the game.</div>
<br />
<div align="center">
<a href="https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life">Wiki</a>
<span> · </span>
<a href="https://conwaylife.com/wiki/Pattern_of_the_Year">Pattern of the Year</a>
<span>
</div>

## Overview

###Functional Requirements

Requirements are to build this using c# net8.0.

The API should include (at a minimum) the following endpoints:
- UploadBoardState - Upload Bulk Board State - used file system :(
- GetNextBoardState - Get Next State - interate one step at a time
- GetBoardStateAtTick - Get N States Ahead - will restart from original
- GetFinalState - Get Final State - will restart from original

###Non-Functional Requirements
- Persist board states - in lue of documentdb\blob access, saving the board states as json files
- production-ready - excluding the file based state, should be good to go.
  - clean, modular, testable -> to a point - it is testable, but not tested -> implementing tests, especailly around the GridService would likely result in refactoring
  - error & validation -> some basics, but could be cleaned up a bit
  - c# best practices -> yes (without a 100% confidence level)

## Getting Started decision points
- The first decision is really to decide how to structure the grid.  
  - Top Left Anchor [0,0]: The more common approach seems to start with position 0, 0 in the top left.  This would be easier to implement in a reasonable amount of time.  
  - Center Anchor [0,0]: Starting with 0, 0 as the center would make it easier to create symmetrical patterns, but may be more difficult to interpret external patterns.
  - Fixed vs. infinate grid -> this solution uses a fixed grid to stay within the allotted time.
- Api Request\Response contract
  - Tried to pull some ideas from the interweb & landed on a clean & easy to work with visual pattern
  - WebApi really built on top of the full soluion, which would make it easy to add console, function, or other entry points
  - Wired up swagger to provide an easy f5 experience to test the methods.
- Managing state
  - Used local file system to make the solution more portable
  - To hose in Azure\AWS, a document\blob database would be a clean fit

## Getting Started

- Clone the repo locally
- open the solution in visual studio
- set the GameOfLife.Api project as the startup
- run in debug mode to launch swagger
- Use Swagger to call api methods - convience methods make the eval process easier
  - GetOriginalBoardState - pull original json payload or some of the pre-loaded games
  - GetCurrentBoardState - pull latest payload that includes latest game state
  - ResetBoard - reset state on an existing game  
  - GetGameList - pulls a list of games to interact with
- Added in WebApi with swagger to expose endpoints
- Added Unit test project with good intentions, but pulled the plug after swagger was installed

You should now be able to access the application at https://localhost:7247/index.html.