namespace JumpJumpBunny

open System

type Direction =
    | NE    = 0
    | NW    = 2
    | E     = 4
    | W     = 8
    | SE    = 16
    | SW    = 32

type HexVector =
    static member E     = (1,0,-1)
    static member NE    = (0,1,-1)
    static member NW    = (-1,1,0)
    static member W     = (-1,0,1)
    static member SW    = (0,-1,1)
    static member SE    = (1,-1,0)
    static member GetVector (d:Direction) =
        match d with
        | NE when d = Direction.NE -> HexVector.NE
        | NW when d = Direction.NW -> HexVector.NW
        | E when d  = Direction.E -> HexVector.E
        | W when d  = Direction.W -> HexVector.W
        | SE when d = Direction.SE -> HexVector.SE
        | SW when d = Direction.SW -> HexVector.SW
        | _ -> HexVector.E 

type HexCellNeighbors<'n>(cell:'n) =
    let parent = cell
    let findNeighbor (d:Direction) =
        0
    member x.NE with get() = findNeighbor(Direction.NE)
    member x.NW with get() = findNeighbor(Direction.NW)
    member x.E  with get() = findNeighbor(Direction.E)
    member x.W  with get() = findNeighbor(Direction.W)
    member x.SE with get() = findNeighbor(Direction.SE)
    member x.SW with get() = findNeighbor(Direction.SW)
      
type HexCellCoord(hexX : int, hexY : int, hexZ : int, ?x : single, ?y : single) as self =
    // add to collection
    member val HexX = hexX with get
    member val HexY = hexY with get
    member val HexZ = hexZ with get
    member val X = defaultArg x 0.0f with get
    member val Y = defaultArg y 0.0f with get
    member x.Neighbors = new HexCellNeighbors<HexCellCoord>(self)


type HexGridMap(rows : int, cols : int, cellSize : single) =
    let f (i:int) = (single)i 
    let cell x y = (x,y,-(x+y))
    let coordInDirection (x,y,z) steps (d:Direction) =
        let v = HexVector.GetVector(d)
        let multiplied = Math.multVector v steps
        Math.addVectors (x,y,z) multiplied

    let width   = Math.evenize cols
    let height  = Math.evenize rows

    let get2AxisX r q   = cellSize * sqrt(3.f) * (f q + 0.5f * f(r&&&1))
    let get2AxisY r     = cellSize * (3.f/2.f) * (f r)
    let makeCell r q = new HexCellCoord( r, q, (r+q) * -1, get2AxisX r q, get2AxisY r )    

    member x.MakeCells() = 
        let leftBoundary    = -width/2
        let rightBoundary   = width/2
        let topBoundary     = height/2
        let bottomBoundary  = -height/2

        let ret = 
            [ 
                for q in leftBoundary .. rightBoundary do 
                    for r in bottomBoundary .. topBoundary do
                        yield makeCell r q 
            ]
        Seq.toArray(ret)

    new(rows,cols) =
        HexGridMap(rows,cols,1.f)


//let verticalOffset = cellSize*3./4.
//let cellWidth = sqrt(3.)/2. * cellSize