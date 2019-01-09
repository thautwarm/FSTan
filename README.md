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


   // create a concrete List type
   type ListSig() =
    // default implements following type classes:
    // - monad (functor and applicative are implemented simultaneously)
    // - show

    inherit mkList<ListSig>()
   type list<'a> = hkt<mkList<ListSig>, 'a>

   let test() =

        let listm: _ list = Do {
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