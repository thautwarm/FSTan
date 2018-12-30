module rec FSTan.Data.Maybe

open FSTan.HKT
open FSTan.Monad
open FSTan.Show

type maybe<'a> = hkt<Maybe, 'a>
and Maybe() =
    inherit monad<Maybe>() with
        override __.bind<'a, 'b> (m: maybe<'a>) (k: 'a -> maybe<'b>) =
            let m = unwrap m
            match m with
            | Some a -> k a
            | None   -> wrap None

        override __.pure'<'a> (a: 'a) : maybe<'a> = wrap <| Some a
        static member wrap<'a> (x : Option<'a>): maybe<'a> =  {wrap = x} :> _
        static member unwrap<'a> (x : maybe<'a>): Option<'a> =  (x :?> _).wrap

        interface show<Maybe> with
            member __.show (a: maybe<'a>) =
                let a = Maybe.unwrap a
                a.ToString()

and OptionWrapper<'a> =
    {wrap : Option<'a>}
    interface maybe<'a>

let Just<'a> (a: 'a) : maybe<'a> = wrap <| Some a

[<GeneralizableValue>]
let Nothing<'a> : maybe<'a> = wrap <| None
let (|Just|Nothing|) (m: maybe<'a>) =
    match unwrap m with
    | Some m -> Just m
    | None    -> Nothing