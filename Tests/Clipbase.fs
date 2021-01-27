namespace Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open ScidFuncs

[<TestClass>]
type TestClipbase () =

    [<TestMethod>]
     member this.ClipbaseClear () =
        let actual = Clipbase.Clear()
        Assert.AreEqual(0, actual)

