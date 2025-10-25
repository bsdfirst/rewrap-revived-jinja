module internal Parsing.Language

open System
open Parsing_

type Language =
    private Language of string * array<string> * array<string> * DocumentProcessor

module Language =

    let private split (s: string) =
        s.ToLower().Split([|'|'|], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun x -> x.Trim())

    // Add .j2/.jinja/.jinja2 variants for extensions
    let private withJinjaVariants (exts: string) =
        let toks = split exts
        if toks.Length = 0 then toks
        else
            toks
            |> Array.collect (fun t -> [| t; t + ".j2"; t + ".jinja"; t + ".jinja2" |])
            |> Array.distinct

    // Add "jinja" and jinja-<id> variants for every id
    let private withJinjaIdVariants (ids: string array) =
        ids
        |> Array.collect (fun id -> [| id; "jinja-" + id |])
        |> Array.append [| "jinja" |]  // also include plain "jinja"
        |> Array.distinct

    // 1: display name, 2: aliases (pipe-separated), 3: extensions (pipe-separated), 4: parser
    let create (name: string) (aliases: string) (exts: string) parser : Language =
        let baseIds = Array.append [| name.ToLower() |] (split aliases)
        let ids' = withJinjaIdVariants baseIds
        let exts' = withJinjaVariants exts
        Language(name, ids', exts', parser)

    let name (Language(n,_,_,_)) = n
    let parser (Language(_,_,_,p)) = p

    let matchesFileLanguage (fileLang: string) (Language(_,ids,_,_)) =
        Seq.contains (fileLang.ToLower()) ids

    let matchesFilePath (path: string) (Language(_,_,exts,_)) =
        let fileName = path.ToLower().Split('\\', '/') |> Array.last
        let tryMatch : string -> bool = function
            | ext when ext.StartsWith(".") -> fileName.EndsWith(ext)
            | fullname -> fileName.Equals(fullname)
        Seq.exists tryMatch exts
