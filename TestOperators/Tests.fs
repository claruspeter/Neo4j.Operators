module Test

open System
open System.Collections.Generic
open NUnit.Framework
open FsUnit
open Foq
open Neo4jClient
open Neo4j.Operators

type Something() = class end
type SomethingElse() = class end
type RELATES_TO() = class end

let node = ExpressionNode<Something> "Fred"
let node2 = ExpressionNode<SomethingElse> "Joe"
let rel = ExpressionRel<RELATES_TO> "r"

let client = Mock<IRawGraphClient>()
                .As<IGraphClient>()
                .Create()
let query : Cypher.ICypherFluentQuery = Cypher.CypherFluentQuery(client) :> _

module Operators =
    [<Test>]
    let ``make node of any type`` () =
        node.VarName |> should equal "Fred"
        node |> should be ofExactType<ExpressionNode<Something>>
        node.ToString() |> should equal "(Fred:Something)"

    [<Test>]
    let ``make relationship of any type`` () =
        let rel = ExpressionRel<RELATES_TO> "Fred"
        rel.VarName |> should equal "Fred"
        rel |> should be ofExactType<ExpressionRel<RELATES_TO>>
        rel.ToString() |> should equal "Fred:RELATES_TO"

    [<Test>]
    let ``Anonymous nodes aren't named``() =
        let a = N<Something>
        a |> should be ofExactType<ExpressionNode<Something>>
        a.VarName |> should equal ""
        a.ToString() |> should equal "(:Something)"

    [<Test>]
    let ``Anonymous rels aren't named``() =
        let a = R<RELATES_TO>
        a |> should be ofExactType<ExpressionRel<RELATES_TO>>
        a.VarName |> should equal ""
        a.ToString() |> should equal ":RELATES_TO"   

    [<Test>]
    let ``link two nodes simply`` () =
        let linked = node --> node2 :> IExpressionPart
        linked |> should be ofExactType<ExpressionPart>
        linked.lhs |> should equal "(Fred:Something)-->(Joe:SomethingElse)"

    [<Test>]
    let ``link two nodes via a named relationship`` () =
        let linked = node -| rel |->  node2 :> IExpressionPart
        linked |> should be ofExactType<ExpressionPart>
        linked.lhs |> should equal "(Fred:Something)-[r:RELATES_TO]->(Joe:SomethingElse)"

    [<Test>]
    let ``link two nodes via an anon relationship`` () =
        let linked = node -| R<RELATES_TO> |->  node2 :> IExpressionPart
        linked |> should be ofExactType<ExpressionPart>
        linked.lhs |> should equal "(Fred:Something)-[:RELATES_TO]->(Joe:SomethingElse)"

    [<Test>]
    let ``reverse link two nodes via an relationship `` () =
        let linked = node <-| rel |-  node2  :> IExpressionPart
        linked |> should be ofExactType<ExpressionPart>
        linked.lhs |> should equal "(Fred:Something)<-[r:RELATES_TO]-(Joe:SomethingElse)"

    [<Test>]
    let ``name untyped relationship`` () =
        let untypedRel = ExpressionRel<ANY> "Georgia"
        let linked = node -| untypedRel |->  node2  :> IExpressionPart
        linked.lhs |> should equal "(Fred:Something)-[Georgia]->(Joe:SomethingElse)"

    [<Test>]
    let ``name untyped node`` () =
        let untypedNode = ExpressionNode<ANY> "Larry"
        let linked = untypedNode -| rel |->  node2  :> IExpressionPart
        linked.lhs |> should equal "(Larry)-[r:RELATES_TO]->(Joe:SomethingElse)"

    [<Test>]
    let ``on-link multiple nodes `` () =
        let linked = ( node -| rel |-> node2 ) --> N<Something> <-| R<RELATES_TO> |- N<SomethingElse>  :> IExpressionPart
        // ----------^-----------------------^ 
        // Note the precedence of --> over |-> requiring brackets :(
        linked |> should be ofExactType<ExpressionPart>
        linked.lhs |> should equal "(Fred:Something)-[r:RELATES_TO]->(Joe:SomethingElse)-->(:Something)<-[:RELATES_TO]-(:SomethingElse)"

module Matches =
    [<Test>]
    let ``one node by name``() =
        let x = query.Match(node)
        x.Query.QueryText |> should equal "MATCH (Fred:Something)"

    [<Test>]
    let ``anon node``() =
        let x = query.Match(N<Something>)
        x.Query.QueryText |> should equal "MATCH (:Something)"

    [<Test>]
    let ``expression combined``() =
        let expr = node -| rel |-> node2
        let x = query.Match(expr)
        x.Query.QueryText |> should equal ("MATCH " + (expr :> IExpressionPart).lhs)

module Returns =
    [<Test>]
    let ``one node by name``() =
        let x = query.Return(node)
        x.Query.QueryText |> should equal "RETURN Fred"
        x.Query.ResultMode |> should equal Cypher.CypherResultMode.Set

    [<Test>]
    let ``one rel by name``() =
        let x = query.Return(rel)
        x.Query.QueryText |> should equal "RETURN type(r)"
        x.Query.ResultMode |> should equal Cypher.CypherResultMode.Set

    [<Test>]
    let ``multiple nodes``() =
        let x = query.Return(node,node2)
        x.Query.QueryText |> should equal "RETURN Fred AS Item1, Joe AS Item2"
        x.Query.ResultMode |> should equal Cypher.CypherResultMode.Projection
        
    [<Test>]
    let ``both nodes and rels``() =
        let x = query.Return(node,rel)
        x.Query.QueryText |> should equal "RETURN Fred AS Item1, type(r) AS Item2"
        x.Query.ResultMode |> should equal Cypher.CypherResultMode.Projection


    [<Test>]
    let ``anon node``() =
        (fun () -> N<Something>) >>  query.Return >> ignore
        |> should throw typeof<Exception>
        