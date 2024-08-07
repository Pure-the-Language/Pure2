# Pure - Design

Design principles:

1. Minimal change to underlying C#
2. Provide shorthands to most common tasks
3. Scope-less

Key concept: Core is scope-less, one should consider all loaded scripts are executed "in-memory" inside one big giant scope, as Pure is intended for short ad-hoc scripting. This is especially true with regard to how `Include()` is implemented at the moment - Include functions as a macro and executes the codes from specified path in "current" scope.

## 20230831 Implementation of `Include`

The expectation for `Include` is it should at least look for scripts relative to the "current" script file once. Though prior to Core 0.1.0, the concept of "script file" doesn't exist. That is, include should work absolutely relatively to script path instead of/in addition to current dir (and PUREPATH environment variable).

In the implementation https://github.com/Pure-the-Language/Pure/pull/6, such capacity is exposed as a front-end level interface.

The "scope-less" design of Pure discourages composition on the file level, and dictates that Include should be functionally equivalent to ~~`Evaluate(File.ReadAllText(Path.Combine(ScriptFolder, FileName))`~~ (now named as `Parse()`).

## 20230831 Issues Regarding Exposing Global/Top-Level Contextual Variable e.g. Something like PSScriptRoot

It might seem advisable from scripting and code composition perspective we expose some file-scope variables like __FILE__ or __NAME__ in Python or $PSScriptRoot in Powershell. In this case, it might seem advisable to have access to ScriptPath just like we have access to Arguments.

In particular, this include:

* Expose ScriptFolder/ScriptPath/ScriptName/ContextName (e.g. something like "main" in Python)

However, we try to avoid clustering namespaces and global state of Pure scripts, and deep composition is not a recommended approach for Pure (it's recommended we encapsulate shared logic into proper C# assemblies as early as possible). At the moment, the functionality of `Include` is implemented as a frontend-level implementation detail rather than a language-level-feature.

Because there is no immediate justifiable need for exposing such higher level variables, we are holding this proposal.

## 20231115 Utility of `Import()`

The current handling of Import aka. by compiling seperate DLLs are actually good in the sense that from a single Nuget identifier (aka. a single script file), it can mix and use different assemblies at will - this is especially useful when using e.g. Notebook's default file, where we may experiment with different Nuget libraries independently.

## 20231120 Keeping `Main`

It might seem clustering of namespace by automatically importing main, but for most "scripting-oriented" modules, it makes less sense to explicitly `using static <ClassName>` because `Import(Module)` is already long enough to type. Instead of discouraging Main, a more proper way is to implement each module properly for non-scripting use. So we should still keep `Main`, so for scripting-use modules it's easier.

Another improvement we can do is to actually re-implement all (standard) libraries as proper well encapsulated C# modules and NOT for scripting use, then provide meta-packages like `DataAnalytics` that provides such a `Main` implementation for automatic imports. Do notice this comes at a cost of potentially importing unused modules but should not be too much of a concern if the design of `DataAnalytics` is lean and focus on essentials.