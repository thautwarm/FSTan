module FSTan.Data.Either
open FSTan.HKT
open FSTan.Show
open FSTan.Monad

type mkEither<'E, 'left>() =
    inherit monad<mkEither<'E, 'left>>()
        default __.pure'<'a> (a: 'a): hkt<mkEither<'E, 'left>, 'a> = Right a :> _
        default __.bind<'a, 'b> (m: hkt<mkEither<'E, 'left>, 'a>) (k: 'a -> hkt<mkEither<'E, 'left>, 'b>): hkt<mkEither<'E, 'left>, 'b> =
            match m :?> eitherData<'E, 'left, 'a> with
            |  Left l  -> Left l :> _
            |  Right r -> k r
        interface show<mkEither<'E, 'left>> with
            member __.show<'a> (a: hkt<mkEither<'E, 'left>, 'a>) =
                let a = a :?> eitherData<_, _, _>
                a.ToString()

and eitherData<'E, 'e, 'a> =
    | Left of 'e
    | Right of 'a
    interface hkt<mkEither<'E, 'e>, 'a>

let Left<'E, 'e, 'a> (e: 'e) : hkt<mkEither<'E, 'e>, 'a> = Left e :> _
let Right<'E, 'e, 'a> (a: 'a) : hkt<mkEither<'E, 'e>, 'a> = Right a :> _
let (|Left|Right|) (m: hkt<mkEither<'E, 'e>, 'a>) =
    match m :?> eitherData<'E, 'e, 'a> with
    | Left l  -> Left l
    | Right r -> Right r



