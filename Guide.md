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
let test() =
    let s1 = show <| I 32
    let s2 = show <| S "123"
    let s3 = show A
    let s4 = show B
    printfn "%s\n%s\n%s\n%s" s1 s2 s3 s4

```
Output:
```
isInt 32
isStr 123
A
B
```


Subtypeclassing
=============

If you're familiar with Haskell and related stuffs, you must have an experience with this case:

```haskell

class Applicative m => Monad m where
    -- descriptions of monad typeclass
```

Above code descibes the an example of dependencies between typeclasses, and `Monad`
is exactly a subtypeclass of `Applicative`.

After introducing this sort of constraints, a higher level abstraction is achieved, which enables reaching a higher ratio of code reuse.

For instance, a `Monad` instance requires implementations of many specific methods(`return`, `bind`, `combine` and so on), but we already have a knowledge about subtypeclassing information of `Monad`, it's a subtypeclass of `Functor` and `Applicative`, so if we implement `pure` from `Applicative` and implement `bind` from `Monad` for an instance(eg. `Maybe`) of  `Monad`, we then implement all instances of `Functor`,  `Applicative` and `Monad`.

In F#, we can implement `Either` `Monad`(also `Functor` and `Applicative`).

```FSharp
open FSTan.HKT
open FSTan.Show
open FSTan.Monad

type either<'e, 'a> = hkt<EitherMonad<'e>, 'a>

and EitherMonad<'e>() =
    inherit monad<EitherMonad<'e>>() with
        override __.pure'<'a> (a: 'a): either<'e, 'a> = Right a :> _
        override __.bind<'a, 'b> (m: either<'e, 'a>) (k: 'a -> either<'e, 'b>): either<'e, 'b> =
            match m :?> eitherData<'e, 'a> with
            |  Left l  -> Left l :> _
            |  Right r -> k r
        interface show<EitherMonad<'e>> with
            member __.show<'a> (a: either<'e, 'a>) =
                let a = a :?> eitherData<_, _>
                a.ToString()

and eitherData<'e, 'a> =
    | Left of 'e
    | Right of 'a
    interface either<'e, 'a>

let Left<'e, 'a> (e: 'e) : either<'e, 'a> = Left e :> _
let Right<'e, 'a> (a: 'a) : either<'e, 'a> = Right a :> _
let (|Left|Right|) (m: either<'e, 'a>) =
    match m :?> eitherData<'e, 'a> with
    | Left l  -> Left l
    | Right r -> Right r
```

Abobe codes in Haskell is similar to

```Haskell

data Either e a = Left e | Right a

instance Functor (Either e) where
    -- ... `fmap` and other "methods" are automatically implemented by `bind` and `return`.

instance Applicative (Either e) where
    pure = Right
    -- ...

instance Monad (Either e) where
    return = pure
    bind m k = \case
        Left l   -> Left l
        Right r  -> k r

instance Show (Either e a) where
    -- this section is not that similar to haskell
```

Feel free to check [implementation of `Monad` in FSTan](https://github.com/thautwarm/FSTan/blob/master/FSTan/Monad.fs).



Higher kined types
==================

```FSharp
let test_hkt<'a, 'b, 'c> (f: hkt<'a, 'b>) : hkt<'b, 'c> =
    /// impl
```

In terms of above snippet, if `c` is a concrete type, then `'a` has kind `* -> * -> *`, as well as `b` has kind `* -> *`.


Do notation
=====================

Open `FSTan.Monad`, where a computation expression is provided to be an alternative of `do` in Haskell.

```FSharp

open FSTan.Monad
open FSTan.Control.Trans.State
open FSTan.Data.Maybe
open FSTan.Data.Either

let plusOne<'m when 'm :> monad<'m>> : stateT<int, 'm, unit> = Do {
    let! state = get     // similar to `state <- get` in haskell
    do! put <| state + 1
    return ()
}

let valueMaybe : stateT<int, MaybeSig, unit> = plusOne
// type maybe<'a> = hkt<MaybeSig, 'a>

let valueEither<'e> : stateT<int, EitherSig<'e>, unit> = plusOne
// type either<'e, 'a> = hkt<EitherSig<'e>, 'a>
```