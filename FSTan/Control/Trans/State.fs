module rec FSTan.Control.Trans.State
open FSTan.HKT
open FSTan.Monad
open FSTan.MonadTrans

type stateT<'s, 'm, 'a when 'm :> monad<'m>> = hkt<stateTrans<'s, 'm>, 'a>
and stateTrans<'s, 'm when 'm :> monad<'m>>() = 
    inherit monad<stateTrans<'s, 'm>>() with
        override si.pure'<'a> (a: 'a): stateT<'s, 'm, 'a> =
            StateT <| fun s -> return' (a, s)

        override si.bind<'a, 'b> 
            (m: stateT<'s, 'm, 'a>) 
            (k: 'a -> stateT<'s, 'm, 'b>): stateT<'s, 'm, 'b> =
            StateT <| fun s ->
            runStateT m s >>= fun (a, s') ->
            runStateT (k a) s'

        interface hkt<'s, 'm>
        interface monadTrans<stateTrans<'s, 'm>, 'm> with
            member si.lift<'a> (m: hkt<'m, 'a>): hkt<stateTrans<'s, 'm>, 'a> =
                StateT <| fun (s: 's) ->
                m >>= fun a -> return' (a, s)

and stateTData<'s, 'm, 'a when 'm :> monad<'m>> = 
    | StateT' of ('s -> hkt<'m, ('a * 's)>)
    interface stateT<'s, 'm, 'a>

let runStateT<'s, 'm, 'a when 'm :> monad<'m>> (m: stateT<'s, 'm, 'a>): ('s -> hkt<'m, ('a * 's)>) =
    let (StateT' f) = m :?> stateTData<'s, 'm, 'a>
    f

let StateT<'s, 'm, 'a when 'm :> monad<'m>>  (f: 's -> hkt<'m, 'a * 's>) : stateT<'s, 'm, 'a> =
    (StateT' f) :> _

let (|StateT|) (m: stateT<'s, 'm, 'a>) =
    let (StateT' f) = m :?> stateTData<'s, 'm, 'a>
    in StateT f

let state f = StateT (return' << f)

let get<'s, 'm when 'm :> monad<'m>> : stateT<'s, 'm, 's> =
    state <| fun s -> (s, s)

let put<'s, 'm when 'm :> monad<'m>> (s: 's) : stateT<'s, 'm, unit> = 
    state <| fun _ -> (), s

let modify f = state <| fun s -> (), f s

let gets f = state <| fun s -> f s, s

let evalStateT<'s, 'm, 'a when 'm :> monad<'m>> : stateT<'s, 'm, 'a> -> 's -> hkt<'m, 'a> =
    fun m s -> Do {
        let! (a, _) = runStateT m s
        return a
    }

let execStateT<'s, 'm, 'a when 'm :> monad<'m>> : stateT<'s, 'm, 'a> -> 's -> hkt<'m, 's> =
    fun m s -> Do {
        let! (_, s) = runStateT m s
        return s
    }