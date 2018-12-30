module FSTan.Functor

open FSTan.HKT

[<AbstractClass>]
type functor<'F>() =
    abstract member fmap<'a, 'b> :
        ('a -> 'b) -> hkt<'F, 'a> -> hkt<'F, 'b>
    abstract member ``<$``<'a, 'b> : 'a -> hkt<'F, 'b> -> hkt<'F, 'a>
    default si.``<$`` a b =
        let const' a _ = a
        (si.fmap << const') a b

let fmap<'a, 'b, 'F when 'F :> functor<'F>> :
    ('a -> 'b) -> hkt<'F, 'a> -> hkt<'F, 'b> =
    getsig<'F>.fmap


let ``<$``<'a, 'b, 'F when 'F :> functor<'F> > :
    'a -> hkt<'F, 'b> -> hkt<'F, 'a> = getsig<'F>.``<$``

let (<<|) a b = ``<$`` a b