module Neo4j.Operators

open System
open System.Linq
open System.Linq.Expressions
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Linq.RuntimeHelpers
open Neo4jClient


type private FunAs() =
    static member LinqExpression1<'TResult>(e: Expression<Func<'TResult>>) = e
    static member LinqExpression2<'T, 'TResult>(e: Expression<Func<'T, 'TResult>>) = e
    static member LinqExpression3<'T, 'R, 'TResult>(e: Expression<Func<'T, 'R, 'TResult>>) = e
    static member LinqExpression4<'T, 'R, 'U, 'TResult>(e: Expression<Func<'T, 'R, 'U, 'TResult>>) = e

type ExpressionNode< ^a when ^a:(static member NAME:string)> 
    = { v:string }
    with static member inline Init<'a> varName : ExpressionNode<'a> = {v=varName }

type ExpressionRel< ^a when ^a:(static member NAME:string)> 
    = { v:string }
    with static member inline Init<'a> varName : ExpressionRel<'a> = {v=varName }

type ExpressionNodeRel = { lhs: string; }

let inline ExprNodeName< ^b when ^b:(static member NAME:string)> (x: ExpressionNode< ^b > ) =
    sprintf "(%s:%s)" x.v ( ^b:(static member NAME:string) () )

let inline ExprRelName< ^b when ^b:(static member NAME:string)> (x: ExpressionRel< ^b > ) =
    sprintf "%s:%s" x.v ( ^b:(static member NAME:string) () )

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



type Cypher.ICypherFluentQuery with
    member inline this.Match< ^b when ^b:(static member NAME:string)> (x: ExpressionNode< ^b > ) =
        this.Match( ExprNodeName x )
    member inline this.Match (x: ExpressionNodeRel ) =
        this.Match( x.lhs )
