module FSTan.Data.Identity

open FSTan.HKT
open FSTan.Monad
open FSTan.Show

type 'a identity = hkt<IdentitySig, 'a>

and IdentitySig() =
    inherit monad<IdentitySig>() with
        override __.bind<'a, 'b> (m: identity<'a>) (k: 'a -> identity<'b>) =
            k (unwrap m)

        override __.pure'<'a> (a: 'a) : identity<'a> = wrap a
        static member wrap<'a> (x : 'a): 'a identity = {wrap = x} :> _
        static member unwrap<'a> (x :  'a identity): 'a =  (x :?> _).wrap
        interface show<IdentitySig> with
            member __.show (x: 'a identity) =
                let x = unwrap x
                x.ToString()

and identityData<'a> =
    {wrap : 'a}
    interface 'a identity

