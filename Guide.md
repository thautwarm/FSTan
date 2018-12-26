# FSTan Guide


Typeclasses
=============


Typeclassess are achived through abstract classes, which makes it works perfect for both subtypeclassing and default implementations.

If some type is constructed with a type constructor, you can implement `show` class for it. 

Let's have a look at how to write a `show` class and use it in polymorphism functions and even operators.


```FSharp

open FSTan.HKT

// define a simple typeclass
[<AbstractClass>]
type show<'s>() =
    abstract member show<'a> : hkt<'s, 'a> -> string

// I have a typeclass, 
// I have 2 datatypes, 
// Oh! 
// Polymorphism!
let show<'a, 's when 's :> show<'s>> = getsig<'s>.show<'a>

type myData1<'a> = // define datatype
    | A | B | C
    interface hkt<MyTypeCons1, 'a> 

and MyTypeCons1() = 
    // define type constructor
    // in F#, we don't really have this, but
    // we can leverage a signature type(yes, this is just a signature) 
    // and `hkt`(check FSTan.HKT, not magic at all) 
    // to fully simulate a type constructor. 
    inherit show<MyTypeCons1>() with
        override si.show a =
            // This conversion can absolutely succeed
            // for there is only one datatype which
            // interfaces hkt<MyTypeCons1, 'a>
            let a = a :?> _ myData1

            sprintf "%A" a 


type myData2<'a> = // define datatype
    | I of int
    | S of string
    interface hkt<MyTypeCons2, 'a> 
and MyTypeCons2() = 
    // define type constructor
    // in F#, we don't really have this, but
    // we can leverage a signature type(yes, this is just a signature) 
    // and `hkt`(check FSTan.HKT, not magic at all) 
    // to fully simulate a type constructor.
    inherit show<MyTypeCons2>() with
        override si.show a =
            let a = a :?> _ myData2
            match a with
            | I a -> sprintf "isInt %d" a
            | S a -> sprintf "isStr %s" a
```


Subtypeclassing
=============

Check https://github.com/thautwarm/FSTan/blob/master/FSTan/Functor.fs.


Higher kined types
==================

A signature type to represent a type constructor in FSTan:

```FSharp
type Sig = ..

let test_hkt<'a, 'b, 'c> (f: hkt<'a, 'b>) : hkt<'b, 'c> = 
    /// impl
```

In terms of above snippet, if `c` is a concrete type, then `'a` is kinded of `* -> * -> *`, as well as `b` is kinded of `* -> *`.