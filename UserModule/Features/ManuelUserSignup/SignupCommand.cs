﻿namespace UserModule.Features.ManuelUserSignup;

public sealed record SignupCommand
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string DisplayName { get; set; }

}
