namespace JumpJumpBunny

open System
open System.Collections.Generic

type JumpEventArgs(fromCell : HexCellCoord, toCell : HexCellCoord) =
    member val From = fromCell with get
    member val To = toCell with get

type IslandSizeChangeArgs<'i>( island:'i, previousSize : single, newSize : single ) =
    member val Island = island with get
    member val PreviousSize = previousSize with get
    member val NewSize = newSize with get

type Proto() as self =
    let death       = new Event<Proto>()
    let reSpawn     = new Event<Proto>()
    let jump        = new Event<JumpEventArgs>()
    let collect     = new Event<Powerups>()
    let stageFinish = new Event<Proto>()
    let actionDenied= new Event<JumpJumpBunny.Action>()
    let mutable previousLoc = new HexCellCoord(0,0,0,0.f,0.f)
    let mutable currentLoc  = new HexCellCoord(0,0,0,0.f,0.f)

    member val Lives    = 1 with get, set
    member val Points   = 0 with get, set
    member val Powerups = Powerups.None with get, set

    member x.PreviousLoc  = previousLoc
    member x.CurrentLoc   = stageFinish
    
    [<CLIEvent>]
    member x.Death = death.Publish
    
    [<CLIEvent>]
    member x.ReSpawn = reSpawn.Publish

    [<CLIEvent>]
    member x.Jump = jump.Publish

    [<CLIEvent>]
    member x.Collect = collect.Publish    
    
    [<CLIEvent>]
    member x.StageFinish = stageFinish.Publish

    member x.AddPoints(p)   = self.Points <- self.Points + p
    member x.AddLives(l)    = self.Lives <- self.Lives + l
    member x.OnCollect()    = collect.Trigger Powerups.None
    member x.OnJump(fromLoc,toLoc) = 
        previousLoc <- fromLoc
        currentLoc <- currentLoc
        jump.Trigger( new JumpEventArgs( previousLoc, currentLoc ) )

    member x.OnDenied() = actionDenied.Trigger Action.Jump

    member x.JumpTo(d:Direction) =
        ()

    interface IUpdate with member x.Update() = ()

    member x.Update() = (self :> IUpdate).Update()

type Nobody() =
    inherit Proto()

type IslandVacatedArgs( entity:Proto, dir:Direction ) =
    member val Direction = dir with get
    member val Entity  = entity with get

type IslandOccupiedArgs( entity:Proto, from:HexCellCoord ) =
    member val Direction = from with get
    member val Entity  = entity with get

type Enemy(duration) as self = 
    inherit Proto()
    static let offGridLocation = new HexCellCoord(-1000000,-1000000,-1000000)
    let mutable location = offGridLocation
    let rate = Math.updateRateMS 12.f
    let mutable d = duration
    let mutable lifeLeft = duration
    let mutable deSpawned = false
    let edgeReached = new Event<Enemy>()
    let deSpawn     = new Event<Proto>()

    member x.Duration with get() = d and set(value) = d <- value
    member x.EdgeReached = edgeReached.Publish
    member x.Despawn = deSpawn.Publish
    member x.IsDespawned() = deSpawned
    member x.Spawn(loc:HexCellCoord) =  location <- loc
    member x.MoveOffGrid() = location <- offGridLocation

    interface IUpdate with
        member x.Update() =
            if not deSpawned then
                // if at end of its life... despawn it
                lifeLeft <- lifeLeft - (int)rate
                if lifeLeft <= 0 then
                    deSpawn.Trigger(self)
                    deSpawned <- true

    member x.Update() = (self :> IUpdate).Update()
    new() = Enemy(20000)

type Neutral(d:Direction) = 
    inherit Enemy()
    member val Direction = d with get, set

