module rec FSTan.Control.Trans.State
open FSTan.HKT
open FSTan.Monad
open FSTan.MonadTrans

type mkStateTras<'ST, 's, 'm when 'm :> monad<'m>>() =
    inherit monad<mkStateTras<'ST, 's, 'm>>()
        default si.pure'<'a> (a: 'a): hkt<mkStateTras<'ST, 's, 'm>, 'a> =
            StateT <| fun s -> return' (a, s)

        override si.bind<'a, 'b>
            (m: hkt<mkStateTras<'ST, 's, 'm>, 'a>)
            (k: 'a -> hkt<mkStateTras<'ST, 's, 'm>, 'b>): hkt<mkStateTras<'ST, 's, 'm>, 'b> =
            StateT <| fun s ->
            runStateT m s >>= fun (a, s') ->
            runStateT (k a) s'

        interface hkt<'s, 'm>
        interface monadTrans<mkStateTras<'ST, 's, 'm>, 'm> with
            member si.lift<'a> (m: hkt<'m, 'a>): hkt<mkStateTras<'ST, 's, 'm>, 'a> =
                StateT <| fun (s: 's) ->
                m >>= fun a -> return' (a, s)

and stateTData<'ST, 's, 'm, 'a when 'm :> monad<'m>> =
    | StateT' of ('s -> hkt<'m, ('a * 's)>)
    interface hkt<mkStateTras<'ST, 's, 'm>, 'a>

let runStateT<'ST, 's, 'm, 'a when 'm :> monad<'m>> (m: hkt<mkStateTras<'ST, 's, 'm>, 'a>): ('s -> hkt<'m, ('a * 's)>) =
    let (StateT' f) = m :?> stateTData<'ST, 's, 'm, 'a>
    f

let StateT<'ST, 's, 'm, 'a when 'm :> monad<'m>>  (f: 's -> hkt<'m, 'a * 's>) : hkt<mkStateTras<'ST, 's, 'm>, 'a> =
    (StateT' f) :> _

let (|StateT|) (m: hkt<mkStateTras<'ST, 's, 'm>, 'a>) =
    let (StateT' f) = m :?> stateTData<'ST, 's, 'm, 'a>
    in StateT f

let state f = StateT (return' << f)

let get<'ST, 's, 'm when 'm :> monad<'m>> : hkt<mkStateTras<'ST, 's, 'm>, 's> =
    state <| fun s -> (s, s)

let put<'ST, 's, 'm when 'm :> monad<'m>> (s: 's) : hkt<mkStateTras<'ST, 's, 'm>, unit> =
    state <| fun _ -> (), s

let modify f = state <| fun s -> (), f s

let gets f = state <| fun s -> f s, s

let evalStateT<'ST, 's, 'm, 'a when 'm :> monad<'m>> : hkt<mkStateTras<'ST, 's, 'm>, 'a> -> 's -> hkt<'m, 'a> =
    fun m s -> Do {
        let! (a, _) = runStateT m s
        return a
    }

let execStateT<'ST, 's, 'm, 'a when 'm :> monad<'m>> : hkt<mkStateTras<'ST, 's, 'm>, 'a> -> 's -> hkt<'m, 's> =
    fun m s -> Do {
        let! (_, s) = runStateT m s
        return s
    }