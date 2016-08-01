﻿namespace Rezoom
open System.Threading.Tasks

[<AbstractClass>]
type Errand() =
    abstract member Identity : obj
    default __.Identity = null
    abstract member DataSource : obj
    default __.DataSource = null
    abstract member SequenceGroup : obj
    default __.SequenceGroup = null
    abstract member Idempotent : bool
    default __.Idempotent = false
    abstract member Mutation : bool
    default __.Mutation = true
    abstract member Parallelizable : bool
    default __.Parallelizable = false
    abstract member InternalPrepare : ServiceContext -> (unit -> obj Task)

[<AbstractClass>]
type Errand<'a>() =
    inherit Errand()

[<AbstractClass>]
type AsynchronousErrand<'a>() =
    inherit Errand<'a>()
    static member private BoxResult(task : 'a Task) =
        box task.Result
    abstract member Prepare : ServiceContext -> (unit -> 'a Task)
    override this.InternalPrepare(cxt) : unit -> obj Task =
        let typed = this.Prepare(cxt)
        fun () ->
            let t = typed()
            t.ContinueWith(AsynchronousErrand<'a>.BoxResult, TaskContinuationOptions.ExecuteSynchronously)

[<AbstractClass>]
type SynchronousErrand<'a>() =
    inherit Errand<'a>()
    abstract member Prepare : ServiceContext -> (unit -> 'a)
    override this.InternalPrepare(cxt) : unit -> obj Task =
        let sync = this.Prepare(cxt)
        fun () ->
            Task.FromResult(box (sync()))
        
        
    
