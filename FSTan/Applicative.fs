module FSTan.Applicative
open FSTan.HKT
open FSTan.Functor

[<AbstractClass>]
type applicative<'F>() =
    inherit functor<'F>()

      abstract member pure'<'a>  : 'a -> hkt<'F, 'a>

      abstract member ap<'a, 'b> : hkt<'F, ('a -> 'b)> -> hkt<'F, 'a> -> hkt<'F, 'b>
      default si.ap f a = si.liftA2 id f a

      abstract member liftA2<'a, 'b, 'c> :
        ('a -> 'b -> 'c) -> hkt<'F, 'a> -> hkt<'F, 'b> -> hkt<'F, 'c>

      default si.liftA2<'a, 'b, 'c> (f: 'a -> 'b -> 'c) (x: hkt<'F, 'a>) (y: hkt<'F, 'b>) =
        si.ap (si.fmap f x) y

      abstract member ``*>``<'a, 'b> : hkt<'F, 'a> -> hkt<'F, 'b> -> hkt<'F, 'b>
      default si.``*>``<'a, 'b> (a1: hkt<'F, 'a>) (a2: hkt<'F, 'b>): hkt<'F, 'b> =
         si.ap (si.``<$`` id a1) a2

      abstract member ``<*`` : hkt<'F, 'a> -> hkt<'F, 'b> -> hkt<'F, 'a>
      default si.``<*`` a b = si.liftA2 (fun x _ -> x) a b


let pure'<'a, 'F when 'F :> applicative<'F>> :
    'a -> hkt<'F, 'a> =
    getsig<'F>.pure'<'a>

let ap<'a, 'b, 'F when 'F :> applicative<'F>> :
    hkt<'F, ('a -> 'b)> -> hkt<'F, 'a> -> hkt<'F, 'b> =
    getsig<'F>.ap<'a, 'b>

let (<*>) a b = ap a b

let liftA<'a, 'b, 'F when 'F :> applicative<'F>>
    (f : 'a -> 'b) (a : hkt<'F, 'a>) : hkt<'F, 'b>
    = let si = getsig<'F> in
      si.pure' f <*> a

let liftA2<'a, 'b, 'c, 'F when 'F :> applicative<'F>> :
    ('a -> 'b -> 'c) -> hkt<'F, 'a> -> hkt<'F, 'b> -> hkt<'F, 'c>
    = getsig<'F>.liftA2

let ``<*``<'a, 'b, 'F when 'F :> applicative<'F>> :
    hkt<'F, 'a> -> hkt<'F, 'b> -> hkt<'F, 'a> =
    getsig<'F>.``<*``

let (<*) a b = ``<*`` a b


let ``*>``<'a, 'b, 'F when 'F :> applicative<'F>> :
    hkt<'F, 'a> -> hkt<'F, 'b> -> hkt<'F, 'b> =
    getsig<'F>.``*>``

let ( *> ) a b = ``*>`` a b

// TODO, liftA3