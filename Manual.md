# C# Programming in Pure <!--Originally: Pure The Scripting Platform-->

Author: Charles Zhang
Year: 2024

<!-- This will be the main book for Pure usage; Consider it as a user guide; This shall be written independent from the rest of Pure solution documentations. -->
<!-- As a proper textbook, consider expanding more on C# language itself -->

> This book is for those great minds that designed C#.

## Foreword

(Pending someone who uses Pure and finds it useful, or even loves it)

## Table of Contents

* Foreword
* Introduction
* Chapter 1: Setup, Installation & Compile from Source
    * 1.1 Instal .Net SDK
        * 1.1.1 Windows
        * 1.1.2 Linux
        * 1.1.3 Mac
    * 1.2 Install Pure
    * 1.3 Write "Hello World!" Program
* Chapter 2: Basics
* Chapter 4: REPL
* Chapter 5: Standard Libraries
* Chapter 6: The Parcel NExT Ecosystem
* Chapter 7: Application Development
    * 7.1 Code Management and Include
    * 7.2 CLI Automation
    * 7.3 Parcel Packages
    * 7.4 Parcel Frameworks
* Chapter 8: Sharing and Export
    * 8.1 Snippet Sharing
    * 8.2 Export Markdown
    * 8.3 Publish .exe
    * (8.4 Convert to (data embedded) Parcel Graph)
* Chapter 9: Best Practices
* Conclusion

## Introduction

> Most common tasks should take no more than 3 lines of code (within 80 characters line length limit).

### Overview

Pure is a scripting tool that exploits the runtime compilation features of latest .Net ecosystem and puts C# to the frontier of scripting use. C# is a strongly typed programming language and requires a semi-colon to end a statement, its variable and function names are case sensitive, which all make a well-written piece of code easy to read. Pure offers utilities that skips the "compilation" step and allows execution directly from a script file. What's more, Pure simplifies certain syntax and usages of the typical C# slightly - but those simple simplifications makes using C# on a daily basis for really small tasks way easier.

This is a very practical guide on how to use Pure, it also serves as a manual to the tool. Pure itself is pretty straightforward, so this books talks more about what's going on "behind the scene".  
It's not a reference book for the C# language, though we will cover certain language concepts when relevant.

Who should use Pure:

* Programming Beginner
* C# Veterans
* Those looking for alternative to Python for automation.

### History

C# has always supported some form of scripting since .Net core (pending fact check), but before Pure, it's fairly inconvenient to setup and use.

