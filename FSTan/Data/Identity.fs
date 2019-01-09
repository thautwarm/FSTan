module FSTan.Data.Identity

open FSTan.HKT
open FSTan.Monad
open FSTan.Show

type mkIdentity<'I>() =
    inherit monad<mkIdentity<'I>>()
        override __.bind<'a, 'b> (m: hkt<mkIdentity<'I>, 'a>) (k: 'a -> hkt<mkIdentity<'I>, 'b>) =
            k (unwrap m)

        override __.pure'<'a> (a: 'a) : hkt<mkIdentity<'I>, 'a> = wrap a
        static member wrap<'a> (x : 'a): hkt<mkIdentity<'I>, 'a> = {wrap = x} :> _
        static member unwrap<'a> (x :  hkt<mkIdentity<'I>, 'a>): 'a =  (x :?> _).wrap
        interface show<mkIdentity<'I>> with
            member __.show (x: hkt<mkIdentity<'I>, 'a>) =
                let x = unwrap x
                x.ToString()

and identityData<'I, 'a> =
    {wrap : 'a}
    interface hkt<mkIdentity<'I>, 'a>

