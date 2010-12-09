OWIN — Open Web Interface for .NET, v1.0 Draft
==============================================

## Overview ##

This document defines a standard interface between .NET web servers and web applications. The goal of the OWIN interface is to decouple server and application, encourage the development of simple modules for .NET web development, and, by being an open standard, stimulate the open source ecosystem of .NET web development tools.

## Definition ##

OWIN comprises three core interfaces: `IApplication`, `IRequest`, and `IResponse`. Broadly speaking, hosts provide application objects with request objects, and application objects provide response objects back to the server. In this document, an OWIN-compatible web server is referred to as a “host”, and an object implementing `IApplication` is referred to as an “application”. How an application is provided to a host is outside the scope of this specification.

### IApplication ###

    public interface IApplication
    {
        IResponse Respond(IRequest request);
    }

Applications generate responses to requests received by a host by implementing the `IApplication` interface, which defines single synchronous method. Applications should always generate a non-null response. 

### IRequest ###

    public interface IRequest
    {
        string Method { get; }
        string Path { get; }
        
        IDictionary<string, IHeader> Headers { get; }
        IDictionary<string, object> Items { get; }
        
        Stream Body();
    }

### IHeader ###

    public interface IHeader
    {
        string Value { get; }
        IEnumerable<string> Values { get; }
    }

The `Method` property is the HTTP request method string of the request (e.g., `"GET"`, `"POST"`).

The `Path` property is the part of the request URI's path relevant to the application. See [Paths](#Paths). The value of the `Path` property includes the query string of the request URI (e.g., “/path/and?query=string”).  

The `Headers` property is a dictionary whose items correspond to HTTP headers in the request. Keys for headers should be the same case as they appear in the HTTP specification. Values are instances of `IHeader` containing the corresponding header value or values. An `IHeader` has two properties, `Value` and `Values`. If there are multiple values associated with an `IHeader` then `Value` will always return the first value yielded by `Values`. `Values` should always yield values in the same order, in the case of headers that should be the same as the order they appeared in the request. Requesting a parameter which has not been set should return an instance of `IHeader` which returns `string.Empty` from `Value` and an empty enumerator from `Values`.

The `Items` property is a dicitonary whose items hold objects relating to the state of the request. Like the `Headers` property, requesting a parameter that has not been set should return `null`.

Hosts should provide the following keys in `Items`:

- `OWIN.BasePath` – The portion of the request URI’s path corresponding to the “root” of the application object. See [Paths](#Paths).
- `OWIN.ServerName`, `OWIN.ServerPort` – Hosts should provide values can be used to reconstruct the full URL of the request in absence of the HTTP `Host` header of the request.
- `OWIN.UrlScheme` – `"http"` or `"https"`
- `OWIN.RemoteEndPoint` — A `System.Net.IPEndPoint` representing the connected client.

`Body` returns a readable, seekable `Stream` for the request body that the consumer is responsible for disposing of.

### IResponse ###

    public interface IResponse
    {
        int Status { get; }
        IDictionary<string, IHeader> Headers { get; }
        
        Stream Body();
    }

The `Status` property is a string containing the integer status of the response followed by a space and a reason phrase without a newline (e.g., `"200 OK"`). All characters in the status string provided by an application should be within the ASCII codepage.

The `Headers` property is a dictionary representing the headers to be sent with the request. Keys must be header names without `':'` or whitespace. Values must be `IEnumerable<string>` sequences containing the corresponding header value strings, without newlines. If the sequence value for a header name contains multiple elements, the host should write a header name-value line with that name once for each value in the sequence. All characters in header name and value strings should be within the ASCII codepage.

`Body` returns a readable, seekable `Stream` for the response body that the consumer is responsible for disposing of.

## Paths ##

Some hosts may have the ability to map application objects to some base path. For example, a host may have an application object configured to respond to requests beginning with `"/my-app"`, in which case it must set the value of `"owin.BasePath"` in `IRequest.Items` to `"/my-app"`. If this host receives a request for `"/my-app/foo"`, the `Uri` property of the `IRequest` object provided to the application at `"/my-app"` must be `"/foo"`. The value of `"owin.BasePath"` may be an empty string and must not end with a trailing slash; the value of the `URI` property must start with a slash.

## Error Handling ##

### Application Errors ###

Applications may throw exceptions in the following places:

- The `IApplication.Respond` method

Host implementations should strive to write response data to the network as “late” as possible, so as to be able to handle as many errors from the application as possible and cleanly send the client a 500-level response. If an error occurs before data is written to the network, the server should provide a 500-level response. If an error occurs enumerating subsequent items from the response body enumerable, the host may append a textual description of the error to the response data which it has already sent and close the connection.

### Host Errors ###

Hosts may throw exceptions during the transmission of the response, such as if the recipient disconnected. Such failures should be propogated back to the application so they are aware of the failure.
