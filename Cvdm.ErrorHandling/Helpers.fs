﻿[<AutoOpen>]
module Cvdm.ErrorHandling.Helpers


module Result =

  /// Returns the specified error if the value is false.
  let requireTrue error value =
    if value then Ok () else Error error

  /// Returns the specified error if the value is true.
  let requireFalse error value =
    if not value then Ok () else Error error

  /// Converts an Option to a Result, using the given error if None.
  let requireSome error option =
    match option with
    | Some x -> Ok x
    | None -> Error error

  /// Returns Ok if the two values are equal, or the specified error if not.
  let requireEqualTo other err this =
    if this = other then Ok () else Error err

  /// Replaces a unit error value with a custom error value.
  let withError error result =
    result |> Result.mapError (fun () -> error)

  /// Returns the contained value if Ok, otherwise returns ifError.
  let orElse ifError result =
    match result with
    | Ok x -> x
    | Error _ -> ifError

  /// Returns the contained value if Ok, otherwise evaluates ifErrorThunk and
  /// returns the result.
  let orElseWith ifErrorThunk result =
    match result with
    | Ok x -> x
    | Error _ -> ifErrorThunk ()

  /// If the result is Ok and the predicate returns true, executes the function
  /// on the Ok value. Passes through the input value.
  let teeIf predicate f result =
    match result with
    | Ok x ->
        if predicate x then f x
    | Error _ -> ()
    result

  /// If the result is Ok, executes the function on the Ok value. Passes through
  /// the input value.
  let tee f result =
    teeIf (fun _ -> true) f result

  /// If the result is Error and the predicate returns true, executes the
  /// function on the Error value. Passes through the input value.
  let teeErrorIf predicate f result =
    match result with
    | Ok _ -> ()
    | Error x ->
        if predicate x then f x
    result

  /// If the result is Error, executes the function on the Error value. Passes
  /// through the input value.
  let teeError f result =
    teeErrorIf (fun _ -> true) f result


module AsyncResult =

  let private asyncMap f asnc =
    async {
      let! x = asnc
      return f x
    }

  /// Maps the Ok value of an async-wrapped result.
  let map mapper asyncResult =
    asyncResult |> asyncMap (Result.map mapper)

  /// Maps the Error value of an async-wrapped result.
  let mapError mapper asyncResult =
    asyncResult |> asyncMap (Result.mapError mapper)

  /// Replaces a unit error value of an async-wrapped result with a custom
  /// error value.
  let withError error asyncResult =
    asyncResult |> asyncMap (Result.withError error)

  /// Extracts the contained value of an async-wrapped result if Ok, otherwise
  /// uses ifError.
  let orElse ifError asyncResult =
    asyncResult |> asyncMap (Result.orElse ifError)

  /// Extracts the contained value of an async-wrapped result if Ok, otherwise
  /// evaluates ifErrorThunk and uses the result.
  let orElseWith ifErrorThunk asyncResult =
    asyncResult |> asyncMap (Result.orElseWith ifErrorThunk)

  /// If the async-wrapped result is Ok and the predicate returns true, executes
  /// the function on the Ok value. Passes through the input value.
  let teeIf predicate f asyncResult =
    asyncResult |> asyncMap (Result.teeIf predicate f)

  /// If the async-wrapped result is Ok, executes the function on the Ok value.
  /// Passes through the input value.
  let tee f asyncResult =
    asyncResult |> asyncMap (Result.tee f)

  /// If the async-wrapped result is Error and the predicate returns true,
  /// executes the function on the Error value. Passes through the input value.
  let teeErrorIf predicate f asyncResult =
    asyncResult |> asyncMap (Result.teeErrorIf predicate f)

  /// If the async-wrapped result is Error, executes the function on the Error
  /// value. Passes through the input value.
  let teeError f asyncResult =
    asyncResult |> asyncMap (Result.teeError f)