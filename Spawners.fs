namespace JumpJumpBunny

open System

type Enemies(grid:HexGrid, ?limit : int) as self =
    let entities = Math.repeatElement (defaultArg limit 5) (new Enemy( Math.rndBetween 5000 10000 ))
    let updateRate = Math.updateRateMS 60.f
    let mutable lastUpdate = DateTime.Now

    member x.GetAll() = entities

    member x.Update() =
        // do we need to spawn?
        let spawnedCount = Seq.map (fun (e:Enemy) -> if e.IsDespawned() then 0 else 1) entities |> Seq.sum
        match spawnedCount with
        | x when x >= entities.Length -> () // maximum enemies active
        | _ ->
            let yes = Math.decide Probability.Sometimes
            let predicate (e:Enemy) = e.IsDespawned()
            if yes then
                let firstUnspawned = Seq.find predicate entities
                firstUnspawned.Spawn( grid.GetRandIsland() )
            
        let isTime = Common.isTimeToUpdate &lastUpdate updateRate // time to update?
        if isTime then
            Seq.iter (fun (e:Enemy) -> e.Update()) entities

    interface IUpdate with member x.Update() = self.Update()

type Neutrals(grid:HexGrid, ?limit : int ) as self =
    inherit Enemies(grid, defaultArg limit 3)

    member x.Update() = ()
    interface IUpdate with member x.Update() = self.Update()


type Consumables(f:PowerupFrequency) as self =
    
    member x.Update() = ()
    interface IUpdate with member x.Update() = self.Update()