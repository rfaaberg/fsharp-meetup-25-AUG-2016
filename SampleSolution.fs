//open System
//open System.IO
//
//[<Literal>]
//let Wall = 35uy
//[<Literal>]
//let Passage = 45uy
//[<Literal>]
//let Exit = 88uy
//[<Literal>]
//let Theseus = 64uy
//[<Literal>]
//let Minotaur = 42uy
//[<Literal>]
//let Prisoner = 36uy
//[<Literal>]
//let Width = 22
//[<Literal>]
//let Height = 12
//
//type Labyrinth = byte array
//
//type GamePhase = 
//    | Started
//    | PrisonerFound
//    | MinotaurKilled
//    | TheseusKilled
//    | ExitFound
//
//type Character = 
//    {
//        Position: int * int
//        Symbol: char 
//        Color: ConsoleColor
//        Draw: bool
//        Move: bool
//    }
//
//type Game = 
//    {
//        Labyrinth: Labyrinth
//        Theseus: Character
//        Minotaur: Character
//        Prisoner: Character
//        ExitPos: int * int
//        Phase: GamePhase
//    }
//
//let translateFromPosition (x,y) = 
//    x + ( y * Width )
//
//let translateToPosition i =
//    let x = i - ((i/Width) * Width) 
//    let y = (i/Width)
//    (x,y)
//
//let startGame fileName = 
//    let allBytes = 
//        File.ReadAllBytes fileName
//        |> Array.filter(fun b -> b <> 10uy && b <> 13uy)
//    let withoutCharacters = 
//        allBytes
//        |> Array.map(fun b -> 
//            if b <> Wall && b <> Passage && b <> Exit then Passage else b)
//    {
//        Game.Labyrinth = withoutCharacters
//        Game.Theseus =
//            let index = Array.FindIndex(allBytes, fun b -> b = Theseus)
//            if index < 0 
//            then failwith (sprintf "Labyrinth data does not have Theseus ('%x')" Theseus)
//            
//            {   Position = index |> translateToPosition
//                Symbol = Theseus |> char
//                Draw = true
//                Move = true
//                Color = ConsoleColor.Cyan }
//        Game.Minotaur = 
//            let index = Array.FindIndex(allBytes, fun b -> b = Minotaur)
//            if index < 0
//            then failwith (sprintf "Labyrinth data does not have Minotaur ('%x')" Minotaur)
//
//            {   Position = index |> translateToPosition
//                Symbol = Minotaur |> char
//                Draw = true
//                Move = true
//                Color = ConsoleColor.Yellow }
//        Game.ExitPos = 
//            let index = Array.FindIndex(allBytes, fun b -> b = Exit)
//            if index < 0 
//            then failwith (sprintf "Labyrinth data does not have Exit ('%x')" Exit)
//
//            index |> translateToPosition
//        Game.Prisoner = 
//            let index = Array.FindIndex(allBytes, fun b -> b = byte(Prisoner))
//            if index < 0 
//            then failwith (sprintf "Labyrinth data does not have Prisoner ('%x')" Prisoner)
//
//            {   Position = index |> translateToPosition
//                Symbol = Prisoner |> char
//                Draw = true
//                Move = true
//                Color = ConsoleColor.Red }
//        Game.Phase = Started
//    }
//
//let drawCharacter (c:Character) =
//    if c.Draw 
//    then 
//        let (x,y) = c.Position
//        Console.ForegroundColor <- c.Color
//        Console.SetCursorPosition(x,y)
//        Console.Write(c.Symbol)
//        Console.SetCursorPosition(0, Height + 2) 
//        Console.ResetColor()
//
//let draw state = 
//    for x in [0 .. Width - 1 ] do 
//        for y in [0 .. Height - 1] do 
//            Console.SetCursorPosition(x,y)
//            Console.Write (state.Labyrinth.[translateFromPosition(x,y)] |> char) 
//    drawCharacter state.Theseus
//    drawCharacter state.Minotaur
//    drawCharacter state.Prisoner
//
//let verifyMove state proposed =
//    let (x,y) = proposed 
//
//    if x < 0 || x > ( Width - 1) || y < 0 || y > ( Height - 1 ) 
//    then state 
//    else
//    
//    match state.Labyrinth.[translateFromPosition(x,y)] with 
//    | b when b = Wall -> state
//    | _ -> { state with Theseus = { state.Theseus with Position = proposed } }
//
//
//
//let moveTheseus (state:Game) = 
//    if state.Theseus.Move = false 
//    then state
//    else
//
//    let (x,y) = state.Theseus.Position
//        
//    match Console.ReadKey(true).Key with 
//    | ConsoleKey.UpArrow ->
//        (x, y - 1) |> verifyMove state 
//    | ConsoleKey.DownArrow -> 
//        (x, y + 1) |> verifyMove state 
//    | ConsoleKey.RightArrow -> 
//        (x + 1, y) |> verifyMove state 
//    | ConsoleKey.LeftArrow ->      
//        (x - 1, y) |> verifyMove state 
//    | _ -> state
//
//type PossibleMove =
//    {
//        Position: int * int
//        Element: byte
//    }
//
//let formulatePossibleMove (state:Game) (x,y) = 
//    { Position = (x,y); Element = state.Labyrinth.[translateFromPosition(x,y)] }    
//
//let moveMinotaur (state:Game) = 
//    if state.Minotaur.Move = false 
//    then state
//    else
//
//    let (x,y) = state.Minotaur.Position
//    let moveOptions = 
//        [   
//            formulatePossibleMove state (x+1,y)
//            formulatePossibleMove state (x-1,y)
//            formulatePossibleMove state (x,y+1)
//            formulatePossibleMove state (x,y-1)                
//        ]
//        |> List.filter(fun m -> m.Element <> Wall)
//    // The Minotaur decides what to do based on the phase of the game
//
////    let rec evaluate moveOptions = 
////        match state.Phase with 
////        | Started -> 
////            match moveOptions |> List.tryFind(fun f -> f.Position = state.Theseus) with 
////            | Some(o) -> { state with Minotaur = o.Position }
////            | None -> evaluate (moveOptions |> List.filter()
////        | _ -> 
////            // pick one at random
////            let rnd = new System.Random()
////            { state with Minotaur = moveOptions.[rnd.Next() % moveOptions.Length].Position }
////    evaluate()            
//
//    let rnd = new System.Random()
//    { state with 
//        Minotaur = { state.Minotaur with 
//                        Position = moveOptions.[rnd.Next() % moveOptions.Length].Position } }       
// 
//let evaluateGame (g:Game) = 
//    match g.Phase with 
//    | MinotaurKilled -> 
//        if g.Theseus.Position = g.ExitPos then 
//            { g with Phase = ExitFound } 
//        else g
//    | Started -> 
//        if g.Theseus.Position = g.Minotaur.Position then 
//            { g with Phase = TheseusKilled; Theseus = { g.Theseus with Move = false }} 
//        elif g.Theseus.Position = g.Prisoner.Position then 
//            { g with Phase = PrisonerFound; Prisoner = { g.Prisoner with Draw = false; Move = false }}
//        else g
//    | PrisonerFound -> 
//        if g.Theseus.Position = g.Minotaur.Position then 
//            { g with Phase = MinotaurKilled; Minotaur = { g.Minotaur with Move = false } }
//        else g
//    | _ -> g
//
//let runGame () = 
//    let rec loop state = 
//        let newState = 
//            state 
//            |> moveTheseus
//            |> moveMinotaur
//        draw newState
//        match evaluateGame newState with 
//        | g when g.Phase = ExitFound ->
//            printfn "Theseus has escaped alive!           "
//        | g when g.Phase = TheseusKilled -> 
//            printfn "Theseus was killed!"
//        | g when g.Phase = PrisonerFound ->
//            printfn "Hooray!  The prisoner has been found!"
//            loop g
//        | g when g.Phase = MinotaurKilled -> 
//            printfn "Theseus killed the Minotaur!         "
//            loop g
//        | _ -> loop newState
//    let initialState = startGame @"..\..\lame_labyrinth.data"
//    draw initialState
//    loop initialState
//
//[<EntryPoint>]
//let main argv = 
//    printfn "%A" argv
//
//    runGame()    
//
//    0 // return an integer exit code
(*
some comment
*)