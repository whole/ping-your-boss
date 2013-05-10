

module Helpers
open System

let prettifyOutput( input : string list) =
    let parsed = List.map (fun s -> s + " online") input 
    let stringifyParsed = String.Join(System.Environment.NewLine, parsed)
    stringifyParsed