namespace JumpJumpBunny

open System
open System.Collections.Generic

type EntityState =
| Normal = 0
| Invulnerable = 1
| Immobile = 2

type Powerups =
| None          = 0
| DoublePoints  = 1
| Immunity      = 2
| HalfPoints    = 4
| SpawnIslandsAround = 8
| SpawnIslandsAroundAndExtendCurrent = 16
| EatOneEnemy   = 32

type IslandDifficulty =
| Effortless    = 0
| VeryEasy      = 1
| Easy          = 2
| Normal        = 3
| Medium        = 4
| Moderate      = 5
| Challenging   = 6
| Difficult     = 7
| Hard          = 8
| VeryHard      = 9
| Insane        = 10
| Nightmare     = 11
| Neo           = 12

type IslandRespawnDelay =
| Instant   = 0
| Quick     = 1
| Fast      = 2
| Brief     = 3
| Normal    = 4
| Delayed   = 5
| Slowed    = 6
| Prolonged = 7
| Posponed  = 8
| Awhile    = 9
| Long      = 10

type IUpdate =
    abstract Update : unit -> unit