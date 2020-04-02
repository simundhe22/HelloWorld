#addin "nuget:?package=Cake.ArgumentHelpers"
#tool "nuget:?package=OctopusTools&Version=6.7.0"
#addin "Cake.Npm"&version=0.8.0
#addin nuget:?package=Cake.SemVer
#addin nuget:?package=semver&version=2.0.4
#module "nuget:?package=Cake.BuildSystems.Module&version=0.3.2"

using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

var target= Argument("Argument","Default");
var buildoutputpath= "D:/Output_build/" ;
var octopkgpath= "D:/OctoPackages/";
var packageId = "app_2";
var semVer = CreateSemVer(1,0,0);
var sourcepath= "HelloWorld.sln";
var octopusApiKey="API-ITVXXO91J3JO5DOAQASGSO7E";

var octopusServerUrl="http://localhost/app#/Spaces-1";

Task("Restore")
    .Does(()=>
    {
      NuGetRestore("HelloWorld.sln");
      DotNetCoreRestore("HelloWorld.sln");
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => 
    {
        MSBuild(sourcepath, new MSBuildSettings()
              .WithProperty("OutDir", buildoutputpath)
                );

    });

Task("OctoPack")
	.IsDependentOn("Build")
	.Does(()=>
	{    
		var octoPackSettings = new OctopusPackSettings()
		{
			BasePath = buildoutputpath,
			OutFolder = octopkgpath,
			Overwrite = true,
			Version = semVer.ToString()
		};    

    OctoPack(packageId,octoPackSettings);
	});

Task("OctoPush")
	.IsDependentOn("OctoPack")
	.Does(()=>
	{	
       var octoPushSettings = new OctopusPushSettings()
    {        
        ReplaceExisting =true
    };
    
    OctoPush(octopusServerUrl, 
        octopusApiKey, 
        GetFiles("D:/OctoPackages/*.*"),
        octoPushSettings);
	});
 
RunTarget(target);
