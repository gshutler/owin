using System;

namespace SOWIN
{
    public interface IApplication
    {
        IResponse Respond(IRequest request);
    }
}
