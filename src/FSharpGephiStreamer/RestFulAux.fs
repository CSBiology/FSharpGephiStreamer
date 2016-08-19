namespace FSharpGephiStreamer

module RestfulAux =
    
    type Error = 
        | Http_CreateRequestFailed of string
        | Http_Response of int
        | Http_JsonInvalid
        | CreateJsonFailed of string

    open FSharp.Net

    let private header = [ "content-type", "application/json"]

    let generateGetRequest =
        Either.tryCatch 
            (fun urlString ->  Http.Request (urlString))
            (fun exn -> Error.Http_CreateRequestFailed exn.Message)

    let generatePostRequest urlString =
        Either.tryCatch             
            (fun content ->  Http.Request(urlString, headers = header, meth="POST", body = RequestBody.Text content))
            (fun exn -> Error.Http_CreateRequestFailed exn.Message)

    let generatePutRequest urlString =
        Either.tryCatch 
            (fun content ->  Http.Request(urlString, headers = header, meth="PUT", body = RequestBody.Text content))
            (fun exn  -> Error.Http_CreateRequestFailed exn.Message)

    let generateDelRequest =
        Either.tryCatch 
            (fun urlString ->  Http.Request(urlString, meth = "DELETE" ))
            (fun exn -> Error.Http_CreateRequestFailed exn.Message)


    let evalHttpResponseText (response:HttpResponse) =
        if response.StatusCode > 199 && response.StatusCode < 300 then            
            match response.Body with
            | ResponseBody.Text r   -> Either.succeed r
            | ResponseBody.Binary r -> Either.fail (Error.Http_JsonInvalid)
        else
            Either.fail (Error.Http_Response response.StatusCode)