type Island(h:HexCellCoord, initialSize : single, minSize : single, maxSize : single, initialDifficulty : IslandDifficulty, respawnDelay: single) as self =

    static let rand = new System.Random(DateTime.Now.Millisecond)
    static let nobody = new Nobody() :> Proto

    let mutable size = initialSize
    let mutable twoSecondToDespawnEventTriggered = false
    let mutable despawnTriggered = false
    let mutable active = true
    let mutable difficulty = initialDifficulty
    let mutable lifeSpan = 0.f
    let mutable respawnDelay = 0.f
    let mutable occupier = nobody
    
    let randIslandRespawnDelay() =
        let enumVals = typeof<IslandRespawnDelay> |> Enum.GetValues
        let upper = enumVals.GetUpperBound(0)
        let lower = enumVals.GetLowerBound(0)
        let peak = int upper/2
        let result : IslandRespawnDelay = enum ( Math.getRandomFromRange lower upper peak 4 )
        result

    let randIslandDifficulty() =
        let enumVals = typeof<IslandDifficulty> |> Enum.GetValues
        let upper = enumVals.GetUpperBound(0)
        let lower = enumVals.GetLowerBound(0)
        let peak = (int initialDifficulty)/2
        let result : IslandDifficulty = enum ( Math.getRandomFromRange lower upper peak 4 )
        result

    let respawnDelayToMS(d:IslandRespawnDelay) = 1000.f * (single d)

    let resetSpawnDelay() =
        let rnd = randIslandRespawnDelay
        respawnDelay <- (randIslandRespawnDelay() |> respawnDelayToMS)
        
 
    let respawn = new Event<Island>()
    let despawn = new Event<Island>()
    let twoSecondsToDespawn = new Event<Island>()
    let sizeChange = new Event<IslandSizeChangeArgs<Island>>()
    let vacated = new Event<IslandVacatedArgs>()
    let occupied = new Event<IslandOccupiedArgs>()

    let reset() =
        size <- initialSize // needs to randomize
        active <- true 
        despawnTriggered <- false
        twoSecondToDespawnEventTriggered <- false
        difficulty <- randIslandDifficulty()

    let rec difficultyPreset d = // (life length, starting size)
        match d with 
        | IslandDifficulty.Effortless    -> (30000.f, maxSize*1.f)
        | IslandDifficulty.VeryEasy      -> (20000.f, maxSize*1.f)
        | IslandDifficulty.Easy          -> (10000.f, maxSize*1.f)
        | IslandDifficulty.Normal        -> (7000.f, maxSize*0.75f)
        | IslandDifficulty.Medium        -> (5500.f, maxSize*0.65f)
        | IslandDifficulty.Moderate      -> (3500.f, maxSize*0.6f)
        | IslandDifficulty.Challenging   -> (2700.f, maxSize*0.55f)
        | IslandDifficulty.Difficult     -> (2000.f, maxSize*0.5f)
        | IslandDifficulty.Hard          -> (1700.f, maxSize*0.5f)
        | IslandDifficulty.VeryHard      -> (1500.f, maxSize*0.4f)
        | IslandDifficulty.Insane        -> (1300.f, maxSize*0.4f)
        | IslandDifficulty.Nightmare     -> (1000.f, maxSize*0.35f)
        | IslandDifficulty.Neo           -> (500.f, maxSize*0.3f)
        | _ -> difficultyPreset IslandDifficulty.Medium

    let everyNthOfSecond = 12.f
    let updateIntervalMS = 1000.f/everyNthOfSecond
    
    let mutable mutableCycleStartedAt = DateTime.Now
    let mutable lastUpdate = DateTime.Now 
    let isTimeToUpdate() = Common.isTimeToUpdate &lastUpdate updateIntervalMS
         
    let getDecayFor d =
        let (d,s) = difficultyPreset d
        lifeSpan <- d
        let numberOfUpdatesTillCollapse = d/(single)updateIntervalMS;
        ((single)s)/numberOfUpdatesTillCollapse

    member val InitialSize = initialSize with get
    member x.DecayRate with get() = getDecayFor difficulty

    [<CLIEvent>]
    member x.Respawn = respawn.Publish

    [<CLIEvent>]
    member x.Despawn = despawn.Publish    
    
    [<CLIEvent>]
    member x.SizeChange = sizeChange.Publish

    [<CLIEvent>]
    member x.Vacated = vacated.Publish

    [<CLIEvent>]
    member x.Occupied = occupied.Publish

    [<CLIEvent>]
    member x.TwoSecondsToDespawn = twoSecondsToDespawn.Publish

    member x.FakeEventCall() = respawn.Trigger(self)

    member val Coord = h with get

    member x.OnSizeChange(previous,next) =
        sizeChange.Trigger( new IslandSizeChangeArgs<Island>(self,previous,next) )

    member x.OnTwoSecondsToDespawn() =
        if not twoSecondToDespawnEventTriggered then
            twoSecondToDespawnEventTriggered <- true
            twoSecondsToDespawn.Trigger(self)

    member x.Occupy( entity:Proto ) =
        occupier <- entity
        occupied.Trigger(  new IslandOccupiedArgs(occupier,occupier.PreviousLoc)  )
        

    member x.OnVacated(d:Direction) =
        vacated.Trigger( new IslandVacatedArgs(occupier,d) )
        occupier <- nobody

    member x.OnRespawn() =
        reset()
        respawn.Trigger(self)

    member x.OnDespawn() =
        despawnTriggered <- true
        resetSpawnDelay()
        despawn.Trigger(self)

    member x.Start() = active <- true
    member x.Stop() = active <- false 

    member x.Update() =
        if active && isTimeToUpdate() then
                let decay = self.DecayRate
                let nextSize = size - decay // linear rate for now, might have to change or add options to various dynamics later
                let previousSize = size
            
                // if at minSize already, trigger despawn
                if size <= minSize && not despawnTriggered then
                    self.OnDespawn()
                elif despawnTriggered then
                    respawnDelay <- respawnDelay - updateIntervalMS // count to next spawn
                    if respawnDelay <= 0.f then
                        self.OnRespawn()
                    else
                        ()
                else        
                    // trigger two second to despawn event
                    let amountLeftToMinSize = (size - minSize)*lifeSpan
                    let twoSecondSpan = 2.0f*everyNthOfSecond*updateIntervalMS
                    if twoSecondSpan >= amountLeftToMinSize && not twoSecondToDespawnEventTriggered then
                        self.OnTwoSecondsToDespawn()
                   
                    size <- if nextSize <= minSize then minSize else nextSize
                    self.OnSizeChange(previousSize,nextSize) // trigger size change
            else ()

    interface IUpdate with member x.Update() = (self :> IUpdate).Update()

    member x.ToDebugString() =
        sprintf "Starting Size: %f\nCurrent: %f\nDecay: %f\nTwo Sec to Despan: %b\nDespanwed: %b\nRespawn delay: %f\nDifficulty: %A" 
            initialSize size self.DecayRate twoSecondToDespawnEventTriggered despawnTriggered respawnDelay difficulty