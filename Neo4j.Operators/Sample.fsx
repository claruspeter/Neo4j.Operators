// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"../build"
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

//NOTE that you could use Neo4jTypeProvider (nuget) to create the following types directly from the data source itself.
[<CLIMutable>]
type Person = { name: string; born: int;}
[<CLIMutable>]
type Movie = { title: string; released: int; tagline: string; }  
type ACTED_IN() = class end
type DIRECTED() = class end

let stagehog = ExpressionNode<Person> "actorAndDirector" 
let movie = ExpressionNode<Movie> "theMovie" 
let r = ExpressionRel<ANY> "r"

let allTheStageHogs =
    db.Cypher
        .Match(  stagehog -| R<ACTED_IN>  |-> movie <-| r |- stagehog  )   
        .Return( r, stagehog, movie )
        .Limit(Nullable<int>(20))
        .Results
        |> Seq.toList

let moviesFromMyFavouriteYear =
    db.Cypher
        .Match( movie )
        .Where( movie @.@ "released" @= 1992 )
        .Return( movie )
        .Limit(Nullable<int>(20))
        .Results
        |> Seq.toList