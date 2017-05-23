// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake

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
      Project = "Neo4jOperators"
      Description = "sfdafdasdf"
      OutputPath = packagedDir
      WorkingDir = "."
      Summary = "asdfasdf"
      Version = "0.4"
      Copyright = "UniLicence Haumohio 2017"
      //AccessKey = myAccesskey
      Publish = false
      Files = [(@"build/Neo4j.Operators.dll", Some @"lib/net45", None) ]
      // DependenciesByFramework =
      //   [{
      //     FrameworkVersion  = "net45"
      //     Dependencies =
      //       ["FSharp.TypeProviders.StarterPack", GetPackageVersion "./packages/" "FSharp.TypeProviders.StarterPack"]
      //   }]
      }
    )
    "Neo4j.Operators.nuspec"
)
// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
