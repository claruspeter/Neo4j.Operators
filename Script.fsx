// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"build"
#r "Neo4j.Operators"
#r "Neo4jClient"

open System
open Neo4jClient
open Neo4j.Operators

[<Literal>]
let connectionstring = @"http://localhost:7474/db/data"
[<Literal>]
let user = "neo4j" 
[<Literal>]
let pwd = "password"
let db = 
    let db = new Neo4jClient.GraphClient(Uri(connectionstring),user ,pwd)
    db.Connect()
    db

[<CLIMutable>]
type Person = { name: string; born: int;}
[<CLIMutable>]
type Movie = { name: string; released: int; }
[<CLIMutable>]
type ACTED_IN = {a:string}
[<CLIMutable>]
type DIRECTED = {b:string}


let writer = ExpressionNode<Person>.Init "writer" 
let director = ExpressionNode<Person>.Init "director" 
let movie = ExpressionNode<Movie>.Init "m" 
let wrote = ExpressionRel<ACTED_IN>.Init "w" 
let directed = ExpressionRel<DIRECTED>.Init "d" 

db.Cypher
    .Match(  writer -| wrote |-> initNode<Movie> "movie" <-| directed |- writer  )   
    .Return( writer )
    .Limit(Nullable<int>(20))
    .Results
    |> Seq.toList