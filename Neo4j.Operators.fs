module Neo4j.Operators

open System
open System.Collections.Generic
open System.Linq
open System.Linq.Expressions
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.RuntimeHelpers
open Neo4jClient

let private rnd = Random()

type ExpressionNode< 'a> 
    = { v:string }
    with static member inline Init<'a> varName : ExpressionNode<'a> = {v=varName }

type ExpressionRel< 'a > 
    = { v:string }
    with static member inline Init<'a> varName : ExpressionRel<'a> = {v=varName }

let N<'a> : ExpressionNode<'a> =
    let t = typeof<'a>.Name.ToLower()
    {v = "" }

let R<'a> : ExpressionRel<'a> =
    let t = typeof<'a>.Name.ToLower()
    {v = "" }

let NodeVar<'a> varName : ExpressionNode<'a> =
    ExpressionNode.Init<'a> varName

let RelVar<'a> varName : ExpressionRel<'a> =
    ExpressionRel.Init<'a> varName


type ExpressionNodeRel = { lhs: string; }

let ExprNodeName<'a> (x: ExpressionNode< 'a > ) =
    sprintf "(%s:%s)" x.v ( x.GetType().GenericTypeArguments.[0].Name )

let ExprRelName<'a> (x: ExpressionRel< 'a > ) =
    sprintf "%s:%s" x.v ( x.GetType().GenericTypeArguments.[0].Name )

let inline (@.@) (ev:ExpressionNode<'b>) (prop:string) =
    sprintf "%s.%s" (ev.v) prop

let (@=) prop (propVal:'a) =
    prop + "=" + propVal.ToString()

let inline ( --> ) (a:ExpressionNode<'a>) (b:ExpressionNode<'b>) =
    sprintf "%s-->%s" (ExprNodeName a) (ExprNodeName b)

let inline ( -| ) (a:ExpressionNode<'a>) (b:ExpressionRel<'b>) =
    { lhs = sprintf "%s-[%s" (ExprNodeName a) (ExprRelName b) }

let inline ( |-> ) (a:ExpressionNodeRel) (b:ExpressionNode<'a>) =
    { lhs = sprintf "%s]->%s" a.lhs (ExprNodeName b) }

let inline ( <-| ) (a:ExpressionNodeRel) (b:ExpressionRel<'a>) =
    { lhs = sprintf "%s<-[%s" a.lhs (ExprRelName b) }

let inline ( |- ) (a:ExpressionNodeRel) (b:ExpressionNode<'b>) =
    { lhs = sprintf "%s]-%s" a.lhs (ExprNodeName b) }

let private toLinq (expr : Expr<'b>) =
    let linq = LeafExpressionConverter.QuotationToExpression expr
    Expression.Lambda<Func<'b>>(linq, [||]) 

type Cypher.ICypherFluentQuery with
    member this.Match x =
        this.Match( ExprNodeName x )
    member this.Match x =
        this.Match( x.lhs )
    member this.Return<'a> (x: ExpressionNode<'a>  ) =
        this.Return<'a>(x.v)
    member this.Return((x: ExpressionNode<'a>), (y: ExpressionNode<'b>)) =
        let expr = <@ ( Cypher.Return.As<'a>(x.v), Cypher.Return.As<'b>(y.v) ) @>
        let linqExpr = toLinq( expr )
        this.Return( linqExpr )
    member this.Return<'a> (x: ExpressionRel<'a>  ) =
        this.Return<'a>(x.v)

