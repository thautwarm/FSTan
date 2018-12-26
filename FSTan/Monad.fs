module FSTan.Monad
open FSTan.HKT
open FSTan.Applicative


[<AbstractClass>]
type monad<'M>() =
    inherit applicative<'M>()
       abstract member return'<'a>     : 'a -> hkt<'M, 'a>
       abstract member bind<'a, 'b>    : hkt<'M, 'a> -> ('a -> hkt<'M, 'b>) -> hkt<'M, 'b>
       abstract member combine<'a, 'b> :  
        hkt<'M, 'a> -> hkt<'M, 'b> -> hkt<'M, 'b>
       
       default si.combine ma mb = si.bind ma <| fun _ -> mb
       default si.return' a = si.pure' a
       
       abstract member fail<'a> : string -> hkt<'M, 'a>
       default __.fail s = failwith s

       default si.fmap<'a, 'b>  (f : 'a -> 'b)  (m : hkt<'M, 'a>) : hkt<'M, 'b> =
            si.bind m (f >> si.return')

let return'<'a, 'M when 'M :> monad<'M>> = getsig<'M>.return'<'a>
let bind<'a, 'b, 'M when 'M :> monad<'M>> = getsig<'M>.bind<'a, 'b>
let (>>=) a b = bind a b
let combine<'a, 'b, 'M when 'M :> monad<'M>> = getsig<'M>.combine<'a, 'b>
let (>>) a b = combine a b

