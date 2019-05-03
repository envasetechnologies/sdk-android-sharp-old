# Instructions on how to create & update the Photector Android SDK bindings

## Introduction

Creating the Android SDK bindings is sadly a far from trivial process, therefore this guide is included with instructions on how the original bindings were created as well as suggestions on how to maintain and update the bindings in the future.

First of all, there are some very important points to keep in mind when creating bindings:
- Use the same versions of dependencies as used in the original bindings
- All depencies of Photector need to be included somehow as well. For the native Photector SDK, this is easy, Gradle takes care of downloading the dependencies when Photector is included in a project, but for C# we don't have Gradle.
- Because of the previous point, we should create bindings for each separate dependency.
- We have to keep in mind only 1 single AAR file can be included in _any_ binding project.
- Because of the previous point, we need to create multiple binding projects, since at least Fotoapparat, Photector and Amazon S3 are distributes as AAR files.
- Multidex might be required, since a huge amount of methods, classes and other symbols are generated for all the bindings. Not having Multidex enabled might cause errors.
- Fotoapparat makes use of modern Java 8 functionality, therefore we need to make sure Desugaring is enabled. The project of desugaring means removing the synctatic sugar of modern language features from methods, variables, etc... in order for the code to be compatible with older Java versions.
- JARs should be distributed as EmbedderJars inside a binding as much as possible, though sometimes we need to use EmbeddedReferenceJar if the JAR is already included in some other way, perhaps as part of another dependency.
- We're not interested in generating C# APIs for all dependencies, so we skip generating C# methods for included classes by modifying a bindings' Metadata.xml file.
- The project might need some manual editing in a text file to enable or disable certain functionality that cannot otherwise be enabled or disabled (e.g. d8 dexer, r8 linker, desugaring, etc...).
- There might be other, unforseen issues one could encounter when generating bindings, though I hope most potential issues will be captured by this document.

## Getting the Gradle dependencies

In order to get the Gradle dependencies, one needs to just sync Gradle from the native Photector Android SDK project. The dependencies will be downloaded into `~/.gradle/caches/modules-2/files-2.1`. 

Please note that many dependencies themselves require other dependencies. To figure out which dependencies are required, try to find the appropriate `gradle.build` file, e.g. for Amazon all the gradle build files are visible on Github. You'd need to sync the project to download the dependencies of course.

## Creating a binding for a Java Archive (JAR)

To create a binding for a Java archive, just create a new binding project and add the JAR to the `Jars` directory and set the build action to `EmbeddedJar` or `EmbeddedReferenceJar`. Also update `Metadata.xml` to remove all nodes, e.g. for Amazon AWS dependencies we would add the following to `Metadata.xml`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<metadata>
	...
    <remove-node path="/api/package[starts-with(@name, 'com.amazonaws')]" />
    ...
</metadata>
```

_**PLEASE NOTE**: Rarely you'd want to set the build action for a JAR to `EmbeddedReferenceJar`, but it's might be required in a few cases, for example we use EmbeddedReferenceJar for `org.apache.http.legacy.jar`, since this JAR is already included on each device._

## Creating a binding for an Android Archive (AAR)

To create a binding for a Java archive, just create a new binding project and add the AAR to the `Jars` directory and set the build action to `LibraryProjectZip`. Optionally include additional dependency JARs with build action set to `EmbeddedJar` or `EmbeddedReferenceJar`, though you'd not want to do this is these dependency JARs are used by other binding projects as well - in that case create a separate binding project for the JAR.

Make sure to update `Metadata.xml` to remove all nodes, e.g. for Amazon AWS dependencies we would add the following to `Metadata.xml`:

```
<?xml version="1.0" encoding="UTF-8"?>
<metadata>
	...
    <remove-node path="/api/package[starts-with(@name, 'com.amazonaws')]" />
    ...
</metadata>
```

_**PLEASE NOTE**: Rarely you'd want to set the build action for a JAR to `EmbeddedReferenceJar`, but it's might be required in a few cases, for example we use EmbeddedReferenceJar for `org.apache.http.legacy.jar`, since this JAR is already included on each device._

## Configuring a binding or test project

Some project configuration isn't possible through Visual Studio's UI, so for these purposes we need to open a project file in a text editor. The following settings might need to be added for a binding project to function correctly:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    ...
    <!-- The following properties might need to be added or changed manually -->
    <AndroidEnableProguard>False</AndroidEnableProguard>
    <AndroidEnableMultiDex>True</AndroidEnableMultiDex>
    <AndroidEnableDesugar>True</AndroidEnableDesugar>
    <!-- We usually don't want to enable d8 and r8 -->
    <!--AndroidDexTool>d8</AndroidDexTool-->
    <!--AndroidLinkTool>r8</AndroidLinkTool-->
  </PropertyGroup>
  ...
 </Project>
```

To give one example, we encountered the error `R8 : error : Library class org.apache.http.conn.scheme.LayeredSocketFactory implements program class org.apache.http.conn.scheme.SocketFactory`. Disabling the R8 linker caused the issue to disappear.

For the `Fotoapparat` dependency `AndroidEnableDesugar` must be set to `True`, since Fotoapparat makes use of modern Java language features that need to be desugared to be compatible with older Java SDKs.

For the `Demo` test project `AndroidEnableMultiDex` must be set to true, since there are too many types, methods, etc... that need to be included.

`AndroidEnableProguard` can probably usually be set to `False`, though one might want to set it to true for code obfusciation or other purposes. 

## Binding projects don't need references to their dependencies

It's better to not reference the dependency project. Actually, if multiple projects refer to the same dependency an error will occur regarding cyclical inclusion of a dependency, so avoid setting references to dependencies.

## Setting the package name for a dependency

In order to change the package name as used in C#, add the following line to `Metadata.xml`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<metadata>
	...
    <attr path="/api/package[@name='com.peir.photector']" name="managedName">PhotectorSharp</attr>
    ...
</metadata>
```

In the above example the Android package name com.peir.photector is changed into PhotectorSharp for the binding project.

## App projects need to include both the Photector library as well as all of it's dependencies

At the moment a consumer of the PhotectorSharp binding need to include both PhotectorSharp as well as all it's dependencies, though perhaps in the future this could be done automatically by creating a Nuget of both PhotectorSharp as well as it's dependencies or perhaps create a project that includes all DLLs.