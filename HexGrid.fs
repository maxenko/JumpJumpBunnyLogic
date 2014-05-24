namespace JumpJumpBunny

type HexGrid(rows,columns,maxCellSize,minCellSize) = 
    
    let map = new HexGridMap(rows,columns,maxCellSize)
    let coords = 
        let cs = map.MakeCells()
        cs
    let coordsArray = Seq.toArray(coords)
    let islands = [ for c in coords do yield new Island( c, maxCellSize, minCellSize, maxCellSize, IslandDifficulty.Challenging, 3000.f ) ]
    let islandsArray = Seq.toArray(islands) 

    //member x.Map with get() = map
    member x.Coordinates with get() = coordsArray
    member x.Islands with get()     = islandsArray
