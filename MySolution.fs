open System
open System.IO

[<Literal>]
let Wall = 35uy
[<Literal>]
let Passage = 45uy
[<Literal>]
let Exit = 88uy
[<Literal>]
let Theseus = 64uy
[<Literal>]
let Minotaur = 42uy
[<Literal>]
let Prisoner = 36uy

[<Literal>]
let Width = 22
[<Literal>]
let Height = 12

module MySolution =
    
    let fileName = "lame_labyrinth.data"

    type Labyrinth = byte array

    let translateFromPosition (x, y) = 
        x + (y * Width)

    let translateToPosition i =
        let x = i - ((i / Width) * Width)
        let y = (i / Width)
        (x, y)

    let allBytes =
        File.ReadAllBytes fileName
        |> Array.filter(fun b -> b <> 10uy && b <> 13uy)

    type GamePhase =
    | Started
    | PrisonerFound
    | MinotaurKilled
    | TheseusKilled
    | ExitFound

    type Character = {
        Position: int * int
        Symbol: char
        Color: ConsoleColor
        Draw: bool
        Move: bool
    }

    type Game = {
        Labyrinth: Labyrinth
        Theseus: Character
        Minotaur: Character
        Prisoner: Character
        ExitPos: int * int 
        Phase: GamePhase
    }

    let drawCharacter (c:Character) =
        if c.Draw
        then 
            let (x,y) = c.Position
            Console.ForegroundColor <- c.Color
            Console.SetCursorPosition(x, y)
            Console.Write(c.Symbol)
            Console.SetCursorPosition(0, Height + 2)
            Console.ResetColor()

    let draw state =
        for x in [0 .. Width - 1 ] do
            for y in [0 .. Height - 1] do
                Console.SetCursorPosition(x, y)
                Console.Write (state.Labyrinth.[translateFromPosition(x, y)] |> char)
        drawCharacter state.Theseus
        drawCharacter state.Minotaur
        drawCharacter state.Prisoner

    let startGame fileName =
        let allBytes =
            File.ReadAllBytes fileName
            |> Array.filter(fun b -> b <> 10uy && b <> 13uy)
        let withoutCharacters =
            allBytes
            |> Array.map(fun b ->
                if b <> Wall && b <> Passage && b <> Exit then Passage else b)
        {
            Game.Labyrinth = withoutCharacters
            Game.Theseus =
                let index = Array.FindIndex(allBytes, fun b -> b = Theseus)
                if index < 0
                then failwith (sprintf "Labyrinth does not have Theseus ('%x')" Theseus)

                {   Position = index |> translateToPosition
                    Symbol = Theseus |> char
                    Draw = true
                    Move = true
                    Color = ConsoleColor.Cyan }
            Game.Minotaur =
                let index = Array.FindIndex(allBytes, fun b -> b = Minotaur)
                if index < 0
                then failwith (sprintf "Labyrinth data does not have Minotaur ('%x')" Minotaur)

                {   Position = index |> translateToPosition 
                    Symbol = Theseus |> char
                    Draw = true
                    Move = true
                    Color = ConsoleColor.Magenta }
            Game.ExitPos =
                let index = Array.FindIndex(allBytes, fun b -> b = Exit)
                if index < 0
                then failwith (sprintf "Labyrinth data does not have Exit ('%x')" Exit)

                index |> translateToPosition
            Game.Prisoner =
                let index = Array.FindIndex(allBytes, fun b -> b = byte(Prisoner))
                if index < 0
                then failwith (sprintf "Labyrinth data does not have Prisoner ('%x')" Prisoner)

                {   Position = index |> translateToPosition 
                    Symbol = Prisoner |> char
                    Draw = true
                    Move = true
                    Color = ConsoleColor.Red }
            Game.Phase = Started
        }

    let moveTheseus state =
        state

    let runGame () =
        let rec loop state =
            let newState =
                state
                |> moveTheseus
            draw newState
            loop newState
        let initialState = startGame @"lame_labyrinth.data"
        draw initialState
        loop initialState

    [<EntryPoint>]
    let main argv =
        printfn "%A" argv

        runGame()

        0 // return an integer exit code