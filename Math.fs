namespace JumpJumpBunny

module Math =

    open System

    let rnd = new Random(DateTime.Now.Millisecond)

    let isOdd x = (x &&& 1) = 1
    let evenize x = if isOdd x then x + 1 else x

    let multVector (x1,y1,z1) x = (x1*x,y1*x,z1*x)
    let addVectors (x1,y1,z1) (x2,y2,z2) = (x1+x2,y1+y2,z1+z2)

    let rec repeatElement n e =
        match n with
        | n when n < 1 -> []
        | 1 -> [e]
        | _ -> [e] @ repeatElement (n-1) e

    let rndBetween min max = rnd.Next(min,max)

    let rndBetweenF min max =
        let seed = single <| Math.Abs(rnd.NextDouble() * 2.0 - 1.0)
        let space = max - min
        single <| seed * space + min 
        
    let positiveOrZero x = if x >= 0 then x else 0

    let probabilityRange f s peak spread = // creates a range of repeated numbers based on their spread distance from peak
        let s = [|f .. s|]
        let repeatTimes n = 
            let dist = abs(n - peak)
            if dist > spread then 0
            else spread - dist
        s
        |> Array.map (fun n -> repeatTimes n)
        |> Array.mapi (fun i n -> repeatElement n s.[i])
        |> Array.toList
        |> List.concat

    let getRandomFromRange f s peak spread =
        let s = probabilityRange f s peak spread
        let size = if s.Length > 0 then s.Length-1 else 0
        s.[rnd.Next(0,size)]

    let randEl s =
        let idx = rnd.Next(0,Seq.length s)
        Seq.head ( Seq.skip idx s )

    let updateRateMS nthOfSecond = 1000.f/nthOfSecond

    let decide (p:Probability) =
        let max = 5 - (int <| p)
        rnd.Next(0,max) = rnd.Next(0,max)