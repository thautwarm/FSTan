module FSTan.Monoid
open FSTan.HKT

[<AbstractClass>]
type semigroup<'s>() =

    abstract member op<'a> :
        hkt<'s, 'a> -> hkt<'s, 'a> -> hkt<'s, 'a>
    abstract member sconcat<'a> :
        hkt<'s, 'a> -> hkt<'s, 'a>
    abstract member stimes<'a> :
        int -> hkt<'s, 'a> -> hkt<'s, 'a>

    default si.stimes a b = semigroup<'s>.stimesDefault si a b

    static member stimesDefault si y0 x0 =
        if y0 <= 0
        then   failwith "stimes: positive multiplier expected"
        else
        let rec f x y =
            match y with
            | _ when y%2 = 0 ->
                f (si.op x x) (y / 2)
            | 1  -> x
            | _ -> g (si.op x x) (y / 2) x
        and g x y z =
            match y with
            | _ when y%2 = 0 ->
                g (si.op x x) (y / 2) z
            | 1 -> si.op x z
            | _ -> g (si.op x x) (y / 2) (si.op x z)
        in f x0 y0


[<AbstractClass>]
type monoid<'s>() =
    inherit semigroup<'s>()
        abstract member mempty<'m> : unit -> hkt<'s, 'm>
        abstract member mappend<'m> : hkt<'s, 'm> -> hkt<'s, 'm> -> hkt<'s, 'm>
        abstract member mconcat<'m> : hkt<'s, 'm> list -> hkt<'s, 'm>

        default si.mappend a b =
            si.op a b

        default si.mconcat xs =
            let empty = si.mempty()
            List.foldBack si.mappend xs empty