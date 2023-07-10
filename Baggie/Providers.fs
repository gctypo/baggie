namespace Baggie

open System
open System.IO

type IAppConfigProvider =
    abstract member GetConfigValue : key: string -> string

type IFileContentProvider =
    abstract member Exists : path: string -> bool
    abstract member ReadAllText : path: string -> string

type FileContentProvider() =
    interface IFileContentProvider with
        member this.Exists (path: string) = File.Exists(path)
        member this.ReadAllText (path: string) = File.ReadAllText(path)

type ITimeNowProvider =
    abstract member Now : DateTime

type TimeNowProvider() =
    interface ITimeNowProvider with
        member this.Now : DateTime = DateTime.Now
