﻿namespace Webpay.Integration.Response;

public class Response
{
    public bool OrderAccepted { get; set; }
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
}