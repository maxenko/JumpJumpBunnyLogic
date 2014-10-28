namespace JumpJumpBunny

type HexGrid(radius,maxCellSize,minCellSize:single) = 
    let map = new HexGridMap(radius,maxCellSize)
    
    let coords = 
        let cs = map.MakeCells()
        cs

    let coordsArray = Seq.toArray(coords)
    let islands = [ for c in coords do
                        let gridCellSize2 = Math.rndBetweenF minCellSize maxCellSize
                        yield 
                            new Island(null, c, gridCellSize2, minCellSize, maxCellSize, IslandDifficulty.Effortless, 3000.f ) 
    ]
    let islandsArray = Seq.toArray(islands)

    //let hasNeighbor HexCoord d = 
        

    //member x.Map with get() = map
    member x.Coordinates with get() = coordsArray
    member x.Islands with get()     = islandsArray
    member x.GetRandIsland()        = Math.randEl coords
    member e.IslandExistsAt(x,y,z)  = 
        Seq.find (fun (i:HexCellCoord) -> i.HexX = x && i.HexY = y && i.HexZ = z ) e.Coordinates
