﻿namespace Rezoom.ORM.Test
open Rezoom.ORM
open Microsoft.VisualStudio.TestTools.UnitTesting

type User = 
    {
        UserId : int
        Name : string
    }

type Folder =
    {
        FolderId : int
        Children : Folder array
    }

[<TestClass>]
type TestCompositeReaders() =
    [<TestMethod>]
    member __.TestReadUser() =
        let colMap =
            [|
                "UserId", ColumnType.Int32
                "Name", ColumnType.String
            |] |> ColumnMap.Parse
        let row = ObjectRow(1, "jim")
        let reader = ReaderTemplate<User>.Template().CreateReader()
        reader.ProcessColumns(colMap)
        reader.Read(row)
        let user = reader.ToEntity()
        Assert.IsNotNull(user)
        Assert.AreEqual(1, user.UserId)
        Assert.AreEqual("jim", user.Name)

    [<TestMethod>]
    member __.TestReadManyUsers() =
        let colMap =
            [|
                "UserId", ColumnType.Int32
                "Name", ColumnType.String
            |] |> ColumnMap.Parse
        let reader = ReaderTemplate<User list>.Template().CreateReader()
        reader.ProcessColumns(colMap)
        reader.Read(ObjectRow(1, "jim"))
        reader.Read(ObjectRow(1, "jim"))
        reader.Read(ObjectRow(2, "jerry"))
        let users = reader.ToEntity()
        Assert.AreEqual(
            [
                { UserId = 1; Name = "jim" }
                { UserId = 2; Name = "jerry" }
            ],
            users)

    [<TestMethod>]
    member __.TestReadFolder1Level() =
        let colMap =
            [|
                "FolderId", ColumnType.Int32
            |] |> ColumnMap.Parse
        let reader = ReaderTemplate<Folder>.Template().CreateReader()
        reader.ProcessColumns(colMap)
        reader.Read(ObjectRow(1))
        let folder = reader.ToEntity()
        Assert.IsNotNull(folder)
        Assert.AreEqual(1, folder.FolderId)
        Assert.IsNull(folder.Children)

    [<TestMethod>]
    member __.TestReadFolder2Levels() =
        let colMap =
            [|
                "FolderId", ColumnType.Int32
                "Children.FolderId", ColumnType.Int32
            |] |> ColumnMap.Parse
        let reader = ReaderTemplate<Folder>.Template().CreateReader()
        reader.ProcessColumns(colMap)
        reader.Read(ObjectRow(1, 2))
        reader.Read(ObjectRow(1, 3))
        let folder = reader.ToEntity()
        Assert.IsNotNull(folder)
        Assert.AreEqual(1, folder.FolderId)
        Assert.AreEqual(2, folder.Children.Length)
        Assert.AreEqual(2, folder.Children.[0].FolderId)
        Assert.AreEqual(3, folder.Children.[1].FolderId)
        Assert.IsNull(folder.Children.[0].Children)
        Assert.IsNull(folder.Children.[1].Children)
            