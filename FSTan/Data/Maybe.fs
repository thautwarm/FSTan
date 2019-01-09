module rec FSTan.Data.Maybe

open FSTan.HKT
open FSTan.Monad
open FSTan.Show

type mkMaybe<'M>() =
    inherit monad<mkMaybe<'M>>()
        override __.bind<'a, 'b> (m: hkt<mkMaybe<'M>, 'a>) (k: 'a -> hkt<mkMaybe<'M>, 'b>) =
            match unwrap m with
            | Some a -> k a
            | None -> Nothing


        override __.pure'<'a> (a: 'a) : hkt<mkMaybe<'M>, 'a> = wrap <| Some a
        static member wrap<'a> (x : Option<'a>): hkt<mkMaybe<'M>, 'a> =  {wrap = x} :> _
        static member unwrap<'a> (x : hkt<mkMaybe<'M>, 'a>): Option<'a> =  (x :?> _).wrap
        interface show<mkMaybe<'M>> with
            member __.show (a: hkt<mkMaybe<'M>, 'a>) =
                let a = mkMaybe<'M>.unwrap a
                a.ToString()

and OptionWrapper<'M, 'a> =
    {wrap : Option<'a>}
    interface hkt<mkMaybe<'M>, 'a>

let Just<'M, 'a> (a: 'a) : hkt<mkMaybe<'M>, 'a> = wrap <| Some a

[<GeneralizableValue>]
let Nothing<'M, 'a> : hkt<mkMaybe<'M>, 'a> = wrap <| None

let (|Just|Nothing|) (m: hkt<mkMaybe<'M>, 'a>) =
    let s: 'a Option = unwrap m
    match s with
    | Some m -> Just m
    | None    -> Nothing

