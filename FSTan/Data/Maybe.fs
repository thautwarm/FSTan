module FSTan.Data.Maybe

open FSTan.HKT
open FSTan.Monad

type maybe<'a> = hkt<Maybe, 'a>
and Maybe() =
    inherit monad<Maybe>() with
        static member si = Maybe()
        override __.bind<'a, 'b> (m: maybe<'a>) (k: 'a -> maybe<'b>) =
            let m = unwrap m
            match m with
            | Some a -> k a
            | None   -> wrap None
        
        override __.pure'<'a> (a: 'a) : maybe<'a> = wrap <| Some a
        static member wrap<'a> (x : Option<'a>): maybe<'a> =  {wrap = x} :> _
        static member unwrap<'a> (x : maybe<'a>): Option<'a> =  (x :?> _).wrap

and MaybeData<'a> = 
    {wrap : Option<'a>}
    interface maybe<'a>


