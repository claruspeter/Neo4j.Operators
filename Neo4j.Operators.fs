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
    {v = "" }// sprintf "%s_%d" t (rnd.Next(9999)) }

let R<'a> : ExpressionRel<'a> =
    let t = typeof<'a>.Name.ToLower()
    {v = "" } //sprintf "%s_%d" t (rnd.Next(9999)) }

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

type private FunAs() =
    static member LinqExpression1<'TResult>(e: Expression<Func<'TResult>>) = e
    static member LinqExpression2<'T, 'TResult>(e: Expression<Func<'T, 'TResult>>) = e
    static member LinqExpression3<'T, 'R, 'TResult>(e: Expression<Func<'T, 'R, 'TResult>>) = e
    static member LinqExpression4<'T, 'R, 'U, 'TResult>(e: Expression<Func<'T, 'R, 'U, 'TResult>>) = e





type Cypher.ICypherFluentQuery with
    member this.Match x =
        this.Match( ExprNodeName x )
    member this.Match x =
        this.Match( x.lhs )
    member this.Return<'a> (x: ExpressionNode<'a>  ) =
        // let para = [| ParameterExpression.Parameter( typeof<Cypher.ICypherResultItem>, x.v ) |]
        // let body = Expression.Variable(typeof< 'a>, x.v)
        // let mthd = MethodCallExpression.Lambda( body, para )
        // //let q = Expr.                                  //.Lambda( a, <@@ a @@> ) :?> Expr<'a>
        // //let aExpr = LeafExpressionConverter.QuotationToLambdaExpression mthd
        // let aCompiled = unbox<Expression<Func<'a>>> mthd
        this.Return<'a>(x.v)
    member this.Return<'a> (x: ExpressionRel<'a>  ) =
        this.Return<'a>(x.v)

