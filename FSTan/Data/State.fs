module FSTan.Data.State
open FSTan.HKT
open FSTan.Monad

type state<'s, 'a> = hkt<stateCons<'s>, 'a>
and stateCons<'s>() =
    inherit monad<stateCons<'s>>() with
        static member run<'a> (m: state<'s, 'a>) =
            let (State f): stateData<'s, 'a> = m :?> _
            f

        override __.bind<'a, 'b> (m: state<'s, 'a>) (k: 'a -> state<'s, 'b>) =
            let m = State <| fun s ->
                let a, s = stateCons.run m s
                stateCons.run (k a) s
            m :> _  

        override __.pure'<'a> (a: 'a) : state<'s, 'a> = 
            let m =  State <| fun s -> (a, s)
            m :> _

and stateData<'s, 'a> = 
    | State of ('s -> ('a * 's))
    interface state<'s, 'a>

[<GeneralizableValue>]
let runState<'s, 'a> = stateCons<'s>.run<'a>

[<GeneralizableValue>]
let get<'s> : state<'s, 's> =  State(fun s -> (s, s)) :> _

let put<'s, 'a> (a: 'a) : state<'s, 'a> = State(fun s -> (a, s)) :> _