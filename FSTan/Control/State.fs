module FSTan.Control.State
open FSTan.HKT
open FSTan.Monad

type state<'s, 'a> = hkt<StateMonad<'s>, 'a>
and StateMonad<'s>() =
    inherit monad<StateMonad<'s>>() with
        static member run<'a> (m: state<'s, 'a>) =
            let (State f): stateData<'s, 'a> = m :?> _
            f

        override __.bind<'a, 'b> (m: state<'s, 'a>) (k: 'a -> state<'s, 'b>) =
            let m = State <| fun s ->
                let a, s = StateMonad.run m s
                StateMonad.run (k a) s
            m :> _  

        override __.pure'<'a> (a: 'a) : state<'s, 'a> = 
            let m =  State <| fun s -> (a, s)
            m :> _

and stateData<'s, 'a> = 
    | State of ('s -> ('a * 's))
    interface state<'s, 'a>

[<GeneralizableValue>]
let runState<'s, 'a> : state<'s, 'a> -> 's -> ('a * 's) = StateMonad<'s>.run<'a>

[<GeneralizableValue>]
let get<'s> : state<'s, 's> =  State(fun s -> (s, s)) :> _

let put<'s, 'a> (a: 'a) : state<'s, 'a> = State(fun s -> (a, s)) :> _

let State<'s, 'a> (f: 's -> ('a * 's)): state<'s, 'a> = upcast (State f)
let (|State|) (s: state<'s, 'a>) =
    let (State f) = s :?> stateData<'s, 'a>
    in State f