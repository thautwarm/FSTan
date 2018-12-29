module FSTan.Data.List

open FSTan.HKT
open FSTan.Monad
open FSTan.Show

type hlist<'a> = hkt<HList, 'a>
and HList() =
    inherit monad<HList>() with
        override __.bind<'a, 'b> (m: hlist<'a>) (k: 'a -> hlist<'b>) =
            let f x = 
                let m: hlist<'b> = k x
                let lst : 'b list = unwrap m
                lst
            wrap <| List.collect f (unwrap m)

        override __.pure'<'a> (a: 'a) : hlist<'a> = wrap <| [a]
 
        static member wrap<'a> (x : List<'a>): hlist<'a> =  {wrap = x} :> _
        static member unwrap<'a> (x : hlist<'a>): List<'a> =  (x :?> _).wrap
        interface show<HList> with
            member __.show (x: hlist<'a>) =
                let x = unwrap x : _ list
                x.ToString()

and hListData<'a> =
    {wrap : List<'a>}
    interface hlist<'a>
        
        


