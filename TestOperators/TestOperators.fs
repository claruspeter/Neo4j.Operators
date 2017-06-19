module TestOperators

open NUnit.Framework
open FsUnit
open Neo4jClient
open Neo4j.Operators

type Something() = class end
type SomethingElse() = class end
type RELATES_TO() = class end

let node = ExpressionNode<Something> "Fred"
let node2 = ExpressionNode<SomethingElse> "Joe"
let rel = ExpressionRel<RELATES_TO> "r"

[<Test>]
let ``Can make node of any type`` () =
    node.VarName |> should equal "Fred"
    node |> should be ofExactType<ExpressionNode<Something>>
    node.ToString() |> should equal "(Fred:Something)"

[<Test>]
let ``Can make relationship of any type`` () =
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
let ``Can link two nodes simply`` () =
    let linked = node --> node2 :> IExpressionPart
    linked |> should be ofExactType<ExpressionPart>
    linked.lhs |> should equal "(Fred:Something)-->(Joe:SomethingElse)"

[<Test>]
let ``Can link two nodes via a named relationship`` () =
    let linked = node -| rel |->  node2 :> IExpressionPart
    linked |> should be ofExactType<ExpressionPart>
    linked.lhs |> should equal "(Fred:Something)-[r:RELATES_TO]->(Joe:SomethingElse)"

[<Test>]
let ``Can link two nodes via an anon relationship`` () =
    let linked = node -| R<RELATES_TO> |->  node2 :> IExpressionPart
    linked |> should be ofExactType<ExpressionPart>
    linked.lhs |> should equal "(Fred:Something)-[:RELATES_TO]->(Joe:SomethingElse)"

[<Test>]
let ``Can reverse link two nodes via an relationship `` () =
    let linked = node <-| rel |-  node2  :> IExpressionPart
    linked |> should be ofExactType<ExpressionPart>
    linked.lhs |> should equal "(Fred:Something)<-[r:RELATES_TO]-(Joe:SomethingElse)"

[<Test>]
let ``Can name untyped relationship`` () =
    let untypedRel = ExpressionRel<ANY> "Georgia"
    let linked = node -| untypedRel |->  node2  :> IExpressionPart
    linked.lhs |> should equal "(Fred:Something)-[Georgia]->(Joe:SomethingElse)"

[<Test>]
let ``Can name untyped node`` () =
    let untypedNode = ExpressionNode<ANY> "Larry"
    let linked = untypedNode -| rel |->  node2  :> IExpressionPart
    linked.lhs |> should equal "(Larry)-[r:RELATES_TO]->(Joe:SomethingElse)"

[<Test>]
let ``Can on-link multiple nodes `` () =
    let linked = ( node -| rel |-> node2 ) --> N<Something> <-| R<RELATES_TO> |- N<SomethingElse>  :> IExpressionPart
    // ----------^-----------------------^ 
    // Note the precedence of --> over |-> requiring brackets :(
    linked |> should be ofExactType<ExpressionPart>
    linked.lhs |> should equal "(Fred:Something)-[r:RELATES_TO]->(Joe:SomethingElse)-->(:Something)<-[:RELATES_TO]-(:SomethingElse)"