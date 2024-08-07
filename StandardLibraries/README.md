# ~~Standard Libraries~~ --> Custom Libraries

(Pending consolidation: Pure 2 will NOT offer any custom libraries - all its library offerings are based on C# Nuget or Parcel eco system. We may however expose certain things through snippets/macros - pending differentiation/definition)

> `Main` should be provided in a namespace the same as library name. This way `Help(LibraryName.Main)` makes sense, and the same `Main` is not conflicting with other libraries.

Lean wrappers and thin dependencies, scripting-oriented. Moving forward we generally do not wish to implement a `Main` in new libraries and rely on meta-packages to expose/import interfaces.

```mermaid
flowchart TD
    CorePackage
    StandardLibrary
    ExperimentalLibrary
    Meta-Package
    Framework

    CorePackage --> StandardLibrary
    StandardLibrary --> ExperimentalLibrary
    StandardLibrary --> Meta-Package
    StandardLibrary --> Framework
```

* Standard Library: Base class for all libraries to inherit from.
* Experimental Library: Semantical emphasis on the experimental status of the library.
* Meta-Package: Meta-packages are implemented as proper C# dlls but can have instructions that's running on scripting scope.
* Frameworks: Intended for complete problem-solving solutions as is.

## Naming Convention

Do not name a class the same as its namespace. As such, for standard libraries, we name the main type the same as module name, but for namespaces, let's denote by its (functional) category.