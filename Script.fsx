// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"build"
#r "Neo4j.Operators"
#r "Neo4jClient"

using System
using Neo4jClient
using Neo4j.Operators

[<Literal>]
let connectionstring = @"http://localhost:7474/db/data"
[<Literal>]
let user = "neo4j" 
[<Literal>]
let pwd = "password"
let db = new Neo4jClient.GraphClient(Uri(connectionstring),user ,pwd)
