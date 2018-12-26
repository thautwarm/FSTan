module FSTan.Show
open FSTan.HKT


type show<'s> = 
    interface
        abstract member show<'a> : hkt<'s, 'a> -> string
    end

let show<'a, 's when 's :> show<'s>> (a: hkt<'s, 'a>) = getsig<'s>.show a