It's natural for anyone who loves the language to one day seek interpret it and even write shorter programs for everyday small tasks (as typical un sxripting scenarios). When I first started, I found CSScropt, latter there is XXXX - byt CSScropt.is using very old technology and none of those are modern and lightweight enough to be easy to use. What's more, embedding those in custom applications is even tricker. On the other hand, existing solutions like [.Net Interactive](https://github.com/dotnet/interactive) targets runtime integration (embedding) but not command line use, and is very bulky.

### Who is This Book For?

If you are a Methodox employee, a technical intern working with C# codebase or a Parcel NExT developer, then you should read this book as well for it provides useful concrete information on Pure.

If you want a comprehensive guide on C#, I have benefited greatly from the textbooks by Andrew Troelsen, Phil Japikse and Christian Nage. If you are looking for an even simpler and more efficient programming platform than Pure for most tasks, consider Programming with Parcel written by myself.

### Who Is This Book Not For?

Pure is designed to work with "straightforward scripts" that doesn't have complicated dependencies, this ranges from a few dozen to a few hundred lines of code. If your goal is to develop large scale software, then Pure is not for you. Even though, Pure can still be useful along with way for experimenting with specific libraries and prototyping. You can still start a large project with Pure - and latter easily transform it into a C# project.

## Chapter 1: Setup, Installation & Compile from Source

### 1.1 Install .Net SDK

### 1.2 Install Pure

### 1.3 Write "Hello World!" Program

Create a text file and put the following inside, let's call the file `Hello.cs`:

```c#
WriteLine("Hello World!");
```

Summon a command line and do the following: 

```bash
pure Hello.cs
```

You should see this in the command prompt:

```
> Hello World!
```

Now do this:

```bash
pure publish Hello.cs
```

You should see an `.exe` file along size your script file. Try execute it, and you should get the same result as in the command prompt.

(Screenshot)

Congratulations! Now you've written your first C# script and compiled a native program!

## Chapter 2 - Basics

Basic Usage: 

* Download [BaseRepl](https://github.com/Pure-the-Language/BaseRepl) to get familiar with CSharp syntax and REPL with C#.
* Play with Pure just like BaseRepl (Releases are generally available on [Github](https://github.com/Pure-the-Language/Pure/releases) or [Itch.io](https://charles-zhang.itch.io/pure)).

### Declare Variables

You can declare variables using keyworf `var`. Variables must be declared and initialized before it can be used. There is no need for semicolons when declare variables.

```C#
var a = 5 // Good
b = 7 // Bad: b is not defined
```

### Import Libraries

Use `Import(<Library Name>)` to import libraries. Libraries should be available under PATH as .Net 8 (or .Net Standard) DLL files.

A library is a collection of C# functionalities. ~~It can optionally expose a static Main class, the methods of which will become available at global scope upon import~~ (deprecating, consider using `using static` explicitly instead).

### 1D Vector Numerics

Pure supports simple vector numerics through a library. See Vector library for more details. Below is some basic example:

```c#
// Define vectors using values or strings
var a = Vector(1, 2, 3)
var b = Vector("2 3 4")

// Vectors support arithmetics using operators
a + b
a * 5
```

## Chapter 4 - REPL

As a scripting tool, REPL and scripts are two primary use of Pure. REPL refers to read–eval–print loop, it's done by interactively executing Pure expressions with the interpreter terminal Pure.

### Get Information (NOT IMPLEMENTED)

At any time during REPL (read–eval–print loop), use `Help(<name>)` to get information on variables, types, namespaces, and libraries.

### Save Session

After you've done some REPL exercise, you can output you inputs in this session by using the `Save(<File Path>)` command. After it's saved, you can modify and clean up the saved history of commands for proper script re-use.

## Chapter 5 - Standard Libraries

~~A few standard libraries are provided as light wrappers of some conventional functionalities as encountered per author's experience and work needs.~~ (If it's really as lightweight or trivial like this, consider exposing them as snippets ~~or macros~~)

### ODBC

```c#
DSN = "Some DSN";
// Create a type to use as return result
public record Result(string Name, double Value);
Select<Result>("""
SELECT
    Name,
    Value
FROM MyValues
"""); // Returns an array
```

## Chapter 7 - Proper Usage Tipcs -> Best Practice

Pure is designed for quick one-shot scripts that are short and functional. As a rule of thumb, it's intended for things that do not exceed a few hundred lines - assuming proper code management is already implemented.

As scripts grows and code is refactored for proper management, one might use `Include` for simple code management; But as code grows in complexity, either one of the two must be done for more reasonable code management:

1. Refactor shared code into proper C# DLL as library;
2. Refactor code project into a proper standalone C# solution.

New: "Develop in Visual Studio, Run in Pure" methodology. For easier debugging and styling for beginners.

### Scale Up

As code logic becomes more intricate, without a proper debugger (which is planned as a VS Code extension), it's slow to debug compared to traditional compiled languages. In this case, one can utilize the power of `dotnet` CLI utility and Visual Studio to enable quick development.

(In short):

* `dotnet new console`
* `dotnet run <project-name.csproj>`

This creates a minimum 2-file template for any single file script. Syntax of certain things might need tweaking and referencing Pure libraries need tweaking as well (we already have documentation on this, pending merging).

## Conclusion

Pure is magical for those who already knew programming, and it's also so for those new to programming.