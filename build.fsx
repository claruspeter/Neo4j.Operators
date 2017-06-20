// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"
let packagedDir = "./packaged/"


// Filesets
let appReferences  =
    !! "/**/*.csproj"
      ++ "/**/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)
let nunitRunnerPath = "packages/NUnit.ConsoleRunner/tools/nunit3-console.exe"

Target "Test" (fun _ ->
    !! (buildDir + "Test*.dll")
    |> NUnit3 (fun p ->
        {p with OutputDir = "TestResults"; ToolPath= nunitRunnerPath;  }
    )
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
        -- "*.zip"
        |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

Target "CreatePackage" (fun _ ->
  //CopyFiles buildDir packagedDir

  NuGet (fun p ->
    {p with
      Title = "Neo4j Operators"
      Authors = ["Haumohio"]
      Project = "Neo4j.Operators"
      Description = "A collection of operators that can help write a Neo4j Cipher query using the natural syntax (or as close as possible) without resorting to too many magic strings."
      OutputPath = packagedDir
      WorkingDir = "."
      Summary = "A collection of operators that can help write a Neo4j Cipher query using the natural syntax (or as close as possible) without resorting to too many magic strings."
      Version = "0.5"
      Copyright = "UniLicence Haumohio 2017"
      //AccessKey = myAccesskey
      Publish = false
      Files = [(@"build/Neo4j.Operators.dll", Some @"lib/net45", None) ]
      Dependencies =
            ["Neo4jClient", GetPackageVersion "./packages/" "Neo4jClient"]
      }
    )
    "Neo4j.Operators.nuspec"
)
// Build order
"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Deploy"

// start build
RunTargetOrDefault "Test"
