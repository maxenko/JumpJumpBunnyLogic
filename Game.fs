namespace JumpJumpBunny

module Game =

    type Scene(averageDifficulty : IslandDifficulty, gridW : int, gridH : int, ?customSpawnPos : HexCellCoord) =
        let spawnPos = defaultArg customSpawnPos (new HexCellCoord(0,0,0))

    // set up grid
        let grid = new HexGrid(gridH,gridW,1.f,0.2f)

    // add player
        let player = new Proto()

    // start enemy spawner
        let enemies = new Enemies(10)

    // start neutral spawner

    // start powerupSpawner

        static let runUpdate l = Seq.iter (fun (i:IUpdate) -> i.Update()) l

        interface IUpdate with
            member x.Update() =
                player.Update()
                enemies.Update()