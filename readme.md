# Neo4j.Operators

## Purpose 
A collection of operators that can help write a Neo4j Cipher query
using the natural syntax (or as close as possible)
without resorting to too many magic strings.

## Example

We can now create expression node variables that can be used (and compile time checked) in both the _Match_ clause and the _Return_ clause.

```` fsharp

    open Neo4j.Operators

    let stagehog = ExpressionNode<Person> "stagehog" 
    let movie = ExpressionNode<Movie> "theMovie" 

    db.Cypher
        .Match(  stagehog -| R<ACTED_IN>  |-> movie <-| R<DIRECTED> |- stagehog  )   
        .Return( stagehog, movie )
        .Results()

````

This reduces to the following equivalent query:

```` fsharp

    db.Cypher
        .Match( "(stagehog:Person)-[:ACTED_IN]->(theMovie:Movie)<-[:DIRECTED]-(stagehog:Person)" )  
        .Return( "stagehog, theMovie" )  // returned as a tuple
        .Results()

````

... and returns:

```` fsharp

    [
     ({name = "Tom Hanks"; born = 1956;},
      {title = "That Thing You Do"; released = 1996; tagline = "In every life there comes a time...";});
     ({name = "Clint Eastwood"; born = 1930;},
      {title = "Unforgiven"; released = 1992; tagline = "It's a hell of a thing, killing a man";});
     ({name = "Danny DeVito"; born = 1944;},
      {title = "Hoffa"; released = 1992; tagline = "He didn't want law. He wanted justice.";})
    ]

````

Full worked example in [Sample.fsx](Neo4j.Operators/Sample.fsx).

## Anonymous elements and Primitives

Anonymous (i.e. un-named) elements can be used in the match query using a couple of provided primitives.

* R&lt;ACTED_IN&gt; : an un-named relationship of type *ACTED_IN* => __[:ACTED_IN]__
* N&lt;Movie&gt; : an un-named node of type _Movie_ => __(:Movie)__

Named expression variables can be typed or un-typed, depending on your query needs.

* ExpressionNode&lt;Person&gt;( "p" ) : an node called "p" of type _Person_ => __(p:Person)__
* ExpressionRel&lt;DIRECTED&gt;( "r" ) : an relationship called "r" of type _DIRECTED_ => __[r:DIRECTED]__
* ExpressionRel&lt;ANY&gt;( "r" ) : an relationship called "r" of any type => __[r:]__

Note that including a relationship variable in the _Return_ clause, will actually return the type of the relationship as a string (e.g. "DIRECTED").

## Installation

### Nuget

[Install-Package Neo4j.Operators](https://www.nuget.org/packages/Neo4j.Operators)

### Build from source

The build is executed using FAKE and the paket package manager.

Windows: <code> build.cmd </code>

Linux: <code> ./build.sh </code>

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