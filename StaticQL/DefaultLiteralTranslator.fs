﻿namespace StaticQL.Translators
open System
open System.Globalization
open StaticQL
open StaticQL.Mapping
open StaticQL.BackendUtilities

[<AbstractClass>]
type DefaultLiteralTranslator() =
    inherit LiteralTranslator()
    override __.NullLiteral = CommandText "NULL"
    override __.CurrentTimeLiteral = CommandText "CURRENT_TIME"
    override __.CurrentDateLiteral = CommandText "CURRENT_DATE"
    override __.CurrentTimestampLiteral = CommandText "CURRENT_TIMESTAMP"
    override __.IntegerLiteral i = CommandText (i.ToString(CultureInfo.InvariantCulture))
    override __.FloatLiteral f = CommandText (f.ToString("0.0##############", CultureInfo.InvariantCulture))
    override this.Literal literal =
        match literal with
        | NullLiteral -> this.NullLiteral
        | CurrentTimeLiteral -> this.CurrentTimeLiteral
        | CurrentDateLiteral -> this.CurrentDateLiteral
        | CurrentTimestampLiteral -> this.CurrentTimestampLiteral
        | StringLiteral str -> this.StringLiteral(str)
        | BlobLiteral blob -> this.BlobLiteral(blob)
        | NumericLiteral (IntegerLiteral i) -> this.IntegerLiteral(i)
        | NumericLiteral (FloatLiteral f) -> this.FloatLiteral(f)
    override this.SignedLiteral literal =
        let literalValue = literal.Value |> NumericLiteral |> this.Literal
        if literal.Sign >= 0 then Seq.singleton literalValue else
        seq {
            yield text "-"
            yield literalValue
        }

