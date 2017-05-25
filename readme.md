# Neo4j.Operators

## Purpose 
A collection of operators that can help write a Neo4j Cipher query
using the natural syntax (or as close as possible)
without resorting to too many magic strings.

## Example

[from Script.fsx](Script.fsx)

```` fsharp

    db.Cypher
        .Match( "(stagehog:Person)-[:ACTED_IN]->(:Movie)<-[:DIRECTED]-(stagehog)" )  
        .Return( "stagehog" )
        .Results

````
-- The original query using a "SQL"-like string for the Cipher match clause, 
and another string for the return variable that can't be compile-time checked.

```` fsharp

    let stagehog = ExpressionNode<Person>.Init "actorAndDirector" 

    db.Cypher
        .Match(  stagehog -| R<ACTED_IN>  |-> N<Movie> <-| R<DIRECTED> |- stagehog  )   
        .Return( stagehog )
        .Results

````
-- Current state of the operators that allow a _similar looking_ query,
that is compile-time checkable, including the return variable.



# Licence

This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>