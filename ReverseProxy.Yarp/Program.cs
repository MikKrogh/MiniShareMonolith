using ReverseProxy.Yarp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<FireBaseJwtValidator>();


builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();


app.MapReverseProxy();

//exposes swagger, maybe bad practice. 
app.Use(async (context, next ) =>
{    
    var method = context.Request.Method;
    var requireAuth = false;

    if (!String.Equals(method, "get", StringComparison.OrdinalIgnoreCase))
        requireAuth = true;

    if (requireAuth)
    {
        var jwtToken = context.Request.Headers["Authorization"].ToString();
        var validator = context.RequestServices.GetRequiredService<FireBaseJwtValidator>();
        var isValid = await validator.ValidateTokenAsync(jwtToken);
        if (!isValid.HasValue || !isValid.Value)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return;
        }
        else await next();        
    }
    else await next();    
});
app.MapGet(string.Empty, ()=> "hello world.");
app.UseHttpsRedirection();
app.Run();
