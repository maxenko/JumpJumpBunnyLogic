namespace JumpJumpBunny

type HexGrid(rows,columns,maxCellSize:single,minCellSize:single) = 
    let map = new HexGridMap(rows,columns)
    
    let coords = 
        let cs = map.MakeCells()
        cs

    let coordsArray = Seq.toArray(coords)
    let islands = [ for c in coords do
                    let gridCellSize2 = Math.rndBetweenF minCellSize maxCellSize
                    yield 
                        new Island( c, gridCellSize2, minCellSize, maxCellSize, IslandDifficulty.Easy, 3000.f ) 
    ]
    let islandsArray = Seq.toArray(islands) 

    //member x.Map with get() = map
    member x.Coordinates with get() = coordsArray
    member x.Islands with get()     = islandsArray
    member x.GetRandIsland() = Math.randEl coords
