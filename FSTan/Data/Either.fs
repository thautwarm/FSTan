module FSTan.Data.Either
open FSTan.HKT
open FSTan.Show
open FSTan.Monad

type either<'e, 'a> = hkt<EitherSig<'e>, 'a>
and EitherSig<'e>() =
    inherit monad<EitherSig<'e>>() with
        override __.pure'<'a> (a: 'a): either<'e, 'a> = Right a :> _
        override __.bind<'a, 'b> (m: either<'e, 'a>) (k: 'a -> either<'e, 'b>): either<'e, 'b> =
            match m :?> eitherData<'e, 'a> with
            |  Left l  -> Left l :> _
            |  Right r -> k r
        interface show<EitherSig<'e>> with
            member __.show<'a> (a: either<'e, 'a>) =
                let a = a :?> eitherData<_, _>
                a.ToString()

and eitherData<'e, 'a> =
    | Left of 'e
    | Right of 'a
    interface either<'e, 'a>

let Left<'e, 'a> (e: 'e) : either<'e, 'a> = Left e :> _
let Right<'e, 'a> (a: 'a) : either<'e, 'a> = Right a :> _
let (|Left|Right|) (m: either<'e, 'a>) =
    match m :?> eitherData<'e, 'a> with
    | Left l  -> Left l
    | Right r -> Right r



