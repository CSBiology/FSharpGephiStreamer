namespace FSharpGephiStreamer

///Helper functions for requests to the Streamer server on the Gephi side
module RestfulAux =
    
    ///Represents error messages returned on error from the streamer server
    type Error = 
        | Http_CreateRequestFailed of string
        | Http_Response of int
        | Http_JsonInvalid
        | CreateJsonFailed of string

    open FSharp.Net

    let private header = [ "content-type", "application/json"]

    ///Generates an error-handled GET request
    let generateGetRequest =
        Either.tryCatch 
            (fun urlString ->  Http.Request (urlString))
            (fun exn -> Error.Http_CreateRequestFailed exn.Message)

    ///Generates an error-handled POST request
    let generatePostRequest urlString =
        Either.tryCatch             
            (fun content ->  Http.Request(urlString, headers = header, meth="POST", body = RequestBody.Text content))
            (fun exn -> Error.Http_CreateRequestFailed exn.Message)

    ///Generates an error-handled PUT request
    let generatePutRequest urlString =
        Either.tryCatch 
            (fun content ->  Http.Request(urlString, headers = header, meth="PUT", body = RequestBody.Text content))
            (fun exn  -> Error.Http_CreateRequestFailed exn.Message)

    ///Generates an error-handled DELETE request
    let generateDelRequest =
        Either.tryCatch 
            (fun urlString ->  Http.Request(urlString, meth = "DELETE" ))
            (fun exn -> Error.Http_CreateRequestFailed exn.Message)


    ///Evaluates a http request to the streamer server. Handles errors without exceptions by returning the Error type instead of throwing an exception.
    let evalHttpResponseText (response:HttpResponse) =
        if response.StatusCode > 199 && response.StatusCode < 300 then            
            match response.Body with
            | ResponseBody.Text r   -> Either.succeed r
            | ResponseBody.Binary r -> Either.fail (Error.Http_JsonInvalid)
        else
            Either.fail (Error.Http_Response response.StatusCode)


