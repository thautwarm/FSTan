module FSTan.MonadTrans
open FSTan.HKT
open FSTan.Monad

type monadTrans<'t, 'm when 'm :> monad<'m>> = interface
    abstract lift<'a> :
        hkt<'m, 'a> -> hkt<'t, 'a>
end

let lift<'m, 'a, 't when 't :> monadTrans<'t, 'm>> = getsig<'t>.lift<'a>
