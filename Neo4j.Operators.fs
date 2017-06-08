module Neo4j.Operators

open System
open System.Collections.Generic
open System.Linq
open System.Linq.Expressions
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.RuntimeHelpers
open Neo4jClient

let private rnd = Random()

type ExpressionElement<'a>( varName: string) =
    new() = ExpressionElement("")
    member this.VarName = varName

type ExpressionNode<'a> (varName) =
    inherit ExpressionElement<'a>(varName)
    new() = ExpressionNode<'a>("")

type ExpressionRel<'a> (varName) =
    inherit ExpressionElement<'a>(varName)
    new() = ExpressionRel<'a>("")

let N<'a> : ExpressionNode<'a> =
    ExpressionNode<'a>()

let R<'a> : ExpressionRel<'a> =
    ExpressionRel<'a>()


type ExpressionPart = { lhs: string; }
type ANY = string

let ExprNodeName<'a> (x: ExpressionNode< 'a > ) =
    let typename = x.GetType().GenericTypeArguments.[0].Name
    match typename with
    | "ANY"
    | "String" -> 
        sprintf "(%s)" x.VarName
    | _ -> sprintf "(%s:%s)" x.VarName typename

let ExprRelName<'a> (x: ExpressionRel< 'a > ) =
    let typename = x.GetType().GenericTypeArguments.[0].Name
    match typename with
    | "ANY"
    | "String" -> 
        sprintf "%s" x.VarName
    | _ -> sprintf "%s:%s" x.VarName typename

let inline (@.@) (ev:ExpressionNode<'b>) (prop:string) =
    sprintf "%s.%s" (ev.VarName) prop

let (@=) prop (propVal:'a) =
    prop + "=" + propVal.ToString()

let inline ( --> ) (a:ExpressionNode<'a>) (b:ExpressionNode<'b>) =
    sprintf "%s-->%s" (ExprNodeName a) (ExprNodeName b)

let inline ( -| ) (a:ExpressionNode<'a>) (b:ExpressionRel<'b>) =
    { lhs = sprintf "%s-[%s" (ExprNodeName a) (ExprRelName b) }

let inline ( |-> ) (a:ExpressionPart) (b:ExpressionNode<'a>) =
    { lhs = sprintf "%s]->%s" a.lhs (ExprNodeName b) }

let inline ( <-| ) (a:ExpressionPart) (b:ExpressionRel<'a>) =
    { lhs = sprintf "%s<-[%s" a.lhs (ExprRelName b) }

let inline ( |- ) (a:ExpressionPart) (b:ExpressionNode<'b>) =
    { lhs = sprintf "%s]-%s" a.lhs (ExprNodeName b) }

let private toLinq (expr : Expr<'b>) =
    let linq = LeafExpressionConverter.QuotationToExpression expr
    Expression.Lambda<Func<'b>>(linq, [||]) 

let retFunc (a: ExpressionElement<'a>) =
    match a with
    | :? ExpressionRel<'a> -> sprintf "type(%s)" a.VarName
    | _ ->a.VarName

type Cypher.ICypherFluentQuery with
    member this.Match x =
        this.Match( ExprNodeName x )
    member this.Match x =
        this.Match( x.lhs )
    member this.Return<'a> (x: ExpressionElement<'a>  ) =
        this.Return<'a>(x.VarName)
    member this.Return((a: ExpressionElement<'a>), (b: ExpressionElement<'b>)) =
        let aName = retFunc a
        let bName = retFunc b
        let expr = 
            <@ ( 
                    Cypher.Return.As<'a>(aName)
                    , Cypher.Return.As<'b>(bName)
            ) @>
        let linqExpr = toLinq( expr )
        this.Return( linqExpr )
    member this.Return((a: ExpressionElement<'a>), (b: ExpressionElement<'b>), (c: ExpressionElement<'c>)) =
        let aName = retFunc a
        let bName = retFunc b
        let cName = retFunc c
        let expr = 
            <@ ( 
                    Cypher.Return.As<'a>(aName)
                    , Cypher.Return.As<'b>(bName)
                    , Cypher.Return.As<'c>(cName) 
            ) @>
        let linqExpr = toLinq( expr )
        this.Return( linqExpr )

