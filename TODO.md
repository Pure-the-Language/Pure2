## TODO

(CONSIDER PUTTING THEM ALL INTO ADO; PENDING GATHERING OLD GITHUB ISSUES)
(Also search "TODO" in all files)

The current state of Pure is very OK to be used for one-liners and quick REPL commands. The two frontends (one REPL/CLI and the other Notebook) are generally very stable right now (as of 2023 late summer to Dec).

- [ ] (Architectural) The REPL interpreter (along with the core engine) aka. Pure.exe cannot parse statements that span multiple lines, this makes stuff like defining functions or classes very inconvinient if not at all impossible - this is not an issue when using Pure to execute script files, but is a hassle when using the REPL. The Pure.exe program itself apparently needs work, along with tne engine code - as a full re-write/replacement, we are yet to integrate BaseRepl from CSharpRepl and implement codename Aurora, though we must be careful to make sure the runtime and startup speed of Pure is acceptible, because at the moment the startup time of Pure is much slower than Python, Elixir or other cli programs. By all means, due to lack of development resources, we might not bother more advanced REPL because it's not worth it and Pure is very efficient without it for majority of use cases.
- [ ] (Architectural) Importing and consuming Nuget packages is functional but not fully streamlined/safe yet - at the moment it's stable if we consume individual packages, but there is no built-in mechanism to safe-guard against potential dependency issues.
- [ ] (Architectural) See issues on Github, e.g. https://github.com/Pure-the-Language/Pure/issues/24

- [ ] (Core)(Pipeline Library) Provide Utilities.Run that streamlines running command and gets output as string.
- [ ] (Core) Enhance "Arguments" with all kinds of command line argument utilities like Elixir and how we usually use it.
- [ ] (Core) Provide optional toggle to Import to not use Static, default we do use Static.
- [ ] Currently `Help` is not showing extension methods.

- [ ] (Documentation) Create basic usage YouTube demo
- [ ] (Documentation) Create YouTube usage tutorial for available Pure libraries
- [ ] (Documentation) Create very basic YouTube tutorials: Intro, CLI usage, Notebook usage, embedding (using Core to consume scripts), and existing library usage. Pure-The-Language YouTube channel.
- [ ] (Documentation) Train custom OpenAI model helping new user decide which custom library or existing snippet can help solve specific problem.

- [ ] (Custom Libraries) Pending consolidation: Pure 2 will NOT offer any custom libraries - all its library offerings are based on C# Nuget or Parcel eco system.
- [ ] (Custom Libraries) ScottPlot released version 5, we need to update our reference. This will be a breaking change to users because API is different - if they referenced ScottPlot directly from Pure.
- [ ] (Custom Libraries, Management) Rename CentralSnippets to Snippets; This way it's much quicker to type.
- [ ] (Custom Libraries) Add/Implement those: CSV (Read Write), Excel (Read Write), DataSource (Read: CSV, Excel, ODBC), InMemoryDB, add Parcel DataGrid standalone package.
- [ ] (Custom Libraries, Data Library) Provide `Data` (standalone).
- [ ] (Custom Libraries) Preparing for deprecating `Main` by well-encapsulating custom libraries using proper class modules and utilize the abstraction of StandardLibrary, ExperimentalLibrary, etc.
- [ ] (Custom Library) (Assembly Generation) Add `Compiler` library which works like how we are dealing with ~~Nuget~~ `dotnet` right now and is able to given a list of C# files (likely written in Pure) and output a proper DLL from it. The workflow is like this: we write scripts in Pure, we use Pure to compile such scripts as Dlls (or just to quickly generate csproj projects), then we can consume those dlls in native C# code or Python.Net - without needing to reference Core. Because if we just need to parse and consume those code as script we could just reference Core. The purpose of this Compiler is to compile native C# modules without dependency on Pure. Apparently we will have to limit the source code to not contain any Pure specific functionalities. We will automatically convert `Import` to proper Nuget/csproj references, and convert `Include` to proper .cs files. I think Roslyn won't be able to compile code, so we will directly utilize `dotnet` (just like how we are treating `Import` right now). It can generate either a console project or a library project, targeting .Net 8.

- [ ] (Framework proposal) (Create a README first) Composer (or "Flow") with functional constructs: `Repeat`, `Sequence`, `Parallel`, `Condition`, and `(base class) Services` (as addons, existing ones: Query with pre and post-processing and simple template format, Stage for global state, etc. as basic nodes). Taking either actions or AtomicAction class instance; Auto-logging. (Can we utilize Aspire?)
- [ ] (Framework) Expresso

- [ ] (Notebook, GUI, Feature) Selected phrase highlight, or search-in-code-cells function. Makes refactoring variables easier.
- [ ] (Notebook) Re-implement Notebook in Godot for cross-platform support and advanced features we didn't have last time. For this we could use embedded runtime in (full C#) Godot (instead of S/C architecture).
- [ ] (Branding) Instead of calling Pure a "programming/scripting platform", we should downgrade it into simply a scripting tool, so it's more acceptable.

## Libraries

- [ ] Refactor all libraries in to Parcel Core (notice they are NOT part of Parcel Standard Libraries)