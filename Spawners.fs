namespace JumpJumpBunny

open System

type Enemies(?limit : int) as self =
    let entities = Math.repeatElement (defaultArg limit 5) (new Enemy( Math.rndBetween 5000 10000 ))
    let updateRate = Math.updateRateMS 60.f
    let mutable lastUpdate = DateTime.Now

    member x.GetAll() = entities

    interface IUpdate with
        member x.Update() =
            let isTime = Common.isTimeToUpdate &lastUpdate updateRate // time to update?
            if isTime then
                Seq.iter (fun (e:Enemy) -> e.Update()) entities

    member x.Update() = (self :> IUpdate).Update()