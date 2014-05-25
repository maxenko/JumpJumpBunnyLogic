namespace JumpJumpBunny

module Game =

    type Scene(averageDifficulty : IslandDifficulty, gridW : int, gridH : int, ?customSpawnPos : HexCellCoord) =
        let spawnPos = defaultArg customSpawnPos (new HexCellCoord(0,0,0))

        // set up grid
        let grid = new HexGrid(gridH,gridW,1.f,0.2f)

        // add player
        let player = new Proto()

        // start enemy spawner
        let enemies = new Enemies(grid,10)

        // start neutral spawner
        let neutrals = new Neutrals(grid,2)

        // start powerupSpawner
        let powerups = new Consumables(PowerupFrequency.Regular)

        static let runUpdate l = Seq.iter (fun (i:IUpdate) -> i.Update()) l

        interface IUpdate with
            member x.Update() =
                player.Update()
                enemies.Update()
                neutrals.Update()
                powerups.Update()
