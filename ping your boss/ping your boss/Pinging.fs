module Pinging

#if interactive
#r "System.DirectoryServices.dll"
#endif
open System
open System.Collections
open System.DirectoryServices
open System.Net.NetworkInformation
open System.Threading.Tasks

         
let prettifyOutput( input : string list) =
    let parsed = List.map (fun s -> s + " online") input 
    let stringifyParsed = String.Join(System.Environment.NewLine, parsed)
    stringifyParsed

let hosts() =
    use searcher =
        new DirectorySearcher(new DirectoryEntry(),
            Filter="(objectClass=computer)", PageSize=50000)
    (searcher.FindAll() :> IEnumerable)
    |> Seq.cast<SearchResult>
    |> Seq.map (fun x -> x.GetDirectoryEntry().Name)
    |> Seq.map (fun n -> n.Substring(n.IndexOf("=")+1))
    |> Seq.toList

let checkHosts hosts =
    let rec ping attempts (host:string) =
        async {
            let! pingResult =
                (new Ping()).SendPingAsync(host)
                |> Async.AwaitTask |> Async.Catch
            match pingResult with
            | Choice2Of2 e -> return false
            | Choice1Of2 reply when reply.Status=IPStatus.Success -> return true
            | _ when attempts > 0 -> return! ping (attempts-1) host
            | _ -> return false
            }

    let results =
        hosts
        |> Seq.map (ping 1)
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Seq.toList
    List.zip results hosts
let availableHosts(computer: List<string>) =
    if computer.[0].Length = 0 then
        hosts()
        |> checkHosts
        |> List.filter fst
        |> List.map snd
        |> prettifyOutput
    else 
        computer
        |> checkHosts
        |> List.filter fst
        |> List.map snd
        |> prettifyOutput
 
