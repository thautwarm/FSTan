# FSTan

Exactly a full-featured and practical implementation typeclasses and higher kinded types in F#.

For manuals check [Guide.md](https://github.com/thautwarm/FSTan/blob/master/Guide.md), where you'll be told how to use these concise typeclasses, higher kined types and constraints.



## Motivation and Features

There are also other similar implementations in FSharp like `Higher` and `FSharpPlus`, but they're not able to provide all the features listed below, which motivate me create a better one:

- Support instance resolution.
- Support ad-hoc polymorphism.
- Support to create a typeclass and add constraints to it.
- Support subtypeclassing.
- Support to directly access type constructor.
- Support default implementations for typeclass.
- All above operations are quite lightweighted and not implemented in a magic way.

Yes, exactly, it's a what many and I have dreamed about for so long.


## Limitation

1. The performance might hurt in some scenarios, for each the datatype works with
higher kinded types have to be upcasted to an unique abstract class, for intsance,
`maybe<'a>` has to be casted to `hkt<Maybe, 'a>`.

2. For some builtin datatypes cannot be interfaced with `hkt`, an extra wrapper class is
required to work with higher kined types.

    For instance, interface type `listData<'a>` is required for the builtin `List<'a>`.

    You can use `wrap` and `unwrap` to transform datatypes from `List<'a>` to `hlist<'a>`, vice versa.

   ```FSharp
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

   let test() =

        let listm = Do {
            let! x = wrap [1, 2, 3]
            wrap [x]
        }
        // listm : resolved to be list<int>

        let f : int -> string = ""
        fmap f listm
        // return value is resolved to be list<string>
   ```
3. Cannot implement instance for datatypes that are not constructed by a type constructor.
For instance, you cannot implement any typeclass for all primitives types like integers, floats and so on, unless you wrap them ...

4. Cannot separate typeclass instances from datatype definitions, which means that you cannot extend an existed datatype and might hurt.