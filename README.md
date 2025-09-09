<div align="center"><strong>Conway's Game of Life</strong></div>
<div align="center">Initial take </div>
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
- Upload Bulk Board State - [Next.js (App Router)](https://nextjs.org)
- Get Next State - [TypeScript](https://www.typescriptlang.org)
- Get N States Ahead - [Auth.js](https://authjs.dev)
- Get Final State - [Postgres](https://vercel.com/postgres)

###Non-Functional Requirements
- Persist board states - in lue of documentdb\blob access, saving the board states as json files
- production-ready - excluding the file based state, should be good to go.
  - clean, modular, testable -> DI with moqable interfaces for external resources
  - error & validation -> some basics, but could be cleaned up a bit
  - c# best practices -> 

## Getting Started decision points
- The first decision is really to decide how to structure the grid.  
  - Top Left Anchor [0,0]: The more common approach seems to start with position 0, 0 in the top left.  This would allow for infinite increasing numbers right and down.  
  - Center Anchor [0,0]: Starting with 0, 0 as the center would make it easier to create symmetrical patterns, but may be more difficult to interpret external patterns.
- Decide on what the Api Request\Response contract
- Find a solution for managing state
- 


## Getting Started

- Started by creating a solution with a domain project to store all the domain specific logic and data structures.  -
- Added Unit test project to allow a test driven approach as executing logic.
- Added in WebApi with swagger to expose endpoints


```
use postman \ curl to run
```


You should now be able to access the application at http://localhost:5276.