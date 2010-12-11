SOWIN — Simple Open Web Interface for .NET, v1.0 Draft
======================================================

### IApplication ###

    public interface IApplication
    {
        IResponse Respond(IRequest request);
    }

### IRequest ###

    public interface IRequest
    {
        string Method { get; }
        string Uri { get; }
        string Path { get; }
        
        IRequestHeaderCollection Headers { get; }
        
        T Items<T>(T value);
        T Items<T>();
        
        Stream Body();
    }

### IRequestHeaderCollection ###

    public interface IRequestHeaderCollection
    {
        IRequestHeader this[string index] { get; }
    }

### IRequestHeader ###

    public interface IRequestHeader
    {
        string Value { get; }
        IEnumerable<string> Values { get; }
        bool Provided { get; }
        int Count { get; }    
    }

### IResponse ###

    public interface IResponse
    {
        int Status { get; }
        IEnumerable<IResponseHeader> Headers { get; }        
        Stream Body();
    }

### IResponseHeader ###

    public interface IResponseHeader
    {
        string Name { get; }
        string Value { get; }
    }
