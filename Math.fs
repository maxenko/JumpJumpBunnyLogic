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
        | 1 -> [e]
        | _ -> [e] @ repeatElement (n-1) e

    let rndBetween min max = rnd.Next(min,max)

    let rndBetweenF min max  =
        let seed = single <| Math.Abs(rnd.NextDouble() * 2.0 - 1.0)
        let space = max - min
        single <| seed * space + min 
        

    let probabilityRange f s peak spread = // this may need further testing, but should work for now
        let len = abs(s - f)
        let mask1 = Array.init len ( fun _ -> 1 )   
        let s = [|f .. s|]
        let leftmost    = peak - spread
        let rightmost   = peak + spread

        let addWeight i = // calculates additional weight for the number depending on its distance from peak number
            if s.[i] < rightmost && s.[i] > leftmost then // if within spread of a the peak number
                let diff = abs ( peak - abs(s.[i]) )
                peak - diff    
            else 0

        mask1 
        |> Seq.mapi (fun i x -> x + addWeight i )       // repeat mask
        |> Seq.mapi (fun i x -> repeatElement x s.[i] ) // sequences of repeated numbers
        |> Seq.concat
        |> Seq.toArray

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