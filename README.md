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

2. For some builtin datatypes cannot be interfaced with `hkt`, an      extra wrapper class is
required to work with higher kined types.   

    For instance, interface type `listData<'a>` is required for the builtin `List<'a>`. 
    
    You can use `wrap` and `unwrap` to transform datatypes from `List<'a>` to `hlist<'a>`, vice versa.

   ```FSharp
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
    
            static member inline wrap<'a> (x : List<'a>): hlist<'a> =  {wrap = x} :> _
            static member inline unwrap<'a> (x : hlist<'a>): List<'a> =  (x :?> _).wrap

    and hListData<'a> = 
        {wrap : List<'a>}
        interface hlist<'a>

   let test() = 
        
        let listm = monad {
            let! x = wrap [1, 2, 3]
            wrap [x]
        }
        // listm : resolved to be hlist<int>

        let f : int -> string = ""
        fmap f listm
        // return value is resolved to be hlist<string>
   ```
3. Cannot implement instance for datatypes that are not constructed by a type constructor.   
For instance, you cannot implement any typeclass for all primitives types like integers, floats and so on, unless you wrap them ...

