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
type Movie = { title: string; released: int; tagline: string; }


type ACTED_IN() = class end

type DIRECTED() = class end

let stagehog = NodeVar<Person> "actorAndDirector" 
let movie = NodeVar<Movie> "theMOvie" 

db.Cypher
    .Match(  stagehog -| R<ACTED_IN>  |-> movie <-| R<DIRECTED> |- stagehog  )   
    .Return( stagehog, movie )
    .Limit(Nullable<int>(20))
    .Results
    |> Seq.toList