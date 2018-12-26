﻿// Learn more about F# at http://fsharp.org

open System

open FSTan.HKT

// define a simple typeclass
[<AbstractClass>]
type show<'s>() =
    abstract member show<'a> : hkt<'s, 'a> -> string

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

open FSTan.Monad
open FSTan.Data.Maybe
open FSTan.Data.List

let some<'a> (a: 'a): maybe<'a> = wrap << Some <| a
[<EntryPoint>]
let main argv =
    test()
    
    
    let m1 =
        doNotation {
            let! x = some 1
        
            return ""
        }
    
    let m2 = 
        doNotation {
            let! x = wrap [1; 2; 3] : _ hlist
            return x * 3
        }
    
    0 // return an integer exit code