namespace JumpJumpBunny

open System

type HexVector =
    static member mask =
        [|
            [|0;1;-1|]; [|-1;1;0|]; [|-1;0;1|]; // nw w sw
            [|1;0;-1|]; [|1;-1;0|]; [|0;-1;1|]  // ne e se
        |]
    static member E     = HexVector.mask.[4]
    static member NE    = HexVector.mask.[3]
    static member NW    = HexVector.mask.[0]
    static member W     = HexVector.mask.[1]
    static member SW    = HexVector.mask.[2]
    static member SE    = HexVector.mask.[5]

    static member GetVector (d:Direction) =
        match d with
        | NE when d = Direction.NE -> HexVector.NE
        | NW when d = Direction.NW -> HexVector.NW
        | E when d  = Direction.E -> HexVector.E
        | W when d  = Direction.W -> HexVector.W
        | SE when d = Direction.SE -> HexVector.SE
        | SW when d = Direction.SW -> HexVector.SW
        | _ -> HexVector.E

    static member GetNeighborCoordinates(x,y,z,d) =
        let v = HexVector.GetVector(d)
        [|x+v.[0];y+v.[1];z+v.[2]|] 

type HexCellNeighbors<'n>(cell:'n) =
    let parent = cell
    member val NE   = cell with get, set
    member val NW   = cell with get, set
    member val E    = cell with get, set
    member val W    = cell with get, set
    member val SE   = cell with get, set
    member val SW   = cell with get, set
      
type HexCellCoord(hexX : int, hexY : int, hexZ : int, ?x : single, ?y : single) =

    static member Nothing = new HexCellCoord(-1,-1,-1,100000.f,1000000.f) // placeholder, representing non-location
    
    // add to collection
    member val HexX = hexX with get
    member val HexY = hexY with get
    member val HexZ = hexZ with get
    member val X = defaultArg x 0.0f with get
    member val Y = defaultArg y 0.0f with get
    member x.Neighbors = new HexCellNeighbors<HexCellCoord>(HexCellCoord.Nothing)


type HexGridMap( radius : int, cellSize : single) =

    let mutable cells = Array.empty<HexCellCoord>
    member x.Cells with get () = cells and set (v) = cells <- v

    member x.MakeCells() =
        let cells = 
            [   for x in -radius .. radius do
                    for z in -radius .. radius do
                        // use axial coordinate system to translate to xy space
                        let r = (single)z
                        let q = (single)x
                        let x2d = cellSize - 3.f/2.f * q
                        let y2d = cellSize * sqrt((*3.f*)1.f) * (r + q/2.f)
                        let cubeGridY = -x-z
                        if (abs(cubeGridY) <= radius ) then // keep overall shape of the grid hexagonal
                            let cell = new HexCellCoord(x,cubeGridY,z,x2d,y2d)
                            yield cell
            ] |> Seq.toArray //|> mapNeighbors
        
        // map out neighbor coordinates
        let assignCoords (c:HexCellCoord) =
            let getC d =
                let v = HexVector.GetNeighborCoordinates(c.HexX,c.HexY,c.HexZ,d)
                new HexCellCoord(v.[0],v.[1],v.[2])
            
            c.Neighbors.E   <- getC Direction.E
            c.Neighbors.NE  <- getC Direction.NE
            c.Neighbors.NW  <- getC Direction.NW
            c.Neighbors.SE  <- getC Direction.SE
            c.Neighbors.SW  <- getC Direction.SW
            c.Neighbors.W   <- getC Direction.W

        Seq.iter( fun (c : HexCellCoord) ->
            assignCoords c
        ) cells    
         
        x.Cells <- cells
        cells