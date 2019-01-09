module FSTan.Data.List

open FSTan.HKT
open FSTan.Monad
open FSTan.Show

module List' = List
type List'<'a> = List<'a>

type 'a list = hkt<ListSig, 'a>
and ListSig() =
    inherit monad<ListSig>() with
        override __.bind<'a, 'b> (m: list<'a>) (k: 'a -> list<'b>) =
            wrap <| List'.collect (unwrap << k) (unwrap m)

        override __.pure'<'a> (a: 'a) : list<'a> = wrap <| [a]
        static member wrap<'a> (x : List'<'a>): list<'a> =  {wrap = x} :> _
        static member unwrap<'a> (x : list<'a>): List'<'a> =  (x :?> _).wrap
        interface show<ListSig> with
            member __.show (x: 'a list) =
                let x = unwrap x
                x.ToString()

and listData<'a> =
    {wrap : List'<'a>}
    interface list<'a>




