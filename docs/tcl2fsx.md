# Converting TCL to FSX

This article shows how a sample TCL script can be converted to an F# version. 

If you follow this article you will see some examples of how to use the provided API functions.

The full sample scipts can be found here:

- TCL script [ImportWhite.tcl](https://github.com/pbbwfc/ScidNet/blob/main/scripts/importWhite.tcl)
- F# script [ImportWhite.fsx](https://github.com/pbbwfc/ScidNet/blob/main/scripts/importWhite.fsx)

## Usage

For the TCL script it needs to be run using the SCID interpreter: **tcscid.exe**. 

If you navigate to the location of the script then you can run it in a command prompt using:

```
"D:\Scid vs PC-4.21\bin\tcscid" ImportWhite.tcl
```

For the F# script, this is built to depend on .NET Core. You therefore need to use F# interactive for this rather tha the .NET Standard version. The easiest way to do this is to use Visual Studio Code with the Ionide Extension.

You can also run it on the command line, like TCL, but using:

```
dotnet fsi ImportWhite.fsx
```
To use the .NET version of the API in the F# script you need to reference **ScidFuncs.dll** and open the namespace **ScidFuncs** at the start of the script. We are also going to use file operations and so also need to open **System.IO**. Thus we use:

```fsharp
#r @"D:\GitHub\ScincNet\debug\bin\ScidFuncs.dll"
open ScidFuncs
open System.IO
```

## Step 1 - Create the Database

In TCL, we first create the database. We then open the database and print an error message if this fails. We then check that it is not read only. We then get the number of games in the database (which should be 0) and then print a message to the console.

This is the code:

```tcl
set basename D:/tmp/WhiteTCL
sc_base create $basename

if {[catch {sc_base open $basename} result]} {
    puts stderr "Error opening database \"$basename\": $result"
    exit 1
}
if {[sc_base isReadOnly]} {
    puts stderr "Error: database \"$basename\" is read-only."
    exit 1
}
set num [sc_base numGames] 

puts "number of games: $num"
```

This should produce this output:

```console
number of games: 0
```

The correspondong F# code is:

```fsharp
let basename = @"D:/tmp/WhiteFSX"
Base.Create(basename)|>ignore

if (Base.Open(basename)<0) then
    printfn "Error opening database %s" basename

if (Base.Isreadonly()) then
    printfn "Error database %s is read only" basename

let num = Base.NumGames()

printfn "number of games: %i" num
```

This produces this output:

```console
number of games: 0
```

## Step 2 - Import PGN files

In TCL, we now create a set a folder that holds the PGN files. We then create a list of the pgn file names. We then loop through these names doing the following:
- create a full nam by including the folder
- import the file
- produce an error message if it fails
- get the number of games imported
- get the warnings generated
- produce a message of the number of games imported
- produce a message on the warnings
Finally close the database.

This is the code:

```tcl
set fol "D:/tmp/"
set pgns [list "Benko.pgn" "Benoni.pgn" "Budapest.pgn" "Dutch.pgn" "Grunfeld.pgn" "KingsIndian.pgn" "OldIndian.pgn" "QGA.pgn" "QGDmain.pgn" "QGDtarr.pgn" "QGDtri.pgn" "QGDunus.pgn" "Slav.pgn"]
foreach pgn $pgns  {
    set pgnfile [file join $fol $pgn]

    if {[catch {sc_base import file $pgnfile} result]} {
        puts stderr "Error importing \"$pgnfile\": $result"
        exit 1
    }
    set numImported [lindex $result 0]
    set warnings [lindex $result 1]
    puts "Imported $numImported games from $pgnfile"
    if {$warnings == ""} {
        puts "There were no PGN errors or warnings."
    } else {
        puts "PGN errors/warnings:"
        puts $warnings
    }
}

sc_base close
```

This should produce this output:

```console
Imported 1 games from D:/tmp/Benko.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Benoni.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Budapest.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Dutch.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Grunfeld.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/KingsIndian.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/OldIndian.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGA.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGDmain.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGDtarr.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGDtri.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGDunus.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Slav.pgn
There were no PGN errors or warnings.
```
The correspondong F# code is:

```fsharp
let fol = "D:/tmp/"
let pgns = ["Benko.pgn"; "Benoni.pgn"; "Budapest.pgn"; "Dutch.pgn"; "Grunfeld.pgn"; "KingsIndian.pgn"; "OldIndian.pgn"; "QGA.pgn"; "QGDmain.pgn"; "QGDtarr.pgn"; "QGDtri.pgn"; "QGDunus.pgn"; "Slav.pgn"]

let doimp pgn =
    let pgnfile = Path.Combine(fol,pgn)
    let mutable num = 0
    let mutable msgs = ""
    if (Base.Import(&num,&msgs,pgnfile)<>0) then
        printfn "Error importing: %s" pgnfile
    printfn "Imported %i games from %s" num pgnfile
    if msgs="" then
        printfn "There were no PGN errors or warnings."
    else
        printfn "PGN errors/warnings:"
        printfn "%s" msgs

pgns|>List.iter doimp

Base.Close()|>ignore
```

This produces this output:

```console
Imported 1 games from D:/tmp/Benko.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Benoni.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Budapest.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Dutch.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Grunfeld.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/KingsIndian.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/OldIndian.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGA.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGDmain.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGDtarr.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGDtri.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/QGDunus.pgn
There were no PGN errors or warnings.
Imported 1 games from D:/tmp/Slav.pgn
There were no PGN errors or warnings.
```

## Step 3 - Classify the games

In TCL, we now import a file of ECO codes. If this fails produce an error message. Now open the database again and check that it is not read only. Now classify the games generating messages. Finally, close the database.

This is the code:

```tcl
set ecofile "D:/tmp/scid.eco"
if {[catch {sc_eco read $ecofile} result]} {
    puts stderr "Error reading ECO file: $result"
    exit 1
}

# Open the database:
if {[catch {sc_base open $basename} result]} {
    puts stderr "Error opening database \"$basename\": $result"
    exit 1
}
if {[sc_base isReadOnly]} {
    puts stderr "Error: database \"$basename\" is read-only."
    exit 1
}

puts "Classifying games..."
puts [sc_eco base 1 1]
sc_base close
```

This should produce this output:

```console
Classifying games...
Classified 13 games in 0.00 seconds
```
The correspondong F# code is:

```fsharp
let ecofile = "D:/tmp/scid.eco"
if Eco.Read(ecofile)<>0 then
    printfn "Error reading ECO file"

if (Base.Open(basename)<0) then
    printfn "Error opening database %s" basename

if (Base.Isreadonly()) then
    printfn "Error database %s is read only" basename

let mutable msgs = ""
Eco.Base(&msgs)|>ignore
printfn "Classifying games...\n%s" msgs

Base.Close()|>ignore
```

This produces this output:

```console
Classifying games...
Classified 13 games in 0.00 seconds
```

## Step 4 - Remove comments from the games

In TCL, we now open the database again and check that it is not read only. Now loop through each game removing the comments. Finally, close the database.

This is the code:

```tcl
if {[catch {sc_base open $basename} result]} {
    puts stderr "Error opening database \"$basename\": $result"
    exit 1
}
if {[sc_base isReadOnly]} {
    puts stderr "Error: database \"$basename\" is read-only."
    exit 1
}

for {set i 1} {$i <= [sc_base numGames]} {incr i} {
    if {[catch { sc_game load $i }]} {
        puts "Error: could not load game number $i"
        exit 1
    }
    sc_game strip comments

    sc_game save $i
}
sc_base close
```

This produces no output.

The correspondong F# code is:

```fsharp
if (Base.Open(basename)<0) then
    printfn "Error opening database %s" basename

if (Base.Isreadonly()) then
    printfn "Error database %s is read only" basename

for i=1 to Base.NumGames() do
    if ScidGame.Load(uint(i))<>0 then
        printfn "Error: could not load game number %i" i
    ScidGame.StripComments()|>ignore
    ScidGame.Save(uint(i))|>ignore

Base.Close()|>ignore
```

## Step 5 - Change the white names in the games to the names of the opening

In TCL, we create a list of names to use. We now open the database again and check that it is not read only. Now loop through each game changing the white name and then saving it. Finally, close the database.

This is the code:

```tcl
set nms [list "Benko" "Benoni" "Budapest" "Dutch" "Grunfeld" "Kings Indian" "Old Indian" "QGA" "QGD main" "QGD tarrasch" "QGD triangle" "QGD unusual" "Slav"]

# Open the database:
if {[catch {sc_base open $basename} result]} {
    puts stderr "Error opening database \"$basename\": $result"
    exit 1
}
if {[sc_base isReadOnly]} {
    puts stderr "Error: database \"$basename\" is read-only."
    exit 1
}

for {set i 1} {$i <= [sc_base numGames]} {incr i} {
    if {[catch { sc_game load $i }]} {
        puts "Error: could not load game number $i"
        exit 1
    }
    set nm [lindex $nms [expr $i-1]]
    sc_game tags set -white $nm

    sc_game save $i
}
sc_base close
```

This produces no output.

The correspondong F# code is:

```fsharp
let nms  =["Benko"; "Benoni"; "Budapest"; "Dutch"; "Grunfeld"; "Kings Indian"; "Old Indian"; "QGA"; "QGD main"; "QGD tarrasch"; "QGD triangle"; "QGD unusual"; "Slav"]

// Open the database:
if (Base.Open(basename)<0) then
    printfn "Error opening database %s" basename

if (Base.Isreadonly()) then
    printfn "Error database %s is read only" basename

let setwht i vl =
    if ScidGame.Load(uint(i+1))<>0 then
        printfn "Error: could not load game number %i" (i+1)
    ScidGame.SetTag("White",vl)|>ignore
    ScidGame.Save(uint(i+1))|>ignore

nms|>List.iteri setwht

Base.Close()|>ignore
```