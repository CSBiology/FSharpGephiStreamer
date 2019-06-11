namespace FSharpGephiStreamer

///Helper functions for requests to the Streamer server on the Gephi side
module RestfulAux =
    open HttpFs
    open HttpFs.Client
    open Hopac

    ///Represents error messages returned on error from the streamer server
    type Error = 
        | Http_CreateRequestFailed of string
        | Http_SendRequestFailed of string
        | Http_Response of int
        | Http_JsonInvalid
        | CreateJsonFailed of string

    let private cType =
        ContentType.create("application","json")

    ///Generates an error-handled GET request
    let generateGetRequest =
        Either.tryCatch 
            (fun urlString ->  Request.createUrl Get (urlString))
            (fun exn -> Error.Http_CreateRequestFailed exn.Message)

    ///Generates an error-handled POST request
    let generatePostRequest urlString =
        Either.tryCatch             
            (fun content ->  
                Request.createUrl Post (urlString)
                |> Request.setHeader (ContentType cType)
                |> Request.body (RequestBody.BodyString content))

            (fun exn -> Error.Http_CreateRequestFailed exn.Message)

    ///Generates an error-handled PUT request
    let generatePutRequest urlString =
        Either.tryCatch 
            (fun content ->  
                Request.createUrl Put (urlString)
                |> Request.setHeader (ContentType cType)
                |> Request.body (RequestBody.BodyString content))
            (fun exn  -> Error.Http_CreateRequestFailed exn.Message)

    ///Generates an error-handled DELETE request
    let generateDelRequest =
        Either.tryCatch 
            (fun urlString ->  Request.createUrl Delete (urlString))
            (fun exn -> Error.Http_CreateRequestFailed exn.Message)

    let evalHttpResponseText =
            (fun r -> 
                job {
                  use! response = getResponse r // disposed at the end of async, don't
                                                      // fetch outside async body
                  // the above doesn't download the response, so you'll have to do that:
                  let! bodyStr = Response.readBodyAsString response
                  if  response.statusCode > 199 && response.statusCode < 300 then
                      return Either.succeed bodyStr
                  else 
                      return Either.fail (Error.Http_Response response.statusCode)

                  // OR:
                  //let! bodyBs = Response.readBodyAsBytes

                  // remember HttpFs doesn't buffer the stream (how would we know if we're
                  // downloading 3GiB?), so once you use one of the above methods, you can't do it
                  // again, but have to buffer/stash it yourself somewhere.
                }
                |> run
                )


    /////Evaluates a http request to the streamer server. Handles errors without exceptions by returning the Error type instead of throwing an exception.
    //let evalHttpResponseText (response:Response) =
    //    if response.statusCode > 199 && response.statusCode < 300 then            
    //        match response.Body with
    //        | ResponseBody.Text r   -> Either.succeed r
    //        | ResponseBody.Binary r -> Either.fail (Error.Http_JsonInvalid)
    //    else
    //        Either.fail (Error.Http_Response response.StatusCode)


