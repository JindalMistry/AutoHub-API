using System;
using System.Collections.Generic;
using System.Text;

namespace AutoHub.Infrastructure.Configuration;

public class StorageSettings
{
    public string Provider { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string BucketName { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public bool UseSSL { get; set; }
}
