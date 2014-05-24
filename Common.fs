module Common
    
    open System

    let isTimeToUpdate (lastUpdate : DateTime byref) (updateRate : single) = 
        let is = (DateTime.Now - lastUpdate).Milliseconds > (int)updateRate
        if is then 
            lastUpdate <- DateTime.Now
            true
        else false 