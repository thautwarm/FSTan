module FSTan.HKT
type hkt<'K, 'T> = interface end

[<GeneralizableValue>]
let getsig<'a> = Unchecked.defaultof<'a>

// Some builtin data types like Map, List, Option cannot be interfaced
// with `hkt`, so we have to wrap them. 
// Following methods provide a common interface to access `wrap` and `unwrap`
// operations for all wrapped types.
let inline wrap<'o, ^f, 'a when ^f : (static member wrap : 'o -> hkt<'f, 'a>)> (o: 'o) : hkt< ^f, 'a> = 
    (^f : (static member wrap : 'o -> hkt<'f, 'a>) o)

let inline unwrap<'o, ^f, 'a when ^f : (static member unwrap : hkt<'f, 'a> -> 'o)> (f : hkt< ^f, 'a>) : 'o = 
    (^f : (static member unwrap : hkt<'f, 'a> -> 'o) f)