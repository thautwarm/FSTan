module FSTan.HKT
type hkt<'K, 'T> = interface end

open System
open System.Reflection

let private ts  = Array.zeroCreate<Type> 0

[<GeneralizableValue>]
let getsig<'a> =
    // There is another way to do so: add constraint
    // `'a when 'a: (new: unit -> 'a)`.
    // However, if this way is adopted, users have to
    // manually mark each generic typevar of a type constructor,
    // which could be verbose and annoying.
    // With above considerations, I use reflection instead.
    let t = typeof<'a>
    let f = t.GetConstructor(
                BindingFlags.Instance ||| BindingFlags.Public,
                null,
                CallingConventions.HasThis,
                ts,
                null)
    let o = f.Invoke([||])
    o :?> 'a

// Some builtin data types like Map, List, Option cannot be interfaced
// with `hkt`, so we have to wrap them.
// Following methods provide a common interface to access `wrap` and `unwrap`
// operations for all wrapped types.
let inline wrap<'o, ^f, 'a when ^f : (static member wrap : 'o -> hkt<'f, 'a>)> (o: 'o) : hkt< ^f, 'a> =
    (^f : (static member wrap : 'o -> hkt<'f, 'a>) o)

let inline unwrap<'o, ^f, 'a when ^f : (static member unwrap : hkt<'f, 'a> -> 'o)> (f : hkt< ^f, 'a>) : 'o =
    (^f : (static member unwrap : hkt<'f, 'a> -> 'o) f)