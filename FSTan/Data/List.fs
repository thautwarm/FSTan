module FSTan.Data.List

open FSTan.HKT
open FSTan.Monad
open FSTan.Show

module List' = List
type List'<'a> = List<'a>

type mkList<'L>() =
    inherit monad<mkList<'L>>()
        static member wrap<'a> (x : List'<'a>): hkt<mkList<'L>, 'a> =
            {wrap = x} :> _
        static member unwrap<'a> (x : hkt<mkList<'L>, 'a>): List'<'a> =
            (x :?> _).wrap

        default si.bind<'a, 'b> (m: hkt<mkList<'L>, 'a>) (k: 'a -> hkt<mkList<'L>, 'b>): hkt<mkList<'L>, 'b> =
            wrap <| List'.collect (unwrap << k) (unwrap m)

        default si.pure'<'a> (a: 'a): hkt<mkList<'L>, 'a> = wrap <| [a]
        interface show<mkList<'L>> with
            member si.show (x: hkt<mkList<'L>, 'a>) =
                let x = unwrap x
                x.ToString()

and listData<'L, 'a> =
    {wrap : List'<'a>}
    interface hkt<mkList<'L>, 'a>




